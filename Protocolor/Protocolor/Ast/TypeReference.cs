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

    public string ToStringStart() {
        if (Generics.Length == 0) {
            return OpeningIdentifier.ToString();
        }

        return $"{OpeningIdentifier} <{string.Join(", ", Generics)}>";
    }

    public string ToStringEnd() {
        return ClosingIdentifier.ToString();
    }

    public override string ToString() {
        return ToStringStart() + " | " + ToStringEnd();
    }
}
