using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class IdentifierChainExpression : Expression {

    public ImmutableArray<IdentifierExpression> Identifiers { get; }

    public IdentifierChainExpression(IEnumerable<IdentifierExpression> identifiers) :
        base(identifiers.Select(x => x.Position).UnionAll()) {
        Identifiers = identifiers.ToImmutableArray();
    }
    public override bool Equals(Node? other) {
        if (other is not IdentifierChainExpression otherIce) {
            return false;
        }

        return Enumerable.SequenceEqual(Identifiers, otherIce.Identifiers);
    }

    public override string ToString() {
        return ToString(DefaultIdentifierFormatter);
    }

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return $"({string.Join(" ", Identifiers.Select(x => x.ToString(identifierFormatter)))})";
    }
}
