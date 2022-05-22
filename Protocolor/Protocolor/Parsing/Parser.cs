using Protocolor.Ast;
using Protocolor.Tokenization;
using Protocolor.Util;

namespace Protocolor.Parsing;
public partial class Parser {
    public static class ParserErrors {
        public static ErrorCode UnexpectedToken =
            new("parser_unexpected_token", ErrorSeverity.Fatal, "Unexpected token. The parser expected a different token here.");

        public static ErrorCode VariableDeclarationMismatchedTypes = 
            new("parser_variable_mismatched_types", ErrorSeverity.Error, "Variable declaration had different reassignability tokens on each side.");

        public static ErrorCode VariableWrongIdentifierType = 
            new("parser_variable_wrong_identifier_type", ErrorSeverity.Fatal, "Variable had the wrong token type. Make sure it's a valid identifier with a valid set of colors.");
    }

    private partial class ParserInstance {
        private readonly TokenReader reader;
        private bool calledRan;
        private List<Error> errors;

        public ParserInstance(Token[] tokens) {
            this.reader = new(tokens);
            errors = new List<Error>();
        }

        public (Node rootNode, Error[] errors) Run() {
            if (calledRan) {
                throw new Exception("Instance has already been run.");
            }

            calledRan = true;

            List<Statement> statements = new();
            while (reader.HasNext) {
                statements.Add(ReadStatement());
            }

            if (statements.Count == 1) {
                return (statements.First(), errors.ToArray());
            }

            if (statements.Count == 0) {
                return (new Block(statements, Rectangle.Zero), errors.ToArray());
            }

            return (new Block(statements, statements.Select(x => x.Position).UnionAll()), errors.ToArray());
        }


        private Statement ReadStatement() {
            Token token = reader.Peek();

            if (token.Type == TokenType.StartBlock) {
                return ReadBlock(reader.Read());
            }
            
            
            // Read the stuff that begins with a list of identifiers
            List<IdentifierToken> identifiers = new();
            while (token.Type == TokenType.Identifier) {
                identifiers.Add((IdentifierToken)reader.Read());
                token = reader.Peek();
            }

            if (token.Type is TokenType.ConstDeclarationL or TokenType.VarDeclarationL) {
                return ReadVariableDeclaration(reader.Read(), identifiers);
            }

            if (token.Type is TokenType.Assignment) {
                return ReadAssignment(reader.Read(), identifiers);
            }

            Expression expression = ReadExpression();
            // TODO: Check if it's a valid expression type
            return new ExpressionStatement(expression);
        }

        private Block ReadBlock(Token openingCharacter) {
            List<Statement> statements = new();
            while (reader.HasNext) {
                if (BracketMatches(openingCharacter, reader.Peek())) {
                    reader.Read();
                    break;
                }

                statements.Add(ReadStatement());
            }
            


            return new Block(statements, statements.Select(x => x.Position).UnionAll());
        }

        private Statement ReadVariableDeclaration(Token openDeclaration, List<IdentifierToken> identifierTokens) {
            bool isConst = openDeclaration.Type == TokenType.ConstDeclarationL;

            Rectangle position = openDeclaration.Position;

            Token maybeIdentifier = reader.Read();
            position = Rectangle.Union(position, maybeIdentifier.Position);

            if (maybeIdentifier is not IdentifierToken identifier) {
                string? customMessage = null;
                if (maybeIdentifier.Type == TokenType.StringLiteral) {
                    customMessage = "Variable must be a identifier type. All red is reserved for string literals";
                }
                if (maybeIdentifier.Type == TokenType.NumberLiteral) {
                    customMessage = "Variable must be a identifier type. All blue is reserved for number literals";
                }

                AddError(ParserErrors.VariableWrongIdentifierType, maybeIdentifier.Position, customMessage);
                return new ErrorStatement(position);
            }

            Token closeDeclaration = reader.Read();
            position = Rectangle.Union(position, closeDeclaration.Position);

            if (closeDeclaration.Type is not (TokenType.ConstDeclarationR or TokenType.VarDeclarationR)) {
                AddError(ParserErrors.UnexpectedToken, position, $"Unexpected token {closeDeclaration}. Expected a variable closer.");
            }

            if (
                (openDeclaration.Type == TokenType.ConstDeclarationL && closeDeclaration.Type != TokenType.ConstDeclarationR) ||
                (openDeclaration.Type == TokenType.VarDeclarationL && closeDeclaration.Type != TokenType.VarDeclarationR)
                ) {
                AddError(ParserErrors.VariableDeclarationMismatchedTypes, position);
            }

            TypeReference? type = null;
            if (identifierTokens.Count > 0) {
                // Expect a closing identifier token
                Token maybeClosingIdentifier = reader.Read();
                
                position = Rectangle.Union(position, maybeClosingIdentifier.Position);

                if (maybeClosingIdentifier is not IdentifierToken closingIdentifier) {
                    AddError(ParserErrors.UnexpectedToken, maybeClosingIdentifier.Position, "Unexpected token. Expected an identifier to close the variable declaration since one was opened.");
                    return new ErrorStatement(position);
                }

                type = new TypeReference(identifierTokens[0].Frame, closingIdentifier.Frame, identifierTokens.Take(1..).Select(x => x.Frame), position);
            }

            Expression? initializer = null;
            if (reader.Peek().Type == TokenType.Assignment) {
                reader.Read();
                initializer = ReadExpression();
            }

            return new VariableDeclaration(identifier.Frame, isConst ? VariableKind.Const : VariableKind.Var, type, initializer, position);
        }

        private Statement ReadAssignment(Token assignmentToken, List<IdentifierToken> identifierTokens) {
            return null!;
        }


        private Expression ReadExpression() {
            return ReadBinaryBooleanOr();
        }

        private Expression ReadBinaryBooleanOr() => GenericReadBinary(TokenType.BooleanOr, ReadBinaryBooleanXor);
        private Expression ReadBinaryBooleanXor() => GenericReadBinary(TokenType.BooleanOr, ReadBinaryBooleanAnd);
        private Expression ReadBinaryBooleanAnd() => GenericReadBinary(TokenType.BooleanOr, ReadComparison);

        private static readonly TokenType[] ComparisonTokens = new[] {
            TokenType.Equals, TokenType.NotEquals, TokenType.GreaterOrEqualThan, TokenType.GreaterThan, TokenType.LessOrEqualThan, TokenType.LessThan,
        };

        private Expression ReadComparison() => GenericReadBinary(ComparisonTokens, ReadBinaryAddSubtract);

        private static readonly TokenType[] AddSubtractTokens = new[] {
            TokenType.Add, TokenType.Subtract,
        };
        private Expression ReadBinaryAddSubtract() => GenericReadBinary(AddSubtractTokens, ReadMultiplyDivide);

        private static readonly TokenType[] MultiplyDivideTokens = new[] {
            TokenType.Divide, TokenType.Multiply, TokenType.Modulo,
        };
        private Expression ReadMultiplyDivide() => GenericReadBinary(MultiplyDivideTokens, ReadLiteral);


        private Expression ReadAccessor() {
            var expression = ReadLiteral();

            while (reader.Peek().Type == TokenType.Dot) {

            }

            return expression;
        }

        private Expression ReadLiteral() {
            return ReadLiteral(reader.Read());
        }

        private Expression ReadLiteral(Token token) {
            if (token.Type == TokenType.NumberLiteral) {
                return ReadNumberLiteral(token);
            }

            if (token.Type == TokenType.StringLiteral) {
                return ReadStringLiteral(token);
            }

            if (token.Type == TokenType.Identifier) {
                return ReadIdentifierLiteral(token);
            }


            AddError(ParserErrors.UnexpectedToken, token.Position, "Unexpected token. Expected an expression.");
            return new ErrorExpression(token.Position);
        }

        private Expression ReadIdentifierLiteral(Token token) {
            IdentifierToken identifierToken = (IdentifierToken)token;
            return new IdentifierExpression(identifierToken.Frame, identifierToken.Position);
        }

        private Expression ReadNumberLiteral(Token token) {
            NumberLiteralToken numberToken = (NumberLiteralToken)token;
            return new NumberLiteral(numberToken.Content, numberToken.Position);
        }

        private Expression ReadStringLiteral(Token token) {
            StringLiteralToken stringToken = (StringLiteralToken)token;
            return new StringLiteral(stringToken.Content, stringToken.Position);
        }
    }


    public (Node rootNode, Error[] errors) Parse(Token[] tokens) {
        ParserInstance instance = new ParserInstance(tokens);

        return instance.Run();
    }
}
