using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Node {

    public Rectangle Position { get; }

    protected Node(Rectangle position) {
        Position = position;
    }

}
