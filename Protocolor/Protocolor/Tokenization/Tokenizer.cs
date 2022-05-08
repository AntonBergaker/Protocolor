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
    }

    private class TokenizerInstance {
        private readonly Grid<RawColor> image;
        private readonly List<Token> output;
        private readonly List<Error> errors;
        private bool started;
        private int i;

        public TokenizerInstance(Grid<RawColor> image) {
            this.image = image;
            output = new();
            errors = new();
        }

        public (Token[], Error[]) Run() {
            if (started) {
                throw new InvalidOperationException("Already run");
            }
            started = true;

            SimpleTokenizerInstance simpleTokenizer = new SimpleTokenizerInstance(image);
            var simpleTokens = simpleTokenizer.Run();

            for (; i < simpleTokens.Length; i++) {
                var simpleToken = simpleTokens[i];

                switch (simpleToken.Type) {
                    case SimpleTokenType.Identifier:
                        ParseIdentifier(simpleToken);
                        break;

                    case SimpleTokenType.LineBreak:
                        output.Add(new Token(simpleToken.Position, TokenType.NewLine));
                        break;

                    case SimpleTokenType.Keyword: 
                        ParseKeyword(simpleToken);
                        break;

                    case SimpleTokenType.Number:
                    case SimpleTokenType.String: 
                        SimpleToken endToken = simpleToken;

                        // Find last number token
                        int j = i;
                        for (; j < simpleTokens.Length - 1; j++) {
                            if (simpleTokens[j + 1].Type != simpleToken.Type) {
                                break;
                            }
                            endToken = simpleTokens[j];
                        }

                        i = j;

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
            output.Add(new Token(trimToContent, TokenType.Identifier));
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
                if (AreaEmpty(new(x0 - 1, y0, x0, y1)) == false) {
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
    }
    
    public (Token[], Error[]) Tokenize(Grid<RawColor> image) {
        TokenizerInstance instance = new(image);
        return instance.Run();

    }
    
}