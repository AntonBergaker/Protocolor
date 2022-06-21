using System;
using NUnit.Framework;
using Protocolor;
using Protocolor.Ast;
using Protocolor.Tokenization;
using Protocolor.Util;
using TT = Protocolor.Tokenization.TokenType;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Parsing;
class TestVariableDeclaration {

    [Test]
    public void SimpleDeclaration() {

        TestingUtil.AssertParsedTokensEqualsAst(
            new VariableDeclaration(
                TestingUtil.StringToBinaryFrame("word"), VariableKind.Const, null, null, Rectangle.Zero
            ),
            new ShorthandToken[] {
                ConstDeclarationL, ShorthandToken.Identifier("word"), ConstDeclarationR,
            }
        );

        TestingUtil.AssertParsedTokensEqualsAst(
            new VariableDeclaration(
                TestingUtil.StringToBinaryFrame("wurdle"), VariableKind.Var, null, null, Rectangle.Zero
            ),
            new ShorthandToken[] {
                VarDeclarationL, ShorthandToken.Identifier("wurdle"), VarDeclarationR,
            }
        );

        TestingUtil.AssertParsedTokensEqualsAst(
            new VariableDeclaration(
                TestingUtil.StringToBinaryFrame("wurdle"), VariableKind.Var, null, new StringLiteral("hello world", Rectangle.Zero), Rectangle.Zero
            ),
            new ShorthandToken[] {
                VarDeclarationL, ShorthandToken.Identifier("wurdle"), VarDeclarationR, TT.Assignment, "hello world"
            }
        );
    }

    [Test]
    public void DeclarationWithTypes() {
        var @var = TestingUtil.StringToBinaryFrame("theVar");
        var typeL = TestingUtil.StringToBinaryFrame("typeL");
        var typeR = TestingUtil.StringToBinaryFrame("typeR");

        {
            TestingUtil.AssertParsedTokensEqualsAst(
                new VariableDeclaration(
                    @var, VariableKind.Const,
                    new TypeReference(typeL, typeR, Array.Empty<IdentifierFrame>(), Rectangle.Zero),
                    new StringLiteral("mm yes", Rectangle.Zero), Rectangle.Zero
                ),
                new ShorthandToken[] {
                    typeL, ConstDeclarationL, @var, ConstDeclarationR, typeR, 
                    TT.Assignment, "mm yes"
                }
            );
        }

        // With a generic
        {
            var genericL = TestingUtil.StringToBinaryFrame("genericL");
            var genericR = TestingUtil.StringToBinaryFrame("genericR");

            TestingUtil.AssertParsedTokensEqualsAst(
                new VariableDeclaration(
                    @var, VariableKind.Const,
                    new TypeReference(typeL, typeR, new []{ genericL, genericR}, Rectangle.Zero),
                    new StringLiteral("mm no", Rectangle.Zero), Rectangle.Zero
                ),
                new ShorthandToken[] {
                    ShorthandToken.Identifier(typeL), genericL, genericR, ConstDeclarationL, ShorthandToken.Identifier(@var),
                    ConstDeclarationR, ShorthandToken.Identifier(typeR), TT.Assignment, "mm no"
                }
            );
        }
    }

}
