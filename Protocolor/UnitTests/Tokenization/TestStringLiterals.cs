using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Protocolor.Tokenization;

namespace UnitTests.Tokenization;

class TestStringLiterals {


    [Test]
    public void ValidStringLiteral() {

    }

    [Test]
    public void EveryCharacter() {
        TestingUtil.AssertImageEqualsTokens("./every_character.png", new ExpectedToken[] {
            TokenType.Identifier, TokenType.Assignment, "abcdefghijklmnopqrstuvwxyz0123456789!#%'()*+,-./:;<=>?[]\\^_{|}~"
        });
    }

    [Test]
    public void ValidStringLiteralWithSpaces() {
        TestingUtil.AssertImageEqualsTokens("./valid_with_spaces.png", new ExpectedToken[] {
            TokenType.Identifier, TokenType.Assignment, "hello world", TokenType.NewLine,
            TokenType.Identifier, TokenType.Assignment, "hi universe",
        });
    }

    [Test]
    public void Capitalization() {
        TestingUtil.AssertImageEqualsTokens("./capitalization.png", new ExpectedToken[] {
            TokenType.Identifier, TokenType.Assignment, "SaRcaSM"
        });
    }

    [Test]
    public void CapitalizationForceUnambiguous() {
        TestingUtil.AssertImageEqualsTokens("./capitalization_force_unambiguous.png", new ExpectedToken[] {
            TokenType.Identifier, TokenType.Assignment, "'", TokenType.NewLine,
            TokenType.Identifier, TokenType.Assignment, ".", TokenType.NewLine,
            TokenType.Identifier, TokenType.Assignment, ",", TokenType.NewLine,
        });
    }

    [Test]
    public void IncompleteCapitalizationErrors() {
        TestingUtil.AssertTokenizedImageErrors("./incomplete_capitalization.png", Tokenizer.TokenizerErrors.StringLiteralMissingFullUnderline);
    }
}
