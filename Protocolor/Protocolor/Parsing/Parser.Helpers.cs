using Protocolor.Ast;
using Protocolor.Tokenization;
using Protocolor.Util;

namespace Protocolor.Parsing; 

public partial class Parser {
    private partial class ParserInstance {

        private Expression GenericReadBinary(TokenType operation, Func<Expression> next) {
            Expression lhs = next();

            Token nextToken = reader.Peek();
            while (operation == nextToken.Type) {
                reader.Skip();
                Expression rhs = next();
                if (rhs is ErrorExpression) {
                    break;
                }
                lhs = new BinaryOperation(lhs, BinaryOperation.GetOperationFromToken(nextToken.Type), rhs, Rectangle.Union(lhs.Position, nextToken.Position, rhs.Position));
                nextToken = reader.Peek();
            }

            return lhs;
        }

        private Expression GenericReadBinary(TokenType[] operations, Func<Expression> next) {
            Expression lhs = next();

            Token nextToken = reader.Peek();
            while (operations.Contains(nextToken.Type)) {
                reader.Skip();
                Expression rhs = next();
                if (rhs is ErrorExpression) {
                    break;
                }
                lhs = new BinaryOperation(lhs, BinaryOperation.GetOperationFromToken(nextToken.Type), rhs, Rectangle.Union(lhs.Position, nextToken.Position, rhs.Position));
                nextToken = reader.Peek();
            }

            return lhs;
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

        private void AddError(ErrorCode code, Rectangle position, string? message = null) {
            errors.Add(new Error(code, position, message));
        }
    }
}