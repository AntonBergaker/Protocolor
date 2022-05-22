using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class IdentifierExpression : Expression {
    public IdentifierFrame Identifier { get; }
    public IdentifierExpression(IdentifierFrame identifier, Rectangle position) : base(position) {
        Identifier = identifier;
    }
    public override bool Equals(Node other) {
        if (other is not IdentifierExpression ie) {
            return false;
        }

        return Identifier.Equals(ie.Identifier);
    }

    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return identifierFormatter(Identifier);
    }
}
