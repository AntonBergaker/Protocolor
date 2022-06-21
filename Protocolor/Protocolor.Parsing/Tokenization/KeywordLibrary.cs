using Protocolor.Util;

namespace Protocolor.Tokenization;
public static class KeywordLibrary {
    private static readonly TwoWayDictionary<ShapeFrame, TokenType> keywords;

    static KeywordLibrary() {
        (TokenType type, string[] frameStrings)[] values =  new [] {
            (TokenType.If, new [] {
                "XXX",
                " XX",
                "   ",
                " X ",
            }),

            (TokenType.Assignment, new[] {
                " X  ",
                "XXXX",
                " X  ",
            }),

            (TokenType.Add, new[] {
                " X ",
                "XXX",
                " X ",
            }),

            (TokenType.Subtract, new[] {
                "XXX",
            }),

            (TokenType.Multiply, new[] {
                "X X",
                " X ",
                "X X",
            }),

            (TokenType.Divide, new[] {
                " X",
                " X",
                "X ",
                "X ",
            }),

            (TokenType.Modulo, new[] {
                "X X ",
                "  X ",
                " X  ",
                " X X",
            }),

            (TokenType.BooleanAnd, new[] {
                " X ",
                "X X",
                "X X",
            }),

            (TokenType.BooleanOr, new[] {
                "X X",
                "X X",
                " X ",
            }),

            (TokenType.BooleanXor, new[] {
                "X X",
                " X ",
                "XXX",
            }),

            (TokenType.Equals, new [] {
                "XXX",
                "   ",
                "XXX",
            }),

            (TokenType.NotEquals, new [] {
                "X  ",
                "XXX",
                " X ",
                "XXX",
                "  X",
            }),

            (TokenType.LessThan, new [] {
                " X",
                "X ",
                " X",
            }),

            (TokenType.GreaterThan, new [] {
                "X ",
                " X",
                "X ",
            }),

            (TokenType.LessOrEqualThan, new [] {
                " X ",
                "X  ",
                " X ",
                "XXX",
            }),

            (TokenType.GreaterOrEqualThan, new [] {
                " X ",
                "  X",
                " X ",
                "XXX"
            }),

            (TokenType.Loop, new [] {
                " XXX X ",
                " X  XXX",
                "XXX  X ",
                " X XXX ",
            }),
        };

        keywords = new();
        foreach (var value in values) {
            keywords.Add(Utils.StringToShape(value.frameStrings), value.type);
        }
    }

    public static bool TryGetOperator(ShapeFrame frame, out TokenType type) {
        return keywords.TryGetValue(frame, out type);
    }
}
