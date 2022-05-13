using Protocolor.Ast;
using Protocolor.Tokenization;
using Protocolor.Util;

namespace Protocolor.Parsing;
public class Parser {

    private class ParserInstance {
        private readonly TokenReader tokenReader;
        private bool calledRan;

        public ParserInstance(Token[] tokens) {
            this.tokenReader = new(tokens);
        }

        public Node Run() {
            if (calledRan) {
                throw new Exception("Instance has already been run.");
            }

            calledRan = true;

            List<Statement> statements = new();
            while (tokenReader.HasNext) {
                statements.Add(ReadStatement());
            }

            if (statements.Count == 1) {
                return statements.First();
            }

            if (statements.Count == 0) {
                return new Block(statements, Rectangle.Zero);
            }

            return new Block(statements, statements.Select(x => x.Position).UnionAll());
        }


        private Statement ReadStatement() {
            return null!;
        }

        private Block ReadBlock(Token openingCharacter) {
            List<Statement> statements = new();
            while (tokenReader.HasNext) {
                statements.Add(ReadStatement());
            }
            


            return new Block(statements, statements.Select(x => x.Position).UnionAll());
        }

        private static bool BracketMatches(Token opening, Token closing) {
            if (opening.Type == TokenType.Pipe) {
                return closing.Type == TokenType.Pipe && 
                       opening.Position.Height == closing.Position.Height;
            }

            if (opening.Type == TokenType.BracketL) {
                return closing.Type == TokenType.BracketR;
            }

            throw new Exception("Opening bracket was not a valid type");
        }
    }


    public Node Parse(Token[] tokens) {
        ParserInstance instance = new ParserInstance(tokens);

        return instance.Run();
    }
}
