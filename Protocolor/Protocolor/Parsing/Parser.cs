using Protocolor.Ast;

namespace Protocolor.Parsing;
public class Parser {

    private class ParserInstance {
        private bool calledRan;

        public ParserInstance() {

        }

        public AstNode Run() {
            if (calledRan) {
                throw new Exception("Instance has already been run.");
            }                

            calledRan = true;
            
            return null!;
        }
    }
}
