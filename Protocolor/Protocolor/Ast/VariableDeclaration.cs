using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;

public enum VariableKind {
    Const,
    Var,
}

public class VariableDeclaration : Statement {
    public IdentifierFrame VariableIdentifier { get; }
    public VariableKind Kind { get; }
    public Expression? Initializer { get; }
    public TypeReference? Type { get; }


    public VariableDeclaration(IdentifierFrame variableIdentifier, VariableKind kind, TypeReference? type, Expression? initializer, Rectangle position) : base(position) {
        VariableIdentifier = variableIdentifier;
        Kind = kind;
        Initializer = initializer;
        Type = type;
    }

    public override bool Equals(Node other) {
        if (other is not VariableDeclaration otherVD) {
            return false;
        }

        return otherVD.Kind == Kind &&
               VariableIdentifier.Equals(otherVD.VariableIdentifier) &&
               Equals(Initializer, otherVD.Initializer) &&
               Equals(Type, otherVD.Type);
    }


    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        StringBuilder sb = new StringBuilder();
        sb.Append(Kind == VariableKind.Const ? "const " : "var ");

        if (Type != null) {
            sb.Append(Type.ToStringStart(identifierFormatter) + " ");
        }

        sb.Append(identifierFormatter(VariableIdentifier));

        if (Type != null) {
            sb.Append(" " + Type.ToStringEnd(identifierFormatter));
        }

        if (Initializer != null) {
            sb.Append(" <- ");
            sb.Append(Initializer);
        }

        return sb.ToString();
    }
}
