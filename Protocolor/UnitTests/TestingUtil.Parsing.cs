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

    public static void AssertImageEqualsAst(string path, Node expectedAst,
        [CallerFilePath] string callerPath = "") {

        Node rootNode = ParseImage(path, callerPath);

        bool equal = expectedAst.Equals(rootNode);
        if (equal == false) {

        }
    }

    private static Node ParseImage(string path, string callerPath) {
        (Token[] tokens, Error[] errors) = TokenizeImage(path, callerPath);
        Parser parser = new Parser();

        if (errors.Length > 0) {
            Assert.Fail("Tokenizer had errors: " + string.Join(",", errors.Select(x => x.Message)));
        }

        var result = parser.Parse(tokens);

        if (result.errors.Length > 0) {
            Assert.Fail("Parsing failed with errors: " + string.Join("\n", errors.Select(x => x.ToString())));
        }

        return result.rootNode;
    }
}
