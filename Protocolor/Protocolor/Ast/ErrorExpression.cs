using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class ErrorExpression : Expression {
    public ErrorExpression(Rectangle position) : base(position) { }
    public override bool Equals(Node other) {
        return other is ErrorExpression;
    }

    public override string ToString() {
        return "ERROR!!!";
    }
}
