using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Protocolor.Ast;
using Protocolor.Util;
using TT = Protocolor.Tokenization.TokenType;

namespace UnitTests.Parsing;
internal class TestStatements {

    [Test]
    public void SimpleAssignment() {

        {
            var frame = TestingUtil.StringToBinaryFrame("var");

            TestingUtil.AssertParsedTokensEqualsAst(
                new Assignment(frame, new NumberLiteral("5", Rectangle.Zero), Rectangle.Zero),
                new ShorthandToken[] {
                    frame, TT.Assignment, 5
                }
            );
        }
    }

}
