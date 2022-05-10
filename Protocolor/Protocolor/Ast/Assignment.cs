using Protocolor.Util;

namespace Protocolor.Ast;

public class Assignment : Statement {
    public IdentifierFrame Identifier { get; }

    public Assignment(IdentifierFrame identifier, Rectangle position) : base(position) {
        Identifier = identifier;
    }

}
