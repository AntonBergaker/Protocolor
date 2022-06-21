using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor.Util;

namespace Protocolor.Ast;
public class NumberLiteral : Expression {
    public string Content { get; }

    public NumberLiteral(string content, Rectangle rectangle) : base(rectangle) {
        Content = content;
    }

    public override bool Equals(Node? other) {
        if (other is not NumberLiteral nl) {
            return false;
        }

        return nl.Content == Content;
    }

    public override string ToString() => ToString(DefaultIdentifierFormatter);

    public override string ToString(IdentifierFormatter identifierFormatter) {
        return Content;
    }
}
