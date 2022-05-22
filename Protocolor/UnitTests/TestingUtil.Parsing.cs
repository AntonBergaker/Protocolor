using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Protocolor;
using Protocolor.Ast;
using Protocolor.Parsing;
using Protocolor.Tokenization;

namespace UnitTests;

public static partial class TestingUtil {

    public static void AssertImageEqualsAst(Node expectedAst, string path,
        [CallerFilePath] string callerPath = "") {

        AssertTokensEqualsAst(expectedAst, TokenizeImage(path, callerPath).tokens);
    }

    public static void AssertParsedTokensEqualsAst(Node expectedAst, ShorthandToken[] tokens) {
        AssertTokensEqualsAst(expectedAst, tokens.Select(x => x.ToToken()).ToArray());
    }

    private static void AssertTokensEqualsAst(Node expectedAst, Token[] tokens) {
        var result = ParseTokens(tokens);

        if (result.errors.Length > 0) {
            Assert.Fail("Parsing failed with errors: " + string.Join("\n", result.errors.Select(x => x.ToString())));
        }

        bool equal = expectedAst.Equals(result.rootNode);
        if (equal == false) {
            Assert.Fail("Trees are not equal! Expected:\n" + expectedAst.ToString(BinaryFrameToString) + "\n\nGot:\n" + result.rootNode.ToString(BinaryFrameToString));
        }
    }

    private static (Node rootNode, Error[] errors) ParseTokens(Token[] tokens) {
        Parser parser = new Parser();

        var result = parser.Parse(tokens);

        return result;
    }
}
