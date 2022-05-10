using Protocolor.Util;

namespace Protocolor.Tokenization;

public partial class Tokenizer {
    private static readonly RawColor WhiteSpace = PaletteColor.White.Color;
    private static readonly RawColor BlockLineColor = PaletteColor.LightGray.Color;
    private static readonly RawColor StringLiteralColor = PaletteColor.Red.Color;
    private static readonly RawColor OperatorColor = PaletteColor.Black.Color;
    private static readonly RawColor NumberLiteralColor = PaletteColor.Cyan.Color;

    private enum SimpleTokenType {
        Keyword,
        String,
        Number,
        Identifier,
        LineBreak,
        Indentation,
        EndOfFile
    }

    private class SimpleToken {
        public readonly Rectangle Position;
        public readonly SimpleTokenType Type;

        public SimpleToken(Rectangle position, SimpleTokenType type) {
            Position = position;
            Type = type;
        }

        public override string ToString() {
            return Type.ToString() + " " + Position.ToString();
        }
    }

    private class SimpleTokenizerInstance {
        private readonly Grid<RawColor> image;
        private readonly List<SimpleToken> output;
        private readonly List<Error> errors;

        private bool startedRunning;

        public SimpleTokenizerInstance(Grid<RawColor> image) {
            this.image = image;
            startedRunning = false;
            output = new();
            errors = new();
        }

        public (SimpleToken[], Error[]) Run() {
            if (startedRunning == true) {
                throw new InvalidOperationException("Instance has already been called once running");
            }
            startedRunning = true;

            int startLine = 0;
            for (int y = 0; y < image.Height; y++) {

                bool lineEmpty = true;
                
                // Ignore block lines, which are one wide gray lines.
                int blockLineWidth = 0;

                for (int x = 0; x < image.Width; x++) {
                    if (image[x, y] == WhiteSpace) {
                        blockLineWidth = 0;
                        continue;
                    }

                    if (image[x, y] == BlockLineColor) {
                        blockLineWidth++;

                        if (blockLineWidth <= 1) {
                            continue;
                        }
                    }

                    lineEmpty = false;
                    break;
                }

                // A full line of whitespace at the start, can be skipped
                if (lineEmpty && startLine == y) {
                    startLine = y + 1;
                    continue;
                }

                // If we have an empty line with data since last, parse it
                if (lineEmpty) {
                    ParseLine(startLine, y-1);
                    startLine = y + 1;
                    continue;
                }
            }

            if (startLine < image.Height) {
                ParseLine(startLine, image.Height-1);
            }

            return (output.ToArray(), errors.ToArray());
        }

        private void ParseLine(int startY, int endY) {
            int startColumn = 0;

            bool hasPassedIndentation = false;

            for (int x = 0; x < image.Width; x++) {
                bool columnEmpty = true;

                for (int y = startY; y <= endY; y++) {
                    if (image[x, y] != WhiteSpace) {
                        columnEmpty = false;
                        break;
                    }
                }

                // A full column of whitespace at the start, can be skipped
                if (columnEmpty && startColumn == x) {
                    startColumn = x + 1;
                    continue;
                }

                // If we have an empty line, send it in for parsing
                if (columnEmpty) {
                    Rectangle blockPosition = new(startColumn, startY, x-1, endY);

                    // If it's one wide, all gray and at the start, its probably an indentation
                    if (blockPosition.Width == 1 && AreaOneColorOrWhitespace(blockPosition, BlockLineColor) && hasPassedIndentation == false) {
                        if (AreaOneColor(blockPosition, BlockLineColor) == false) {
                            errors.Add(new Error(TokenizerErrors.InvalidBlockShape, blockPosition));
                        }

                        output.Add(new SimpleToken(blockPosition, SimpleTokenType.Indentation));
                        startColumn = x + 1;
                        continue;
                    }

                    hasPassedIndentation = true;
                    ParseBlock(blockPosition);
                    startColumn = x + 1;
                    continue;
                }
            }

            if (startColumn < image.Width) {
                ParseBlock(new(startColumn, startY, image.Width-1, endY));
            }

            // Add a newline token at the end of every line
            output.Add(new SimpleToken(new Rectangle(image.Width-1, startY, image.Width-1, endY), SimpleTokenType.LineBreak));
        }

        private void ParseBlock(Rectangle position) {
            var (x0, y0, x1, y1) = position;

            // X and Y are already guaranteed to be trimmed, but Y can maybe still be trimmed
            for (int y = y0; y <= y1; y++) {
                bool lineEmpty = AreaEmpty(new(x0, y, x1, y));
                // Line is empty, increase startY
                if (lineEmpty) {
                    y0 = y + 1;
                } else {
                    break;
                }
            }

            for (int y = y1; y >= y0; y--) {
                bool lineEmpty = AreaEmpty(new(x0, y, x1, y));
                // Line is empty, decrease endY
                if (lineEmpty) {
                    y1 = y - 1;
                } else {
                    break;
                }
            }

            bool isOperator = true;
            bool isStringLiteral = true;
            bool isNumberLiteral = true;

            for (int x = x0; x <= x1; x++) {
                for (int y = y0; y <= y1; y++) {
                    RawColor color = image[x, y];
                    if (color == WhiteSpace) {
                        continue;
                    }

                    if (color != OperatorColor) {
                        isOperator = false;
                    }
                    if (color != StringLiteralColor) {
                        isStringLiteral = false;
                    }
                    if (color != NumberLiteralColor) {
                        isNumberLiteral = false;
                    }
                }
            }

            SimpleTokenType type = SimpleTokenType.Identifier;
            if (isOperator) {
                type = SimpleTokenType.Keyword;
            } else if (isNumberLiteral) {
                type = SimpleTokenType.Number;
            } else if (isStringLiteral) {
                type = SimpleTokenType.String;
            }

            output.Add(new SimpleToken(new Rectangle(x0, y0, x1, y1), type));
        }

        private bool AreaEmpty(Rectangle position) {
            return AreaOneColor(position, WhiteSpace);
        }

        private bool AreaOneColor(Rectangle area, RawColor color) {
            for (int x = area.X0; x <= area.X1; x++) {
                for (int y = area.Y0; y <= area.Y1; y++) {
                    if (image[x, y] != color) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AreaOneColorOrWhitespace(Rectangle area, RawColor color) {
            for (int x = area.X0; x <= area.X1; x++) {
                for (int y = area.Y0; y <= area.Y1; y++) {
                    if (image[x, y] != color && image[x, y] != WhiteSpace) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}