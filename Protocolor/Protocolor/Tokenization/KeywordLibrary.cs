using Protocolor.Util;

namespace Protocolor.Tokenization;
public static class KeywordLibrary {
    private static readonly TwoWayDictionary<IdentifierFrame, TokenType> keywords;

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

            (TokenType.LesserThan, new [] {
                " X",
                "X ",
                " X",
            }),

            (TokenType.GreaterThan, new [] {
                "X ",
                " X",
                "X ",
            }),

            (TokenType.LesserOrEqualThan, new [] {
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
        };

        keywords = new();
        foreach (var value in values) {
            keywords.Add(Utils.StringToFrame(value.frameStrings), value.type);
        }
    }

    public static bool TryGetOperator(IdentifierFrame frame, out TokenType type) {
        return keywords.TryGetValue(frame, out type);
    }
}
