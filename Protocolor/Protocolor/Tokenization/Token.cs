using Protocolor.Util;

namespace Protocolor.Tokenization;

public enum TokenType {
    NumberLiteral,
    StringLiteral,
    Identifier,
    ConstDeclarationL,
    ConstDeclarationR, 
    VarDeclarationL,
    VarDeclarationR,
    Assignment,
    Add,
    Subtract,
    Multiply,
    ArrowL,
    ArrowR,
    NewLine,
    Pipe,
    BracketL,
    BracketR,
    EndOfFile,
}

public class Token {
    public readonly Rectangle Position;
    public readonly TokenType Type;

    public Token(Rectangle position, TokenType type) {
        this.Position = position;
        this.Type = type;
    }
}