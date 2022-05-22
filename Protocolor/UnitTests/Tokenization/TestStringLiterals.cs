using NUnit.Framework;
using Protocolor.Tokenization;
using static Protocolor.Tokenization.TokenType;

namespace UnitTests.Tokenization;

class TestStringLiterals {


    [Test]
    public void ValidStringLiteral() {

    }

    [Test]
    public void EveryCharacter() {
        TestingUtil.AssertImageEqualsTokens("./every_character.png", new ShorthandToken[] {
            Identifier, Assignment, "abcdefghijklmnopqrstuvwxyz0123456789!#%'()*+,-./:;<=>?[]\\^_{|}~"
        });
    }

    [Test]
    public void ValidStringLiteralWithSpaces() {
        TestingUtil.AssertImageEqualsTokens("./valid_with_spaces.png", new ShorthandToken[] {
            Identifier, Assignment, "hello world", NewLine,
            Identifier, Assignment, "hi universe",
        });
    }

    [Test]
    public void Capitalization() {
        TestingUtil.AssertImageEqualsTokens("./capitalization.png", new ShorthandToken[] {
            Identifier, Assignment, "SaRcaSM"
        });
    }

    [Test]
    public void CapitalizationForceUnambiguous() {
        TestingUtil.AssertImageEqualsTokens("./capitalization_force_unambiguous.png", new ShorthandToken[] {
            Identifier, Assignment, "'", NewLine,
            Identifier, Assignment, ".", NewLine,
            Identifier, Assignment, ",", NewLine,
        });
    }

    [Test]
    public void IncompleteCapitalizationErrors() {
        TestingUtil.AssertTokenizedImageErrors("./incomplete_capitalization.png", Tokenizer.TokenizerErrors.StringLiteralMissingFullUnderline);
    }
}
