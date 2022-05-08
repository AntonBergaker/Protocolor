using NUnit.Framework;
using Protocolor.Tokenization;

namespace UnitTests.Tokenization;
class TestVariableDeclarations {

    [Test]
    public void ValidDeclaration() {
        TokenizationUtil.AssertTokenizedImageEquals("./valid.png", new[] {
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.VarDeclarationL, TokenType.Identifier, TokenType.VarDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
            TokenType.Identifier, TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Identifier, TokenType.Assignment, TokenType.NumberLiteral,
        });
    }

    [Test]
    public void ValidDeclarationEdgecases() {
        TokenizationUtil.AssertTokenizedImageEquals("./valid_edgecases.png", new[] {
            TokenType.VarDeclarationL, TokenType.Identifier, TokenType.VarDeclarationR, TokenType.Assignment, TokenType.StringLiteral, TokenType.NewLine,
            TokenType.ConstDeclarationL, TokenType.Identifier, TokenType.ConstDeclarationR, TokenType.Assignment, TokenType.NumberLiteral, TokenType.NewLine,
        });
    }

    [Test]
    public void ErrorUnclearMutability() {
        TokenizationUtil.AssertImageErrors("./error_unclear_mutability.png", Tokenizer.TokenizerErrors.DeclarationUnclearError.Identifier);
    }

    [Test]
    public void ErrorUnevenSidesMutability() {
        TokenizationUtil.AssertImageErrors("./error_uneven_sides.png", Tokenizer.TokenizerErrors.DeclarationUnevenError.Identifier);
    }
}
