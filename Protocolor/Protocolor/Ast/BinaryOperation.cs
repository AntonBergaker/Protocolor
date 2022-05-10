using Protocolor.Util;

namespace Protocolor.Ast;
public class BinaryOperation : Expression {
    public Expression Lhs { get; }
    public Expression Rhs { get; }
    public enum OperationType {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        ShiftLeft,
        ShiftRight,
    }

    public BinaryOperation(Expression lhs, Expression rhs, Rectangle position) : base(position) {
        Lhs = lhs;
        Rhs = rhs;
    }

}
