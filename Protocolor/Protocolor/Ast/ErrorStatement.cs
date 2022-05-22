using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;

/// <summary>
/// Used so we can return something from methods that fail.
/// </summary>
public class ErrorStatement : Statement {
    public ErrorStatement(Rectangle position) : base(position) { }

    public override bool Equals(Node other) {
        return other is ErrorStatement;
    }

    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return "ERROR!!!";
    }
}
