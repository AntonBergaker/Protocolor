using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Node : IEquatable<Node> {

    public delegate string IdentifierFormatter(IdentifierFrame frame);
    protected string DefaultIdentifierFormatter(IdentifierFrame frame) {
        return frame.ToString("|");
    }


    public Rectangle Position { get; }

    protected Node(Rectangle position) {
        Position = position;
    }

    public abstract bool Equals(Node? other);

    public override bool Equals(object? obj) {
        if (obj is not Node other) {
            return false;
        }
        return Equals(other);
    }

    public abstract override string ToString();

    public abstract string ToString(IdentifierFormatter identifierFormatter);
}
