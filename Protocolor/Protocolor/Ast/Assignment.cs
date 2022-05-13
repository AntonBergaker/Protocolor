using Protocolor.Util;

namespace Protocolor.Ast;

public class Assignment : Statement {
    public IdentifierFrame Identifier { get; }
    public Expression Value { get; }

    public Assignment(IdentifierFrame identifier, Expression value, Rectangle position) : base(position) {
        Identifier = identifier;
        Value = value;
    }

}
