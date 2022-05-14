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
    }

    public BinaryOperation(Expression lhs, Expression rhs, OperationType operation, Rectangle position) : base(position) {
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
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public override string ToString() {
        return $"({Lhs} {GetOperationString(Operation)} {Rhs})";
    }
}
