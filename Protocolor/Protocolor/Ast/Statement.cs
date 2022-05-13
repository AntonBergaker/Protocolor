using Protocolor.Util;

namespace Protocolor.Ast;
public abstract class Statement : Node {
    protected Statement(Rectangle position) : base(position) { }
}
