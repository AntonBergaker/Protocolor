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
}
