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

        public static readonly ErrorCode UnalignedLiteral = new ErrorCode(
            "tokenizer_unaligned_literal",
            ErrorSeverity.Error,
            "Literal is not properly vertically aligned."
        );

        public static readonly ErrorCode InvalidBlockShape = new ErrorCode(
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

        private List<(Token startToken, int startTokenIndex)> blockData = new();
        private SimpleReader reader = null!;

        public (Token[], Error[]) Run() {
            if (started) {
                throw new InvalidOperationException("Already run");
            }
            started = true;

            SimpleTokenizerInstance simpleTokenizer = new SimpleTokenizerInstance(image);
            reader = new SimpleReader(simpleTokenizer.Run());

            while (reader.HasNext()) {
                var simpleToken = reader.Read();

                switch (simpleToken.Type) {
                    case SimpleTokenType.Identifier: 
                        ParseIdentifier(simpleToken);
                        break;

                    case SimpleTokenType.Indentation:
                        // Can only happen if we start the file with an indent, so go back one and pretend we just read a newline
                        reader.Rewind();
                        ParseIndentation(simpleToken);
                        break;


                    case SimpleTokenType.LineBreak:
                        ParseIndentation(simpleToken);
                        break;


                    case SimpleTokenType.Keyword:
                        ParseKeyword(simpleToken);
                        break;


                    case SimpleTokenType.Number:
                    case SimpleTokenType.String: 
                        SimpleToken endToken = simpleToken;

                        while (reader.Peek().Type == simpleToken.Type) {
                            endToken = reader.Read();
                        }

                        TokenType type = simpleToken.Type == SimpleTokenType.Number
                            ? TokenType.NumberLiteral
                            : TokenType.StringLiteral;

                        output.Add(new Token(Rectangle.Union(simpleToken.Position, endToken.Position), type));
                        break;
                    

                    default:
                        // todo error
                        break;
                }
            }

            return (output.ToArray(), errors.ToArray());
        }

        private void ParseIndentation(SimpleToken simpleToken) {
            int indentation = 0;
            // Read indentations after a newline

            reader.PushState();

            while (reader.Peek().Type == SimpleTokenType.Indentation) {
                SimpleToken indentToken = reader.Read();

                if (indentation < blockData.Count) {
                    (Token startToken, _) = blockData[indentation];

                    if (indentToken.Position.X0 != startToken.Position.X0 ||
                        indentToken.Position.X1 != startToken.Position.X1) {
                        // Todo: complain about misalignment
                    }
                }

                indentation++;
            }

            reader.PopState();

            // Add any closing blocks before the newline
            while (indentation < blockData.Count) {

                var (startToken, index) = blockData.Last();
                blockData.RemoveAt(blockData.Count-1);


                // Expand up and down as far as possible to capture entire block line for visualization
                int x = startToken.Position.X0;
                int y0 = startToken.Position.Y0;

                while (y0 > 0) {
                    if (image[x, y0 - 1] != BlockLineColor) {
                        break;
                    }
                    y0--;
                }

                int y1 = startToken.Position.Y1;
                while (y1 < image.Height) {
                    if (image[x, y1] != BlockLineColor) {
                        break;
                    }

                    y1++;
                }

                Rectangle closingRect = new(x, y0, x + 1, y1);

                // Swap out the starting block with a new one with updated size information
                output[index] = new Token(closingRect, TokenType.StartBlock);

                output.Add(new Token(closingRect, TokenType.EndBlock));
            }

            output.Add(new Token(simpleToken.Position, TokenType.NewLine));

            indentation = 0;

            // Add any opening blocks after the newline
            while (reader.Peek().Type == SimpleTokenType.Indentation) {
                SimpleToken indentToken = reader.Read();
                indentation++;

                if (indentation > blockData.Count) {
                    Token startBlockToken = new Token(indentToken.Position, TokenType.StartBlock);
                    blockData.Add((startBlockToken, output.Count));

                    output.Add(startBlockToken);
                }

            }

            return;
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
                    AreaOneColor(new Rectangle(x0, y0, x0+1, y1), PaletteColor.Black) &&
                    AreaEmpty(new Rectangle(x0 + 1, y0+1, x0, y1-1))
                    ) {
                    output.Add(new Token(position, TokenType.BracketL));
                    return;
                }

                if (
                    AreaOneColor(new Rectangle(x1-1, y0, x1, y1), PaletteColor.Black) &&
                    AreaEmpty(new Rectangle(x1 - 1, y0 + 1, x1, y1 - 1))
                ) {
                    output.Add(new Token(position, TokenType.BracketR));
                    return;
                }
            }

            if (IdentifierFrame.TryParseFromRaw(position, image, out IdentifierFrame? frame) == false) {
                errors.Add(new Error(TokenizerErrors.UnknownKeyword, position));
                return;
            }

            if (KeywordLibrary.TryGetOperator(frame, out TokenType operatorType) == false) {
                errors.Add(new Error(TokenizerErrors.UnknownKeyword, position));
                return;
            }

            output.Add(new Token(position, operatorType));
        }

        private void ParseIdentifier(SimpleToken simpleToken) {
            bool isInitializer = true;

            Rectangle pos = simpleToken.Position;

            // Check if the bottom is a variable initializer
            int bottomY = pos.Y1 - 1;
            for (int x = pos.X0; x < pos.X1; x++) {
                if (image[x, bottomY] != OperatorColor) {
                    isInitializer = false;
                    break;
                }
            }

            // Check if there's at least two nubs
            if (isInitializer) {
                Point leftPoint = new(pos.X0, pos.Y1 - 2);
                Point rightPoint = new(pos.X1 - 1, pos.Y1 - 2);

                if ((pos.Contains(leftPoint) && image[leftPoint] == OperatorColor && 
                     pos.Contains(rightPoint) && image[rightPoint] == OperatorColor) == false) {
                    isInitializer = false;
                }
            }

            if (isInitializer == false) {
                output.Add(new Token(pos, TokenType.Identifier));
                return;
            }

            (int count, bool hasInvalid) CountSide(int x) {
                int count = 0;
                bool sawWhitespace = false;
                bool sawInvalidColor = false;
                for (int y = pos.Y1-1; y >= pos.Y0; y--) {
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
            var rightSide = CountSide(pos.X1 - 1);

            if (leftSide.hasInvalid || rightSide.hasInvalid) {
                errors.Add(new Error(TokenizerErrors.DeclarationSidesNonWhitespace, pos));
            }

            if (leftSide.count != rightSide.count) {
                errors.Add(new Error(TokenizerErrors.DeclarationUnevenError, pos));
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
                    errors.Add(new Error(TokenizerErrors.DeclarationUnclearError, pos));
                }
            }

            Rectangle trimToContent = pos;
            trimToContent = TrimToContent(new Rectangle(trimToContent.X0+1, trimToContent.Y0, trimToContent.X1-1, trimToContent.Y1-1));
            output.Add(new Token(pos, leftToken));
            
            output.Add(new IdentifierToken(trimToContent, ParseFrame(trimToContent)));
            
            output.Add(new Token(pos, rightToken));
        }

        private Rectangle TrimToContent(Rectangle content) {
            var (x0, y0, x1, y1) = content;

            // Trim top
            for (; y0 < y1; y0++) {
                if (AreaEmpty(new(x0, y0, x1, y0+1)) == false) {
                    break;
                }
            }
            
            // Trim bot
            for (; y1 > y0; y1--) {
                if (AreaEmpty(new(x0, y1-1, x1, y1)) == false) {
                    break;
                }
            }

            // Trim left
            for (; x0 < x1; x0++) {
                if (AreaEmpty(new(x0, y0, x0 + 1, y1)) == false) {
                    break;
                }
            }

            // Trim right
            for (; x1 > x0; x1--) {
                if (AreaEmpty(new(x1 - 1, y0, x1, y1)) == false) {
                    break;
                }
            }

            return new Rectangle(x0, y0, x1, y1);
        }

        private bool AreaEmpty(Rectangle area) {
            for (int x = area.X0; x < area.X1; x++) {
                for (int y = area.Y0; y < area.Y1; y++) {
                    if (image[x, y] != WhiteSpace) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AreaOneColor(Rectangle area, PaletteColor color) {
            RawColor rawColor = color.Color;
            for (int x = area.X0; x < area.X1; x++) {
                for (int y = area.Y0; y < area.Y1; y++) {
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

            for (int x = position.X0; x < position.X1; x++) {
                for (int y = position.Y0; y < position.Y1; y++) {

                    if (PaletteColor.TryFromRaw(image[x, y], out PaletteColor color) == false) {
                        if (complainedAboutPalette == false) {
                            errors.Add(new Error(TokenizerErrors.ColorNotInPalette, position));
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