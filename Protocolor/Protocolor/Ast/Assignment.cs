using Protocolor.Util;

namespace Protocolor.Ast;

public class Assignment : Statement {
    public IdentifierFrame Identifier { get; }
    public Expression Value { get; }

    public Assignment(IdentifierFrame identifier, Expression value, Rectangle position) : base(position) {
        Identifier = identifier;
        Value = value;
    }

    public override bool Equals(Node other) {
        if (other is not Assignment assignment) {
            return false;
        }

        return Identifier.Equals(assignment.Identifier) &&
               Value.Equals(assignment.Value);
    }

    public override string ToString() {
        return $"{Identifier} <- {Value}";
    }
}
