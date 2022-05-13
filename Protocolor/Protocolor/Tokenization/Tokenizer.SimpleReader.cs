using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolor.Tokenization; 

public partial class Tokenizer {
    private class SimpleReader {
        private readonly SimpleToken[] tokens;
        private int index = 0;

        private readonly SimpleToken endOfFile;

        private Stack<int> memoryStack = new Stack<int>();

        public SimpleReader(SimpleToken[] tokens) {
            this.tokens = tokens;
            endOfFile = new SimpleToken(this.tokens.LastOrDefault()?.Position ?? default, SimpleTokenType.EndOfFile);
        }

        public SimpleToken Read() {
            if (index >= tokens.Length) {
                return endOfFile;
            }

            return tokens[index++];
        }

        public SimpleToken Peek() {
            if (index >= tokens.Length) {
                return endOfFile;
            }

            return tokens[index];
        }

        public bool HasNext => index < tokens.Length;

        public void PushState() {
            memoryStack.Push(index);
        }

        public void PopState() {
            index = memoryStack.Pop();
        }

        public void Rewind() {
            index--;
            if (index < 0) {
                throw new Exception("Went outside the bounds of the reader");
            }
        }
    }
}