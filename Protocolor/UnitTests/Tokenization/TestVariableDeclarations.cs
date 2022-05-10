using NUnit.Framework;
using Protocolor.Tokenization;
using Protocolor.Util;

namespace UnitTests.Tokenization;
class TestVariableDeclarations {

    [Test]
    public void ValidDeclaration() {
        TokenizationUtil.AssertTokenizedImageEquals("./valid.png", new ExpectedToken[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.VarDeclarationL, TokenType.Identifier, TokenType.VarDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.Identifier, TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Identifier, TokenType.Assignment, TokenType.NumberLiteral,
        });
    }

    [Test]
    public void ValidDeclarationEdgecases() {
        TokenizationUtil.AssertTokenizedImageEquals("./valid_edgecases.png", new ExpectedToken[] {
            new(TokenType.VarDeclarationL, new(1, 1, 7, 4)), 
            new(Utils.StringToFrame(
                "pbbp",
                "CbbC"
                ), new (2, 1, 6, 3)), 
            
            new(TokenType.VarDeclarationR, new(1, 1, 7, 4)), 
            TokenType.Assignment, TokenType.StringLiteral, TokenType.NewLine,


            new(TokenType.ConstDeclarationL, new(0, 6, 9, 18)), 
            new(Utils.StringToFrame("g"), new (2, 11, 3, 12)), 
            new(TokenType.ConstDeclarationR, new(0, 6, 9, 18)), 
            
            TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
        });
    }

    [Test]
    public void ErrorUnclearMutability() {
        TokenizationUtil.AssertImageErrors("./error_unclear_mutability.png", Tokenizer.TokenizerErrors.DeclarationUnclearError);
    }

    [Test]
    public void ErrorUnevenSidesMutability() {
        TokenizationUtil.AssertImageErrors("./error_uneven_sides.png", Tokenizer.TokenizerErrors.DeclarationUnevenError);
    }
}
