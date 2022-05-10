using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Statement : AstNode {
    protected Statement(Rectangle position) : base(position) { }
}
