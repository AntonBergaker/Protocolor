using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using Protocolor;
using Protocolor.Tokenization;
using Protocolor.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = Protocolor.Util.Rectangle;

namespace UnitTests.Tokenization;
public static class TokenizationUtil {
    public static void AssertTokenizedImageEquals(string path, ExpectedToken[] expectedTypes, [CallerFilePath] string callerPath = "") {
        if (expectedTypes.Last().Type != TokenType.NewLine) {
            expectedTypes = expectedTypes.Append(TokenType.NewLine).ToArray();
        }

        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);

        if (errors.Any(x => x.Code.Severity >= ErrorSeverity.Warning)) {
            Assert.Fail("Compilation failed with errors: " + string.Join("\n", errors.Select(x => x.ToString())));

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
                    sb.Append("\n");
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
                    sb.Append("\n");
                }
            }


            Assert.Fail(sb.ToString());
        }

        for (int i = 0; i < tokens.Length; i++) {
            expectedTypes[i].AssertEquals(tokens[i]);
        }
    }

    public static void AssertImageErrors(string path, ErrorCode errorCode, Rectangle? location = null, [CallerFilePath] string callerPath = "") {
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
public class ExpectedToken {
    private readonly Action<Token>? compareFunction;
    public TokenType Type { get; }

    public ExpectedToken(TokenType type, Rectangle position) : this((other) => {
        AssertPosition(other, position);
    }, type) { }

    public ExpectedToken(IdentifierFrame frame, Rectangle position) : this((other) => {
        AssertPosition(other, position);
        AssertSameFrame(other, frame);
    }, TokenType.Identifier) { }

    private ExpectedToken(Action<Token>? compareFunction, TokenType type) {
        Type = type;
        this.compareFunction = compareFunction;
    }

    public static implicit operator ExpectedToken(TokenType type) {
        return new ExpectedToken(null, type);
    }

    public static implicit operator ExpectedToken(IdentifierFrame frame) {
        return new ExpectedToken((other) => {
            AssertSameFrame(other, frame);
        }, TokenType.Identifier);
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

    public void AssertEquals(Token token) {
        if (token.Type != Type) {
            Assert.Fail($"Token at position {token.Position} is not the expected type.\n" +
                        $"Expected: {this.Type}\n" +
                        $"Actual: {token.Type}");
        }

        this.compareFunction?.Invoke(token);
    }
}