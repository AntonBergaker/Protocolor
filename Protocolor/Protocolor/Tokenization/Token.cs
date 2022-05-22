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
    Modulo,
    ShiftLeft,
    ShiftRight,
    BooleanAnd,
    BooleanOr,
    BooleanXor,
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor,
    NewLine,
    Pipe,
    BracketL,
    BracketR,
    If,
    Equals,
    NotEquals,
    LessThan,
    GreaterThan,
    LessOrEqualThan,
    GreaterOrEqualThan,
    Dot,
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

        if (Type == TokenType.Identifier && this is not IdentifierToken) {
            throw new ArgumentException($"Token {Type} has a dedicated class, please use it instead.");
        }

        if (Type == TokenType.NumberLiteral && this is not NumberLiteralToken) {
            throw new ArgumentException($"Token {Type} has a dedicated class, please use it instead.");
        }

        if (Type == TokenType.StringLiteral && this is not StringLiteralToken) {
            throw new ArgumentException($"Token {Type} has a dedicated class, please use it instead.");
        }
    }
}

public class NumberLiteralToken : Token {
    public string Content { get; }
    public NumberLiteralToken(string content, Rectangle position) : base(position, TokenType.NumberLiteral) {
        Content = content;
    }
}

public class StringLiteralToken : Token {
    public string Content { get; }
    public StringLiteralToken(string content, Rectangle position) : base(position, TokenType.StringLiteral) {
        Content = content;
    }
}

public class IdentifierToken : Token {
    public IdentifierFrame Frame { get; }
    
    public IdentifierToken(IdentifierFrame frame, Rectangle position) : base(position, TokenType.Identifier) {
        Frame = frame;
    }
}