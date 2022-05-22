using NUnit.Framework;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Tokenization; 

class TestStatements {
    [Test]
    public void SimpleStatements() {

        TestingUtil.AssertImageEqualsTokens("./statement.png", new ShorthandToken[] {
            ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral
        });

        TestingUtil.AssertImageEqualsTokens("./statement_multiline.png", new ShorthandToken[] {
            ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral, NewLine,
            ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral, Add, NumberLiteral
        });        
    }

    [Test]
    public void TouchingEdges() {
        TestingUtil.AssertImageEqualsTokens("./touching_edges.png", new ShorthandToken[] {
            ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, StringLiteral, NewLine,
            Identifier, Pipe, Pipe
        });
    }
    
}