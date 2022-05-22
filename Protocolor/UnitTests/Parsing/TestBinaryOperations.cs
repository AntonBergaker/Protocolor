using NUnit.Framework;
using Protocolor.Ast;
using Protocolor.Util;
using TT = Protocolor.Tokenization.TokenType;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Parsing;
class TestBinaryOperations {

    [Test]
    public void Simple() {

        var @var = TestingUtil.StringToBinaryFrame("blergh");

        TestingUtil.AssertParsedTokensEqualsAst(
            new VariableDeclaration(
                @var, VariableKind.Const, null, 
                    new BinaryOperation(
                        new NumberLiteral("5", Rectangle.Zero),
                        BinaryOperation.OperationType.Add,
                        new NumberLiteral("10", Rectangle.Zero)
                    , Rectangle.Zero)
                , Rectangle.Zero
            ),
            new ShorthandToken[] {
                ConstDeclarationL, @var, ConstDeclarationR, TT.Assignment, 5, Add, 10,
            }
        );

    }

}
