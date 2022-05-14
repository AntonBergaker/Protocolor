using NUnit.Framework;
using Protocolor.Tokenization;

namespace UnitTests.Tokenization; 

class TestStatements {
    [Test]
    public void SimpleStatements() {

        TestingUtil.AssertImageEqualsTokens("./statement.png", new ExpectedToken[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral
        });

        TestingUtil.AssertImageEqualsTokens("./statement_multiline.png", new ExpectedToken[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.Add, TokenType.NumberLiteral
        });        
    }

    [Test]
    public void TouchingEdges() {
        TestingUtil.AssertImageEqualsTokens("./touching_edges.png", new ExpectedToken[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.StringLiteral, TokenType.NewLine,
            TokenType.Identifier, TokenType.Pipe, TokenType.Pipe
        });
    }
    
}