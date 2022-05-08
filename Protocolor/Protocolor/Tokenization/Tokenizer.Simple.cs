using Protocolor.Util;

namespace Protocolor.Tokenization;

public partial class Tokenizer {
    private static readonly RawColor WhiteSpace = PaletteColor.White.Color;
    private static readonly RawColor StringLiteralColor = PaletteColor.Red.Color;
    private static readonly RawColor OperatorColor = PaletteColor.Black.Color;
    private static readonly RawColor NumberLiteralColor = PaletteColor.Cyan.Color;

    private enum SimpleTokenType {
        Keyword,
        String,
        Number,
        Identifier,
        LineBreak,
        EndOfFile
    }

    private class SimpleToken {
        public readonly Rectangle Position;
        public readonly SimpleTokenType Type;

        public SimpleToken(Rectangle position, SimpleTokenType type) {
            Position = position;
            Type = type;
        }
    }

    private class SimpleTokenizerInstance {
        private readonly Grid<RawColor> image;
        private readonly List<SimpleToken> output;

        private bool startedRunning;

        public SimpleTokenizerInstance(Grid<RawColor> image) {
            this.image = image;
            startedRunning = false;
            output = new();
        }

        public SimpleToken[] Run() {
            if (startedRunning == true) {
                throw new InvalidOperationException("Instance has already been called once running");
            }
            startedRunning = true;

            int startLine = 0;
            for (int y = 0; y < image.Height; y++) {

                bool lineEmpty = LineEmpty(new(0, y), image.Width);

                // A full line of whitespace at the start, can be skipped
                if (lineEmpty && startLine == y) {
                    startLine = y + 1;
                    continue;
                }

                // If we have an empty line with data since last, parse it
                if (lineEmpty) {
                    ParseLine(startLine, y);
                    startLine = y + 1;
                    continue;
                }
            }

            if (startLine < image.Height) {
                ParseLine(startLine, image.Height);
            }

            return output.ToArray();
        }

        private void ParseLine(int startY, int endY) {
            int startColumn = 0;

            for (int x = 0; x < image.Width; x++) {
                bool columnEmpty = true;

                for (int y = startY; y < endY; y++) {
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
                    ParseBlock(new(startColumn, startY), new (x, endY));
                    startColumn = x + 1;
                    continue;
                }
            }

            if (startColumn < image.Width) {
                ParseBlock(new(startColumn, startY), new(image.Width, endY));
            }

            // Add a newline token at the end of every line
            output.Add(new SimpleToken(new Rectangle(image.Width-1, startY, image.Width, endY), SimpleTokenType.LineBreak));
        }

        private void ParseBlock(Point start, Point end) {
            int startY = start.Y;
            int endY = end.Y;

            // X and Y are already guaranteed to be trimmed, but Y can maybe still be trimmed
            for (int y = startY; y < endY; y++) {
                bool lineEmpty = LineEmpty(new(start.X, y), end.X);
                // Line is empty, increase startY
                if (lineEmpty) {
                    startY = y + 1;
                } else {
                    break;
                }
            }

            for (int y = endY - 1; y > startY; y--) {
                bool lineEmpty = LineEmpty(new(start.X, y), end.X);
                // Line is empty, decrease endY
                if (lineEmpty) {
                    endY = y;
                } else {
                    break;
                }
            }

            bool isOperator = true;
            bool isStringLiteral = true;
            bool isNumberLiteral = true;

            for (int x = start.X; x < end.X; x++) {
                for (int y = startY; y < endY; y++) {
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

            output.Add(new SimpleToken(new Rectangle(start.X, startY, end.X, endY), type));
        }

        private bool LineEmpty(Point start, int endX) {
            bool lineEmpty = true;

            for (int x = start.X; x < endX; x++) {
                if (image[x, start.Y] != WhiteSpace) {
                    lineEmpty = false;
                    break;
                }
            }

            return lineEmpty;
        }
    }
}