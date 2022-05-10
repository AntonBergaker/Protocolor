using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Protocolor.Tokenization;
using Protocolor.Util;

namespace UnitTests.Tokenization;
public class TestBlocksAndIndentation {

    [Test]
    public void SingleStatement() {
        TokenizationUtil.AssertTokenizedImageEquals("./single_statement.png", new ExpectedToken[] {
            TokenType.If, TokenType.Pipe, TokenType.Identifier, TokenType.Equals, TokenType.NumberLiteral, TokenType.Pipe, TokenType.NewLine,
            TokenType.StartBlock, TokenType.Identifier, TokenType.Assignment, TokenType.NumberLiteral, TokenType.EndBlock,
        });
    }

    [Test]
    public void NestedBlocks() {
        Rectangle firstRect = new(1, 6, 2, 20);
        Rectangle secondRect = new(3, 6, 4, 20);
        Rectangle thirdRect = new(5, 13, 6, 20);

        TokenizationUtil.AssertTokenizedImageEquals("./nested_blocks.png", new ExpectedToken[] {
            TokenType.Identifier, TokenType.Assignment, TokenType.Identifier, TokenType.NewLine,
            new (TokenType.StartBlock, firstRect), new (TokenType.StartBlock, secondRect),
            TokenType.Identifier, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            new (TokenType.StartBlock, thirdRect), 
            TokenType.Identifier, TokenType.Assignment, TokenType.Identifier,
            new (TokenType.EndBlock, thirdRect), new (TokenType.EndBlock, secondRect), new (TokenType.EndBlock, firstRect)
        });
    }
    
}
