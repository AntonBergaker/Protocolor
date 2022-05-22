using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Protocolor;
using Protocolor.Tokenization;
using Protocolor.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = Protocolor.Util.Rectangle;

namespace UnitTests;
public static partial class TestingUtil {
    public static void AssertImageEqualsTokens(string path, ShorthandToken[] expectedTypes, [CallerFilePath] string callerPath = "") {
        if (expectedTypes.Last().Type != TokenType.NewLine) {
            expectedTypes = expectedTypes.Append(TokenType.NewLine).ToArray();
        }

        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);

        if (errors.Any(x => x.Code.Severity >= ErrorSeverity.Warning)) {
            Assert.Fail("Tokenization failed with errors: " + string.Join("\n", errors.Select(x => x.ToString())));
        }

        if (expectedTypes.Length != tokens.Length) {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Token lengths are different. {expectedTypes.Length} != {tokens.Length}");
            sb.AppendLine("Expected:");

            bool placeComma = false;
            foreach (var token in expectedTypes) {
                if (placeComma) {
                    sb.Append(", ");
                }
                placeComma = true;

                sb.Append(token.Type.ToString());

                if (token.Type == TokenType.NewLine) {
                    placeComma = false;
                    sb.Append('\n');
                }
            }

            sb.AppendLine("Actual:");
            placeComma = false;
            foreach (var token in tokens) {
                if (placeComma) {
                    sb.Append(", ");
                }
                placeComma = true;

                sb.Append(token.Type.ToString());

                if (token.Type == TokenType.NewLine) {
                    placeComma = false;
                    sb.Append('\n');
                }
            }


            Assert.Fail(sb.ToString());
        }

        for (int i = 0; i < tokens.Length; i++) {
            expectedTypes[i].AssertEquals(tokens[i]);
        }
    }

    public static void AssertTokenizedImageErrors(string path, ErrorCode errorCode, Rectangle? location = null, [CallerFilePath] string callerPath = "") {
        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);

        if (errors.Length == 0) {
            Assert.Fail("Compilation did not throw any errors.");
            return;
        }

        Error? error = errors.FirstOrDefault(x => x.Code.Identifier == errorCode.Identifier);
        if (error == null) {
            Assert.Fail("Compilation did not throw the provided error.");
            return;
        }

        if (location != null) {
            Assert.AreEqual(location.Value, error.Position);
        }

    }

    private static Grid<RawColor> ImportImage(string path, string callerPath) {
        callerPath = callerPath[..^3] + ".Source";
        using Image<Bgra32>? rawImage = Image.Load<Bgra32>(Path.Join(callerPath, path));
        Grid<RawColor> image = Utils.ImageToArray(rawImage);
        return image;
    }

    private static (Token[] tokens, Error[] erros) TokenizeImage(string path, string callerPath) {
        var image = ImportImage(path, callerPath);
        Tokenizer tokenizer = new Tokenizer();
        return tokenizer.Tokenize(image);
    }
}

/// <summary>
/// Asserts stuff
/// </summary>
public class ShorthandToken {
    private readonly Action<Token>? compareFunction;
    private readonly Func<Token> toTokenFunc;

    public TokenType Type {
        get;
    }

    public ShorthandToken(TokenType type, Rectangle position) : this((other) => {
        AssertPosition(other, position);
    }, () => new Token(position, type), type) {
    }

    public ShorthandToken(IdentifierFrame frame, Rectangle position) : this((other) => {
        AssertPosition(other, position);
        AssertSameFrame(other, frame);
    }, () => new IdentifierToken(frame, position)
    , TokenType.Identifier) {
    }

    private ShorthandToken(Action<Token>? compareFunction, Func<Token> toTokenFunc, TokenType type) {
        Type = type;
        this.compareFunction = compareFunction;
        this.toTokenFunc = toTokenFunc;
    }

    public static implicit operator ShorthandToken(TokenType type) {
        return new ShorthandToken(null, () => new Token(Rectangle.Zero, type), type);
    }

    public static implicit operator ShorthandToken(string @string) {
        return new ShorthandToken((other) => {
            AssertString(other, @string);
        }, () => new StringLiteralToken(@string, Rectangle.Zero), TokenType.StringLiteral);
    }

    public static implicit operator ShorthandToken(int @int) {
        return new ShorthandToken((other) => {
            AssertString(other, @int.ToString());
        }, () => new NumberLiteralToken(@int.ToString(), Rectangle.Zero), TokenType.NumberLiteral);
    }

    private static void AssertSameFrame(Token token, IdentifierFrame frame) {
        if (token is not IdentifierToken identifierToken) {
            Assert.Fail("Provided token was not an identifier");
            return;
        }
        Assert.AreEqual(identifierToken.Frame, frame);
    }

    private static void AssertPosition(Token token, Rectangle myPosition) {
        Assert.AreEqual(myPosition, token.Position);
    }

    private static void AssertString(Token token, string text) {
        if (token is not StringLiteralToken stringToken) {
            Assert.Fail("Provided token was not a string literal");
            return;
        }
        Assert.AreEqual(text, stringToken.Content);
    }

    private static void AssertNumber(Token token, string number) {
        if (token is not NumberLiteralToken numberLiteral) {
            Assert.Fail("Provided token was not a number literal");
            return;
        }
        Assert.AreEqual(number, numberLiteral.Content);
    }

    public void AssertEquals(Token token) {
        if (token.Type != Type) {
            Assert.Fail($"Token at position {token.Position} is not the expected type.\n" +
                        $"Expected: {this.Type}\n" +
                        $"Actual: {token.Type}");
        }

        this.compareFunction?.Invoke(token);
    }


    public Token ToToken() {
        return toTokenFunc();
    }

    public static implicit operator ShorthandToken(IdentifierFrame frame) {
        return Identifier(frame);
    }

    public static ShorthandToken Identifier(IdentifierFrame frame) {
        return new ShorthandToken((other) => {
            AssertSameFrame(other, frame);
        }, () => new IdentifierToken(frame, Rectangle.Zero), TokenType.Identifier);
    }

    public static ShorthandToken IdentifierShape(params string[] lines) {
        return Identifier(Utils.StringToFrame(lines));
    }

    public static ShorthandToken Identifier(string @string) {
        return Identifier(TestingUtil.StringToBinaryFrame(@string));
    }
}