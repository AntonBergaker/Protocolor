using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Node : IEquatable<Node> {

    public Rectangle Position { get; }

    protected Node(Rectangle position) {
        Position = position;
    }

    public abstract bool Equals(Node other);

    public override bool Equals(object? obj) {
        if (obj is not Node other) {
            return false;
        }
        return Equals(other);
    }

    public abstract override string ToString();

}
