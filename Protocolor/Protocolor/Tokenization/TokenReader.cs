using Protocolor.Util;

namespace Protocolor.Tokenization;
public class TokenReader {

    private readonly Token[] tokens;

    private int index;

    public TokenReader(Token[] tokens) {
        this.tokens = tokens;
        endOfFile = new Token(tokens.LastOrDefault()?.Position ?? new Rectangle(), TokenType.EndOfFile);
    }

    private readonly Token endOfFile;

    public bool HasNext => index < tokens.Length;

        public Token Peek() {
        int startIndex = index;

        Token token = Read();
        index = startIndex;
        return token;
    }

    public Token Read() {
        while (true) {
            if (index >= tokens.Length) {
                return endOfFile;
            }

            if (tokens[index].Type != TokenType.NewLine) {
                return tokens[index++];
            }

            index++;
        }
    }

    public void Skip() {
        Read();
    }

    public void SkipIncludeNewline() {
        ReadIncludeNewline();
    }

    public Token ReadIncludeNewline() {
        if (index >= tokens.Length) {
            return endOfFile;
        }

        return tokens[index++];
    }

    public Token PeekIncludeNewline() {
        if (index >= tokens.Length) {
            return endOfFile;
        }

        return tokens[index];
    }
}
