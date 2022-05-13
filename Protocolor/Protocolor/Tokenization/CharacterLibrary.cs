using System.Collections.Immutable;
using Protocolor.Util;

namespace Protocolor.Tokenization;
public static class CharacterLibrary {
    private static readonly Dictionary<ShapeFrame, ImmutableArray<CharacterData>> characters;

    private record CharacterShapeData(char LowercaseLetter, char UppercaseLetter, int Offset, params string[] Shape);

    public record CharacterData(char LowercaseLetter, char UppercaseLetter, int Offset);

    static CharacterLibrary() {
        CharacterShapeData[] values = new CharacterShapeData[] {
            #region Alphabet
            new('a', 'A', 0,
                " X ",
                "X X",
                "XXX",
                "X X"
            ),

            new('b', 'B', 0,
                "XX ",
                "XXX",
                "X X",
                "XX "
            ),

            new('c', 'C', 0,
                " XX",
                "X  ",
                "X  ",
                " XX"
            ),

            new('d', 'D', 0,
                "XX ",
                "X X",
                "X X",
                "XX "
            ),

            new('e', 'E', 0,
                "XXX",
                "XX ",
                "X  ",
                "XXX"
            ),

            new('f', 'F', 0,
                "XXX",
                "X  ",
                "XXX",
                "X  "
            ),

            new('g', 'G', 0,
                " XX",
                "X  ",
                "X X",
                " XX"
            ),

            new('h', 'H', 0,
                "X X",
                "XXX",
                "X X",
                "X X"
            ),

            new('i', 'I', 0,
                "XXX",
                " X ",
                " X ",
                "XXX"
            ),

            new('j', 'J', 0,
                "  X",
                "  X",
                "X X",
                " X "
            ),

            new('k', 'K', 0,
                "X X",
                "XX ",
                "X X",
                "X X"
            ),

            new('l', 'L', 0,
                "X  ",
                "X  ",
                "X  ",
                "XXX"
            ),

            new('m', 'M', 0,
                "X   X",
                "XX XX",
                "X X X",
                "X   X"
            ),

            new('n', 'N', 0,
                "X  X",
                "XX X",
                "X XX",
                "X  X"
            ),

            new('o', 'O', 0,
                "XXX",
                "X X",
                "X X",
                "XXX"
            ),

            new('p', 'P', 0,
                "XX ",
                "X X",
                "XX ",
                "X  "
            ),

            new('q', 'Q', 0,
                " X  ",
                "X X ",
                "X X ",
                " XXX"
            ),

            new('r', 'R', 0,
                "XX ",
                "X X",
                "XX ",
                "X X"
            ),

            new('s', 'S', 0,
                "XXX",
                "XX ",
                "  X",
                "XXX"
            ),

            new('t', 'T', 0,
                "XXX",
                " X ",
                " X ",
                " X "
            ),

            new('u', 'U', 0,
                "X X",
                "X X",
                "X X",
                "XXX"
            ),

            new('v', 'V', 0,
                "X X",
                "X X",
                "X X",
                " X "
            ),

            new('w', 'W', 0,
                "X   X",
                "X X X",
                "X X X",
                " X X "
            ),

            new('x', 'X', 0,
                "X X",
                " X ",
                " X ",
                "X X"
            ),

            new('y', 'Y', 0,
                "X X",
                "X X",
                " X ",
                " X "
            ),

            new('z', 'Z', 0,
                "XXX",
                " XX",
                "X  ",
                "XXX"
            ),

            #endregion

            #region Numbers
            new('0', '\0', 0,
                " X ",
                "X X",
                "X X",
                " X "
            ),

            new('1', '\0', 0,
                " X ",
                "XX ",
                " X ",
                "XXX"
            ),

            new('2', '\0', 0,
                "XX ",
                "  X",
                " X ",
                "XXX"
            ),

            new('3', '\0', 0,
                "XX ",
                " XX",
                "  X",
                "XX "
            ),

            new('4', '\0', 0,
                "X X",
                "X X",
                "XXX",
                "  X"
            ),

            new('5', '\0', 0,
                "XXX",
                "XX ",
                "  X",
                "XX "
            ),

            new('6', '\0', 0,
                "X  ",
                "XXX",
                "X X",
                "XXX"
            ),

            new('7', '\0', 0,
                "XXX",
                "  X",
                " X ",
                " X "
            ),

            new('8', '\0', 0,
                " XX",
                "XXX",
                "X X",
                "XXX"
            ),

            new('9', '\0', 0,
                "XXX",
                "X X",
                "XXX",
                "  X"
            ),
            #endregion

            #region Symbols
            new('!', '\0', 0,
                "X",
                "X",
                " ",
                "X"
            ),

            new('#', '\0', 0,
                " X X",
                "XXXX",
                " X X",
                "XXXX"
            ),

            new('%', '\0', 0,
                "X X ",
                "  X ",
                " X  ",
                " X X"
            ),

            new('\'', '\0', 0,
                "X",
                "X"
            ),

            new('(', '\0', 0,
                " X",
                "X",
                "X",
                " X"
            ),

            new(')', '\0', 0,
                "X ",
                " X",
                " X",
                "X "
            ),

            new('*', '\0', 1,
                "X X",
                " X ",
                "X X"
            ),

            new('+', '\0', 1,
                " X ",
                "XXX",
                " X"
            ),

            new(',', '\0', 2,
                "X",
                "X"
            ),

            new('-', '\0', 2,
                "XXX"
            ),

            new('.', '\0', 3,
                "X"
            ),

            new('/', '\0', 0,
                " X",
                " X",
                "X ",
                "X "
            ),

            new(':', '\0', 1,
                "X",
                " ",
                "X"
            ),

            new(';', '\0', 1,
                " X",
                "  ",
                "XX"
            ),

            new('<', '\0', 1,
                " X",
                "X ",
                " X"
            ),

            new('=', '\0', 1,
                "XXX",
                "   ",
                "XXX"
            ),

            new('>', '\0', 1,
                "X ",
                " X",
                "X "
            ),

            new('?', '\0', 0,
                "XXX",
                " XX",
                "   ",
                " X "
            ),

            new('[', '\0', 0,
                "XX",
                "X ",
                "X ",
                "XX"
            ),

            new('\\', '\0', 0,
                "X ",
                "X ",
                " X",
                " X"
            ),

            new(']', '\0', 0,
                "XX",
                " X",
                " X",
                "XX"
            ),

            new('^', '\0', 0,
                " X ",
                "X X"
            ),

            new('_', '\0', 3,
                "XXX"
            ),

            new('{', '\0', 0,
                " XX",
                " X ",
                "XX ",
                " XX"
            ),

            new('|', '\0', 0,
                "X",
                "X",
                "X",
                "X"
            ),

            new('}', '\0', 0,
                "XX ",
                " X ",
                " XX",
                "XX "
            ),

            new('~', '\0', 1,
                " X X",
                "X X"
            ),

            #endregion
        };

        Dictionary<ShapeFrame, List<CharacterData>> mutableCharacters = new();
        foreach (var value in values) {
            var frame = Utils.StringToShape(value.Shape);

            if (mutableCharacters.TryGetValue(frame, out var list) == false) {
                list = new();
                mutableCharacters[frame] = list;
            }

            list.Add(new(value.LowercaseLetter, value.UppercaseLetter, value.Offset));
        }

        characters = mutableCharacters.ToDictionary(x => x.Key, x => ImmutableArray.CreateRange(x.Value));
    }

    public static ImmutableArray<CharacterData> GetCharactersForShape(ShapeFrame shape) {
        if (characters.TryGetValue(shape, out var array)) {
            return array;
        }

        return ImmutableArray<CharacterData>.Empty;
    }
}
