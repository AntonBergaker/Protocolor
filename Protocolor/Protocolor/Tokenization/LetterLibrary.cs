using Protocolor.Util;

namespace Protocolor.Tokenization;
public static class LetterLibrary {
    private static readonly TwoWayDictionary<IdentifierFrame, char> letters;

    static LetterLibrary() {
        (char letter, string[] frameStrings)[] values = new[] {
            ('A', new [] {
                " X",
                "X X",
                "XXX",
                "X X",
            }),

            ('B', new [] {
                "XX ",
                "XXX",
                "X X",
                "XX ",
            }),

            ('C', new [] {
                " XX",
                "X  ",
                "X  ",
                " XX",
            }),

            ('D', new [] {
                "XX ",
                "X X",
                "X X",
                "XX ",
            }),

            ('E', new [] {
                "XXXX",
                "XXX ",
                "X   ",
                "XXXX",
            }),

            ('F', new [] {
                "XXXX",
                "X   ",
                "XXXX",
                "X   ",
            }),

            ('G', new [] {
                " XX",
                "X  ",
                "X X",
                " XX",
            }),

            ('H', new [] {
                "X X",
                "XXX",
                "X X",
                "X X",
            }),

            ('I', new [] {
                "XXX",
                " X ",
                " X ",
                "XXX",
            }),

            ('J', new [] {
                "XXX",
                "  X",
                "  X",
                "XX ",
            }),

            ('K', new [] {
                "X X",
                "XX ",
                "X X",
                "X X",
            }),

            ('L', new [] {
                "X  ",
                "X  ",
                "X  ",
                "XXX",
            }),

            ('M', new [] {
                "X   X",
                "XX XX",
                "X X X",
                "X   X",
            }),

            ('N', new [] {
                "X  X",
                "XX X",
                "X XX",
                "X  X",
            }),

            ('O', new [] {
                " X ",
                "X X",
                "X X",
                " X ",
            }),

            ('P', new [] {
                "XX ",
                "X X",
                "XX ",
                "X  ",
            }),

            ('Q', new [] {
                " X  ",
                "X X ",
                "X X ",
                " XXX",
            }),

            ('R', new [] {
                "XX ",
                "X X",
                "XX ",
                "X X",
            }),

            ('S', new [] {
                "XXX",
                "XX ",
                "  X",
                "XXX",
            }),

            ('T', new [] {
                "XXX",
                " X ",
                " X ",
                " X ",
            }),

            ('U', new [] {
                "X X",
                "X X",
                "X X",
                "XXX",
            }),

            ('V', new [] {
                "X X",
                "X X",
                "X X",
                " X ",
            }),

            ('W', new [] {
                "X X X",
                "X X X",
                "X X X",
                " X X ",
            }),

            ('X', new [] {
                "X X",
                " X ",
                " X ",
                "X X",
            }),

            ('Y', new [] {
                "X X",
                "X X",
                " X ",
                " X ",
            }),

            ('Z', new [] {
                "XXX",
                " XX",
                "X  ",
                "XXX",
            }),
        };

        letters = new();
        foreach (var value in values) {
            letters.Add(Utils.StringToFrame(value.frameStrings), value.letter);
        }
    }

    public static bool TryGetLetter(IdentifierFrame frame, out char letter) {
        return letters.TryGetValue(frame, out letter);
    }
}
