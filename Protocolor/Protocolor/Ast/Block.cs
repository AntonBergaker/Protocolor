using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class Block : Node {
    public ImmutableArray<Statement> Statements { get; }

    public Block(IEnumerable<Statement> statements, Rectangle position) : base(position) {
        Statements = statements.ToImmutableArray();
    }
}
