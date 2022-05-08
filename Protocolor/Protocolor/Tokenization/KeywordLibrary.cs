using Protocolor.Util;

namespace Protocolor.Tokenization;
public static class KeywordLibrary {
    private static readonly TwoWayDictionary<IdentifierFrame, TokenType> keywords;

    static KeywordLibrary() {
        (TokenType type, string[] frameStrings)[] values =  new [] {
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
