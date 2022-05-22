using NUnit.Framework;
using Protocolor.Tokenization;
using Protocolor.Util;
using TT = Protocolor.Tokenization.TokenType;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Tokenization;
public class TestBlocksAndIndentation {

    [Test]
    public void SingleStatement() {
        TestingUtil.AssertImageEqualsTokens("./single_statement.png", new ShorthandToken[] {
            If, Pipe, Identifier, TT.Equals, NumberLiteral, Pipe, NewLine,
            StartBlock, Identifier, Assignment, NumberLiteral, EndBlock,
        });
    }

    [Test]
    public void NestedBlocks() {
        Rectangle firstRect = new(1, 6, 1, 19);
        Rectangle secondRect = new(3, 6, 3, 19);
        Rectangle thirdRect = new(5, 13, 5, 19);

        TestingUtil.AssertImageEqualsTokens("./nested_blocks.png", new ShorthandToken[] {
            Identifier, Assignment, Identifier, NewLine,
            new (StartBlock, firstRect), new (StartBlock, secondRect),
            Identifier, Assignment, NumberLiteral, NewLine,
            new (StartBlock, thirdRect), 
            Identifier, Assignment, Identifier,
            new (EndBlock, thirdRect), new (EndBlock, secondRect), new (EndBlock, firstRect)
        });
    }

    [Test]
    public void ErrorGappedBlock() {
        TestingUtil.AssertTokenizedImageErrors("./gapped_indent.png", Tokenizer.TokenizerErrors.BlockShapeInvalid);
    }

    [Test]
    public void ErrorUnalignedBlock() {
        TestingUtil.AssertTokenizedImageErrors("./unaligned_block.png", Tokenizer.TokenizerErrors.BlockShapeInvalid);
    }

    [Test]
    public void BlocksWithoutSeparator() {
        TestingUtil.AssertImageEqualsTokens("./blocks_without_separator.png", new ShorthandToken[] {
            If, Pipe, Identifier, TT.Equals, NumberLiteral, Pipe, NewLine,
            StartBlock, ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral, EndBlock, NewLine,
            StartBlock, ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral, EndBlock,
        });
    }

    [Test]
    public void SplitOnWrongBlock() {
        TestingUtil.AssertTokenizedImageErrors("./split_on_outer_block.png", Tokenizer.TokenizerErrors.BlockShapeInvalid);
    }

    [Test]
    public void StartWithBlock() {
        TestingUtil.AssertImageEqualsTokens("./start_with_block.png", new ShorthandToken[] {
            StartBlock, Identifier, Assignment, Identifier, EndBlock,
        });
    }

}
