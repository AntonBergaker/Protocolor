using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Expression : Node {
    protected Expression(Rectangle position) : base(position) { }
}
