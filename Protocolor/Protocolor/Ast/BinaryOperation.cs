using Protocolor.Tokenization;
using Protocolor.Util;

namespace Protocolor.Ast;
public class BinaryOperation : Expression {
    public Expression Lhs { get; }
    public Expression Rhs { get; }
    public OperationType Operation { get; }

    public enum OperationType {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        ShiftLeft,
        ShiftRight,
        BooleanAnd,
        BooleanXor,
        BooleanOr,
        BitwiseOr,
        BitwiseXor,
        BitwiseAnd
    }

    private static readonly TwoWayDictionary<TokenType, OperationType> TypeDictionary = new() {
        { TokenType.Add, OperationType.Add },
        { TokenType.Divide, OperationType.Divide },
        { TokenType.Multiply, OperationType.Multiply },
        { TokenType.Subtract, OperationType.Subtract },
        { TokenType.Modulo, OperationType.Modulo },
        { TokenType.ShiftLeft, OperationType.ShiftLeft },
        { TokenType.ShiftRight, OperationType.ShiftRight },
        { TokenType.BooleanAnd, OperationType.BooleanAnd },
        { TokenType.BooleanXor, OperationType.BooleanXor },
        { TokenType.BooleanOr, OperationType.BooleanOr },
        { TokenType.BitwiseOr, OperationType.BitwiseOr },
        { TokenType.BitwiseXor, OperationType.BitwiseXor },
        { TokenType.BitwiseAnd, OperationType.BitwiseAnd }
    };

    public static OperationType GetOperationFromToken(TokenType tokenType) {
        return TypeDictionary[tokenType];
    }


    public BinaryOperation(Expression lhs, OperationType operation, Expression rhs, Rectangle position) : base(position) {
        Lhs = lhs;
        Rhs = rhs;
        Operation = operation;
    }

    public override bool Equals(Node other) {
        if (other is not BinaryOperation bo) {
            return false;
        }

        return Operation == bo.Operation && Lhs.Equals(bo.Lhs) && Rhs.Equals(bo.Rhs);
    }

    public string GetOperationString(OperationType type) {
        return type switch {
            OperationType.Add => "+",
            OperationType.Subtract => "-",
            OperationType.Multiply => "*",
            OperationType.Divide => "/",
            OperationType.Modulo => "%",
            OperationType.ShiftLeft => "<<",
            OperationType.ShiftRight => ">>",
            OperationType.BooleanAnd => "&&",
            OperationType.BooleanXor => "^^",
            OperationType.BooleanOr => "||",
            OperationType.BitwiseOr => "|",
            OperationType.BitwiseXor => "^",
            OperationType.BitwiseAnd => "&",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return $"({Lhs.ToString(identifierFormatter)} {GetOperationString(Operation)} {Rhs.ToString(identifierFormatter)})";
    }
}
