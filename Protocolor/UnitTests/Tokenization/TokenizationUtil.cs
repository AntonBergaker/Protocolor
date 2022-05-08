using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Protocolor;
using Protocolor.Tokenization;
using Protocolor.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = Protocolor.Util.Rectangle;

namespace UnitTests.Tokenization;
static class TokenizationUtil {


    public static void AssertTokenizedImageEquals(string path, TokenType[] expectedTypes, [CallerFilePath] string callerPath = "") {
        if (expectedTypes.Last() != TokenType.NewLine) {
            expectedTypes = expectedTypes.Append(TokenType.NewLine).ToArray();
        }

        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);

        if (errors.Any(x => x.Code.Severity >= ErrorSeverity.Warning)) {
            Assert.Fail("Compilation failed with errors: " + string.Join("\n", errors.Select(x => x.ToString())));

        }

        if (expectedTypes.Length != tokens.Length) {
            Assert.Fail($"Token lengths are different. {expectedTypes.Length} != {tokens.Length}\n" +
                        $"Expected: {string.Join(", ", expectedTypes.Select(x => x.ToString()))}\n" +
                        $"Actual: {string.Join(", ", tokens.Select(x => x.Type.ToString()))}");
        }

        for (int i = 0; i < tokens.Length; i++) {
            Assert.AreEqual(expectedTypes[i], tokens[i].Type);
        }
    }

    public static void AssertImageErrors(string path, string errorCode, Rectangle? location = null, [CallerFilePath] string callerPath = "") {
        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);

        Error? error = errors.FirstOrDefault(x => x.Code.Identifier == errorCode);
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
