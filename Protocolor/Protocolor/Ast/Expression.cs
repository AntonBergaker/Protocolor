using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Expression : AstNode {
    protected Expression(Rectangle position) : base(position) { }
}
