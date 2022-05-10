using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class AstNode {

    public Rectangle Position { get; }

    protected AstNode(Rectangle position) {
        Position = position;
    }

}
