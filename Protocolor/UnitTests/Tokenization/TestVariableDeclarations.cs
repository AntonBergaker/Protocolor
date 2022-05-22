using NUnit.Framework;
using Protocolor.Tokenization;
using Protocolor.Util;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Tokenization;
class TestVariableDeclarations {

    [Test]
    public void ValidDeclaration() {
        TestingUtil.AssertImageEqualsTokens("./valid.png", new ShorthandToken[] {
            ConstDeclarationL, Identifier, ConstDeclarationR, Assignment, NumberLiteral, NewLine,
            VarDeclarationL, Identifier, VarDeclarationR, Assignment, NumberLiteral, NewLine,
            Identifier, ConstDeclarationL, Identifier, ConstDeclarationR, Identifier, Assignment, NumberLiteral,
        });
    }

    [Test]
    public void ValidDeclarationEdgecases() {
        TestingUtil.AssertImageEqualsTokens("./valid_edgecases.png", new ShorthandToken[] {
            new(VarDeclarationL, new(1, 1, 6, 3)), 
            new(Utils.StringToFrame(
                "pbbp",
                "CbbC"
                ), new (2, 1, 5, 2)), 
            
            new(VarDeclarationR, new(1, 1, 6, 3)), 
            Assignment, StringLiteral, NewLine,


            new(ConstDeclarationL, new(0, 6, 8, 17)), 
            new(Utils.StringToFrame("g"), new (2, 11, 2, 11)), 
            new(ConstDeclarationR, new(0, 6, 8, 17)), 
            
            Assignment, NumberLiteral, NewLine,
        });
    }

    [Test]
    public void ErrorUnclearMutability() {
        TestingUtil.AssertTokenizedImageErrors("./error_unclear_mutability.png", Tokenizer.TokenizerErrors.DeclarationUnclearError);
    }

    [Test]
    public void ErrorUnevenSidesMutability() {
        TestingUtil.AssertTokenizedImageErrors("./error_uneven_sides.png", Tokenizer.TokenizerErrors.DeclarationUnevenError);
    }
}
