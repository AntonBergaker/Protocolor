using System.Text;
using Protocolor.Util;

namespace Protocolor.Tokenization; 

public partial class Tokenizer {
    public static class TokenizerErrors {
        public static readonly ErrorCode DeclarationUnclearError = new ErrorCode(
            "tokenizer_declaration_unclear_mutability",
            ErrorSeverity.Error,
            "Variable declaration is too tall to be a mutable variable, but need to cover the whole identifier to be considered an immutable variable."
        );

        public static readonly ErrorCode DeclarationUnevenError = new ErrorCode(
            "tokenizer_declaration_uneven_sides",
            ErrorSeverity.Error,
            "Variable declaration has uneven height of the sides."
        );

        public static readonly ErrorCode DeclarationSidesNonWhitespace = new ErrorCode(
            "tokenizer_declaration_sides_not_whitespace",
            ErrorSeverity.Error,
            "Variable declaration contains pixels that are not whitespace. If identifier was not intended to be a variable declaration, make sure it does not contain only black pixels on its lowest line."
        );

        public static readonly ErrorCode UnknownKeyword = new ErrorCode(
            "tokenizer_keyword_unknown",
            ErrorSeverity.Error,
            "The keyword was not recognized. If this was not meant to be a keyword, a pixel that is not black must be included."
        );

        public static readonly ErrorCode StringLiteralUnaligned = new ErrorCode(
            "tokenizer_string_literal_unaligned",
            ErrorSeverity.Error,
            "String literal is not properly vertically aligned."
        );

        public static readonly ErrorCode StringLiteralUnknownCharacter = new ErrorCode(
            "tokenizer_string_literal_unknown_character",
            ErrorSeverity.Error,
            "The character did not match any known string literal characters."
        );

        public static readonly ErrorCode StringLiteralAmbiguousCharacter = new ErrorCode(
            "tokenizer_string_literal_unknown_character",
            ErrorSeverity.Error,
            "The character was found but was ambiguous because of its position."
        );

        public static readonly ErrorCode StringLiteralMissingFullUnderline = new ErrorCode(
            "tokenizer_string_literal_missing_full_underline",
            ErrorSeverity.Error,
            "A character was partially underlined. Capitalized characters require the entire character to be underlined."
        );

        public static readonly ErrorCode NumberLiteralUnaligned = new ErrorCode(
            "tokenizer_number_literal_unaligned",
            ErrorSeverity.Error,
            "Number literal is not properly vertically aligned."
        );

        public static readonly ErrorCode NumberLiteralUnknownCharacter = new ErrorCode(
            "tokenizer_number_unknown_character",
            ErrorSeverity.Error,
            "The character did not match any known numbers."
        );

        public static readonly ErrorCode BlockShapeInvalid = new ErrorCode(
            "tokenizer_invalid_block_shape",
            ErrorSeverity.Error,
            "Block is not properly aligned or filled in. Block must cover the entirety of the lines it encompasses and connect to the rest of the block."
        );

        public static readonly ErrorCode ColorNotInPalette = new ErrorCode(
            "tokenizer_color_not_in_palette",
            ErrorSeverity.Error,
            "The provided color is not part of the paint palette."
        );
    }

    private class TokenizerInstance {
        private readonly Grid<RawColor> image;
        private readonly List<Token> output;
        private readonly List<Error> errors;
        private bool started;

        public TokenizerInstance(Grid<RawColor> image) {
            this.image = image;
            output = new();
            errors = new();
        }

        private class BlockEntry {
            public Token StartToken { get; }
            public SimpleToken PreviousToken { get; set; }
            public int StartTokenIndex { get; }

            public BlockEntry(Token startToken, SimpleToken previousToken, int startTokenIndex) {
                StartToken = startToken;
                PreviousToken = previousToken;
                StartTokenIndex = startTokenIndex;
            }
        }

        private readonly List<BlockEntry> blockData = new();
        private SimpleReader reader = null!;

        public (Token[], Error[]) Run() {
            if (started) {
                throw new InvalidOperationException("Already run");
            }
            started = true;

            SimpleTokenizerInstance simpleTokenizer = new SimpleTokenizerInstance(image);

            var (simpleTokens, simpleTokenErrors) = simpleTokenizer.Run();

            errors.AddRange(simpleTokenErrors);
            reader = new SimpleReader(simpleTokens);

            while (reader.HasNext) {
                var simpleToken = reader.Read();

                switch (simpleToken.Type) {
                    case SimpleTokenType.Identifier: 
                        ParseIdentifier(simpleToken);
                        break;

                    case SimpleTokenType.Indentation:
                        // Can only happen if we start the file with an indent, so go back one and pretend we just read a newline
                        reader.Rewind();
                        ParseIndentation(simpleToken, false);
                        break;


                    case SimpleTokenType.LineBreak:
                        ParseIndentation(simpleToken, true);
                        break;


                    case SimpleTokenType.Keyword:
                        ParseKeyword(simpleToken);
                        break;


                    case SimpleTokenType.Number:
                        ParseNumberLiteral(simpleToken);
                        break;

                    case SimpleTokenType.String: 
                        ParseStringLiteral(simpleToken);
                        break;

                    default:
                        throw new Exception($"Encountered an invalid SimpleToken {simpleToken}");

                }
            }

            return (output.ToArray(), errors.ToArray());
        }

        private void ParseIndentation(SimpleToken simpleToken, bool cameFromNewline) {
            int indentation = 0;
            // Read indentations after a newline

            reader.PushState();

            while (reader.Peek().Type == SimpleTokenType.Indentation) {
                SimpleToken indentToken = reader.Read();

                if (indentation < blockData.Count) {
                    var blockEntry = blockData[indentation];

                    var prePos = blockEntry.PreviousToken.Position;
                    var startPos = blockEntry.StartToken.Position;

                    // If the previous block and this block are not connected, they are separate blocks and we must close this block
                    bool hasEmptyArea = false;
                    int minX = Math.Min(indentToken.Position.X0, prePos.X0);
                    int maxX = Math.Max(indentToken.Position.X0, prePos.X0);

                    for (int y = prePos.Y1 + 1; y < indentToken.Position.Y1; y++) {
                        if (AreaEmpty(new Rectangle(minX, y, maxX, y))) {
                            hasEmptyArea = true;
                            break;
                        }
                    }

                    if (hasEmptyArea) {
                        // Obviously only possible at top level
                        if (indentation == blockData.Count - 1) {
                            CloseBlock();
                            break;
                        } else {
                            AddError(TokenizerErrors.BlockShapeInvalid, Rectangle.Union(startPos, indentToken.Position));
                        }
                        continue;
                    }


                    // See that they start at the same X positions
                    if (indentToken.Position.X0 != startPos.X0 ||
                        indentToken.Position.X1 != startPos.X1) {
                        AddError(TokenizerErrors.BlockShapeInvalid, Rectangle.Union(indentToken.Position, startPos));
                    }

                    blockEntry.PreviousToken = indentToken;
                }

                indentation++;
            }

            reader.PopState();

            // Add any closing blocks before the newline
            while (indentation < blockData.Count) {
                CloseBlock();
            }

            if (cameFromNewline) {
                output.Add(new Token(simpleToken.Position, TokenType.NewLine));
            }

            indentation = 0;

            // Add any opening blocks after the newline
            while (reader.Peek().Type == SimpleTokenType.Indentation) {
                SimpleToken indentToken = reader.Read();
                indentation++;

                if (indentation > blockData.Count) {
                    Token startBlockToken = new Token(indentToken.Position, TokenType.StartBlock);
                    blockData.Add(new (startBlockToken, indentToken, output.Count));

                    output.Add(startBlockToken);
                }

            }

            return;
        }

        private void CloseBlock() {
            var blockEntry = blockData[^1];
            blockData.RemoveAt(blockData.Count - 1);

            var startPos = blockEntry.StartToken.Position;

            // Expand up and down as far as possible to capture entire block line for visualization
            int x = startPos.X0;
            int y0 = startPos.Y0;

            while (y0 > 0) {
                if (image[x, y0 - 1] != BlockLineColor) {
                    break;
                }
                y0--;
            }

            int y1 = startPos.Y1;
            while (y1 < image.Height - 1) {
                if (image[x, y1 + 1] != BlockLineColor) {
                    break;
                }

                y1++;
            }

            Rectangle closingRect = new(x, y0, x, y1);

            // Swap out the starting block with a new one with updated size information
            output[blockEntry.StartTokenIndex] = new Token(closingRect, TokenType.StartBlock);

            output.Add(new Token(closingRect, TokenType.EndBlock));
        }

        private void ParseKeyword(SimpleToken simpleToken) {
            Rectangle position = simpleToken.Position;

            // Check for variable height pipes
            if (position.Width == 1 && position.Height >= 3) {
                output.Add(new Token(position, TokenType.Pipe));
                return;
            }

            if (position.Width == 2 && position.Height >= 3) {
                var (x0, y0, x1, y1) = position;
                if (
                    AreaOneColor(new Rectangle(x0, y0, x0, y1), PaletteColor.Black) &&
                    AreaEmpty(new Rectangle(x0 + 1, y0+1, x0 + 1, y1-1))
                    ) {
                    output.Add(new Token(position, TokenType.BracketL));
                    return;
                }

                if (
                    AreaOneColor(new Rectangle(x1, y0, x1, y1), PaletteColor.Black) &&
                    AreaEmpty(new Rectangle(x1 - 1, y0 + 1, x1 - 1, y1 - 1))
                ) {
                    output.Add(new Token(position, TokenType.BracketR));
                    return;
                }
            }

            ShapeFrame frame = ShapeFrame.ParseFromRaw(position, image);

            if (KeywordLibrary.TryGetOperator(frame, out TokenType operatorType) == false) {
                AddError(TokenizerErrors.UnknownKeyword, position);
                return;
            }

            output.Add(new Token(position, operatorType));
        }

        private void ParseIdentifier(SimpleToken simpleToken) {
            bool isInitializer = true;

            Rectangle pos = simpleToken.Position;

            // Check if the bottom is a variable initializer
            for (int x = pos.X0; x <= pos.X1; x++) {
                if (image[x, pos.Y1] != OperatorColor) {
                    isInitializer = false;
                    break;
                }
            }

            // Check if there's at least two nubs
            if (isInitializer) {
                Point leftPoint = new(pos.X0, pos.Y1 - 1);
                Point rightPoint = new(pos.X1, pos.Y1 - 1);

                if ((pos.Contains(leftPoint) && image[leftPoint] == OperatorColor && 
                     pos.Contains(rightPoint) && image[rightPoint] == OperatorColor) == false) {
                    isInitializer = false;
                }
            }

            if (isInitializer == false) {
                if (IdentifierFrame.TryParseFromRaw(pos, image, out var frame)) {
                    output.Add(new IdentifierToken(frame, pos));
                } else {
                    AddError(TokenizerErrors.ColorNotInPalette, pos);
                }

                return;
            }

            (int count, bool hasInvalid) CountSide(int x) {
                int count = 0;
                bool sawWhitespace = false;
                bool sawInvalidColor = false;
                for (int y = pos.Y1; y >= pos.Y0; y--) {
                    if (image[x, y] == OperatorColor) {
                        if (sawWhitespace == false) {
                            count++;
                        }
                    }
                    else if (image[x, y] == WhiteSpace) {
                        sawWhitespace = true;
                    } else {
                        sawInvalidColor = true;
                    }
                }

                return (count, sawInvalidColor);
            }

            // Count the length of the sides, and that any extra space is whitespace
            var leftSide = CountSide(pos.X0);
            var rightSide = CountSide(pos.X1);

            if (leftSide.hasInvalid || rightSide.hasInvalid) {
                AddError(TokenizerErrors.DeclarationSidesNonWhitespace, pos);
            }

            if (leftSide.count != rightSide.count) {
                AddError(TokenizerErrors.DeclarationUnevenError, pos);
            }

            int height = Math.Max(leftSide.count, rightSide.count);

            TokenType leftToken, rightToken;

            if (height <= 2) {
                leftToken = TokenType.VarDeclarationL;
                rightToken = TokenType.VarDeclarationR;
            } else {
                leftToken = TokenType.ConstDeclarationL;
                rightToken = TokenType.ConstDeclarationR;

                if (height < pos.Height) {
                    AddError(TokenizerErrors.DeclarationUnclearError, pos);
                }
            }

            Rectangle trimToContent = pos;
            trimToContent = TrimToContent(new Rectangle(trimToContent.X0+1, trimToContent.Y0, trimToContent.X1-1, trimToContent.Y1-1));
            output.Add(new Token(pos, leftToken));
            
            output.Add(new IdentifierToken(ParseFrame(trimToContent), trimToContent));
            
            output.Add(new Token(pos, rightToken));
        }

        private void ParseStringLiteral(SimpleToken startToken) {
            StringBuilder content = new StringBuilder();

            // Read all tokens and put them in a list so we can manip easier
            var tokenList = new List<SimpleToken>() {
                startToken
            };

            int forcedY1 = -1;

            while (reader.Peek().Type == SimpleTokenType.String) {
                tokenList.Add(reader.Read());
            }

            List<(Rectangle rect, bool isCapitalized)> tokenPosList = new();

            foreach (SimpleToken simpleToken in tokenList) {
                Rectangle tokenPos = simpleToken.Position;

                // Look for capitalization underline
                bool hasBlackAtBottom = false;

                for (int x = tokenPos.X0; x <= tokenPos.X1; x++) {
                    if (image[x, tokenPos.Y1] == OperatorColor) {
                        hasBlackAtBottom = true;
                        break;
                    }
                }

                // No black, already parsed
                if (hasBlackAtBottom == false) {
                    tokenPosList.Add((tokenPos, false));
                    continue;
                }

                if (forcedY1 == -1) {
                    forcedY1 = tokenPos.Y1;
                } else {
                    if (forcedY1 != tokenPos.Y1) {
                        AddError(TokenizerErrors.StringLiteralUnaligned, tokenPos);
                    }
                }

                tokenPos = new Rectangle(tokenPos.Point0, tokenPos.Point1 - new Point(0, 1));

                void AddTokenPos(Rectangle position) {
                    // Check for complete black underline
                    bool hasBlack = false;
                    bool hasWhite = false;

                    for (int x = position.X0; x <= position.X1; x++) {
                        if (image[x, position.Y1 + 1] == OperatorColor) {
                            hasBlack = true;
                        }
                        if (image[x, position.Y1 + 1] == WhiteSpace) {
                            hasWhite = true;
                        }
                    }

                    if (hasBlack && hasWhite) {
                        AddError(TokenizerErrors.StringLiteralMissingFullUnderline, position);
                    }

                    tokenPosList.Add((TrimToContent(position), hasWhite == false));
                }

                int startColumn = tokenPos.X0;
                // Look for additional splits, since the underline was parsed as an entire token we might be able to split it further
                for (int x = tokenPos.X0; x <= tokenPos.X1; x++) {
                    bool emptyColumn = AreaEmpty(new Rectangle(x, tokenPos.Y0, x, tokenPos.Y1));

                    if (emptyColumn && startColumn == x) {
                        startColumn = x+1;
                        continue;
                    }

                    if (emptyColumn) {
                        AddTokenPos(new Rectangle(startColumn, tokenPos.Y0, x - 1, tokenPos.Y1));
                        startColumn = x + 1;
                    }
                }

                // Add stray
                if (startColumn <= tokenPos.X1) {
                    AddTokenPos(new Rectangle(startColumn, tokenPos.Y0, tokenPos.X1, tokenPos.Y1));
                }
            }

            Rectangle position = tokenPosList.Select(x => x.rect).UnionAll();
            if (forcedY1 != -1) {
                position = new(position.X0, Math.Min(position.Y0, forcedY1 - 4), position.X1, forcedY1 - 1);
            }

            if (position.Height > 4) {
                AddError(TokenizerErrors.StringLiteralUnaligned, position);
                output.Add(new StringLiteralToken("", position));
                return;
            }

            foreach (var tokenData in tokenPosList) {
                var (tokenPos, isCapitalized) = tokenData;

                void AppendToken(CharacterLibrary.CharacterData data) {
                    if (isCapitalized && data.UppercaseLetter != '\0') {
                        content.Append(data.UppercaseLetter);
                    } else {
                        content.Append(data.LowercaseLetter);
                    }
                }

                ShapeFrame frame = ShapeFrame.ParseFromRaw(tokenPos, image);
                var possibleCharacters = CharacterLibrary.GetCharactersForShape(frame);

                if (possibleCharacters.Length == 0) {
                    AddError(TokenizerErrors.StringLiteralUnknownCharacter, tokenPos);
                    continue;
                }

                if (possibleCharacters.Length == 1) {
                    AppendToken(possibleCharacters[0]);
                    continue;
                }

                bool found = false;
                foreach (var possibleCharacter in possibleCharacters) {
                    if (position.Height == 4 && possibleCharacter.Offset == tokenPos.Y0 - position.Y0) {
                        AppendToken(possibleCharacter);
                        found = true;
                        break;
                    }
                }

                if (found == false) {
                    AddError(TokenizerErrors.StringLiteralAmbiguousCharacter, tokenPos);
                }

            }

            output.Add(new StringLiteralToken(content.ToString(), position));
        }

        private static readonly HashSet<char> ValidNumberChars = "0123456789.-".ToHashSet();
        private void ParseNumberLiteral(SimpleToken startToken) {
            StringBuilder content = new StringBuilder();

            var list = new List<SimpleToken>() {
                startToken
            };

            while (reader.Peek().Type == SimpleTokenType.Number) {
                list.Add(reader.Read());
            }

            Rectangle position = list.Select(x => x.Position).UnionAll();

            if (position.Height > 4) {
                AddError(TokenizerErrors.NumberLiteralUnaligned, position);
                output.Add(new NumberLiteralToken("0", position));
                return;
            }

            foreach (SimpleToken token in list) {
                ShapeFrame frame = ShapeFrame.ParseFromRaw(token.Position, image);
                var possibleCharacters = CharacterLibrary.GetCharactersForShape(frame);

                var validCharacter = possibleCharacters.FirstOrDefault(x => ValidNumberChars.Contains(x.LowercaseLetter));

                if (validCharacter == null) {
                    AddError(TokenizerErrors.NumberLiteralUnknownCharacter, token.Position);
                    continue;
                }

                content.Append(validCharacter.LowercaseLetter);
            }

            output.Add(new NumberLiteralToken(content.ToString(), position));
        }

        private void AddError(ErrorCode code, Rectangle position) {
            errors.Add(new Error(code, position));
        }

        private Rectangle TrimToContent(Rectangle content) {
            var (x0, y0, x1, y1) = content;

            // Trim top
            for (; y0 < y1; y0++) {
                if (AreaEmpty(new(x0, y0, x1, y0)) == false) {
                    break;
                }
            }
            
            // Trim bot
            for (; y1 > y0; y1--) {
                if (AreaEmpty(new(x0, y1, x1, y1)) == false) {
                    break;
                }
            }

            // Trim left
            for (; x0 < x1; x0++) {
                if (AreaEmpty(new(x0, y0, x0, y1)) == false) {
                    break;
                }
            }

            // Trim right
            for (; x1 > x0; x1--) {
                if (AreaEmpty(new(x1, y0, x1, y1)) == false) {
                    break;
                }
            }

            return new Rectangle(x0, y0, x1, y1);
        }

        private bool AreaEmpty(Rectangle area) {
            for (int x = area.X0; x <= area.X1; x++) {
                for (int y = area.Y0; y <= area.Y1; y++) {
                    if (image[x, y] != WhiteSpace) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AreaOneColor(Rectangle area, PaletteColor color) {
            RawColor rawColor = color.Color;
            for (int x = area.X0; x <= area.X1; x++) {
                for (int y = area.Y0; y <= area.Y1; y++) {
                    if (image[x, y] != rawColor) {
                        return false;
                    }
                }
            }

            return true;
        }

        private IdentifierFrame ParseFrame(Rectangle position) {
            Grid<PaletteColor> colors = new Grid<PaletteColor>(position.Width, position.Height);
            bool complainedAboutPalette = false;

            for (int x = position.X0; x <= position.X1; x++) {
                for (int y = position.Y0; y <= position.Y1; y++) {

                    if (PaletteColor.TryFromRaw(image[x, y], out PaletteColor color) == false) {
                        if (complainedAboutPalette == false) {
                            AddError(TokenizerErrors.ColorNotInPalette, position);
                            complainedAboutPalette = true;
                        }

                        // Default to light gray I guess.
                        // TODO make it pick the closest color
                        color = PaletteColor.LightGray;
                    }

                    colors[x - position.X0, y - position.Y0] = color;
                }
            }

            return new IdentifierFrame(colors);
        }
    }
    
    public (Token[], Error[]) Tokenize(Grid<RawColor> image) {
        TokenizerInstance instance = new(image);
        return instance.Run();

    }
}