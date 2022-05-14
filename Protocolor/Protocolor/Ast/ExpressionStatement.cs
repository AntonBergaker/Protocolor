using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolor.Ast;
public class ExpressionStatement : Statement {
    public Expression Expression { get; }

    public ExpressionStatement(Expression expression) : base(expression.Position) {
        Expression = expression;
    }

    public override bool Equals(Node other) {
        if (other is not ExpressionStatement es) {
            return false;
        }

        return es.Expression.Equals(Expression);
    }

    public override string ToString() {
        return Expression.ToString();
    }
}
