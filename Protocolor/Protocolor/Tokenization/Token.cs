using Protocolor.Util;

namespace Protocolor.Tokenization;

public enum TokenType {
    StringLiteral,
    NumberLiteral,
    Identifier,
    ConstDeclarationL,
    ConstDeclarationR,
    VarDeclarationL,
    VarDeclarationR,
    Assignment,
    Add,
    Subtract,
    Multiply,
    Divide,
    NewLine,
    Pipe,
    BracketL,
    BracketR,
    If,
    Equals,
    NotEquals,
    LesserThan,
    GreaterThan,
    LesserOrEqualThan,
    GreaterOrEqualThan,
    StartBlock,
    EndBlock,
    EndOfFile,
}

public class Token {
    public TokenType Type { get; }
    public Rectangle Position { get; }

    public Token(Rectangle position, TokenType type) {
        Type = type;
        this.Position = position;
    }
}

public class KeywordToken : Token {
    public KeywordToken(Rectangle position, TokenType type) : base(position, type) {
    }
}

public class NumberLiteralToken : Token {
    public NumberLiteralToken(Rectangle position) : base(position, TokenType.NumberLiteral) {

    }
}

public class StringLiteralToken : Token {
    public string Content { get; }
    public StringLiteralToken(Rectangle position, string content) : base(position, TokenType.StringLiteral) {
        Content = content;
    }
}

public class IdentifierToken : Token {
    public IdentifierFrame Frame { get; }
    
    public IdentifierToken(Rectangle position, IdentifierFrame frame) : base(position, TokenType.Identifier) {
        Frame = frame;
    }
}