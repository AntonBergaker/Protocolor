using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class TypeReference : Node {
    public IdentifierFrame OpeningIdentifier { get; }
    public IdentifierFrame ClosingIdentifier { get; }
    public ImmutableArray<IdentifierFrame> Generics { get; }

    public TypeReference(IdentifierFrame openingIdentifier, IdentifierFrame closingIdentifier, IEnumerable<IdentifierFrame> generics, Rectangle position) : base(position) {
        OpeningIdentifier = openingIdentifier;
        ClosingIdentifier = closingIdentifier;
        Generics = ImmutableArray.CreateRange(generics);
    }

    public override bool Equals(Node other) {
        if (other is not TypeReference otherTr) {
            return false;
        }

        return OpeningIdentifier.Equals(otherTr.OpeningIdentifier) && 
               ClosingIdentifier.Equals(otherTr.ClosingIdentifier) && 
               Enumerable.SequenceEqual(Generics, otherTr.Generics);
    }

    public string ToStringStart(IdentifierFormatter identifierFormatter) {
        if (Generics.Length == 0) {
            return identifierFormatter(OpeningIdentifier);
        }

        return $"{identifierFormatter(OpeningIdentifier)} <{string.Join(", ", Generics.Select(x => identifierFormatter(x)))}>";
    }

    public string ToStringEnd(IdentifierFormatter identifierFormatter) {
        return identifierFormatter(ClosingIdentifier);
    }

    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return ToStringStart(identifierFormatter) + " | " + ToStringEnd(identifierFormatter);
    }
}
