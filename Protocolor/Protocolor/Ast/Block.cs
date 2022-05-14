using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class Block : Statement {
    public ImmutableArray<Statement> Statements { get; }

    public Block(IEnumerable<Statement> statements, Rectangle position) : base(position) {
        Statements = statements.ToImmutableArray();
    }

    public override bool Equals(Node other) {
        if (other is not Block block) {
            return false;
        }

        if (Statements.Length != block.Statements.Length) {
            return false;
        }

        for (int i = 0; i < Statements.Length; i++) {
            if (Statements[i].Equals(block.Statements[i]) == false) {
                return false;
            }
        }

        return true;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");

        foreach (Statement statement in Statements) {
            sb.AppendLine("\t" + statement.ToString().Replace("\n", "\n\t"));
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
