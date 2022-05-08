using NUnit.Framework;
using Protocolor.Tokenization;

namespace UnitTests.Tokenization; 

class TestStatements {
    [Test]
    public void SimpleStatements() {

        TokenizationUtil.AssertTokenizedImageEquals("./statement.png", new [] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral
        });

        TokenizationUtil.AssertTokenizedImageEquals("./statement_multiline.png", new[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.Add, TokenType.NumberLiteral
        });        
    }

    [Test]
    public void TouchingEdges() {
        TokenizationUtil.AssertTokenizedImageEquals("./touching_edges.png", new[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.StringLiteral, TokenType.NewLine,
            TokenType.Identifier, TokenType.Pipe, TokenType.Pipe
        });
    }
    
}