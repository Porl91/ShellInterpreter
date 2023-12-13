using System.Collections.Immutable;

namespace ShellInterpreter.CodeAnalysis;

public sealed class Parser {
	private readonly string _text;
	private readonly ImmutableArray<SyntaxToken> _tokens;

	private int _position;

	public Parser(string text) {
		_text = text;

		var lexer = new Lexer(_text);
		var tokens = new List<SyntaxToken>();

		SyntaxToken token;

		do {
			token = lexer.Lex();

			tokens.Add(token);
		} while (token.Type != TokenType.EndOfFile);

		_tokens = tokens.ToImmutableArray();
	}

	public ExpressionSyntax Parse() {
		var root = ParseBinaryExpression();
		MatchToken(TokenType.EndOfFile);
		return root;
	}

	private CommandExpressionSyntax ParseCommand() {
		var stringTokens = ImmutableArray.CreateBuilder<string>();

		while (Current.Type == TokenType.String) {
			var stringToken = MatchToken(TokenType.String);

			stringTokens.Add((string)stringToken.Value);
		}

		return new CommandExpressionSyntax(stringTokens.ToImmutable());
	}

	private FileExpressionSyntax ParseFile() {
		var stringToken = MatchToken(TokenType.String);

		return new FileExpressionSyntax((string)stringToken.Value);
	}

	private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0) {
		ExpressionSyntax left = ParseCommand();

		while (true) {
			var precedence = Current.Type.GetBinaryOperatorPrecedence();
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			var operatorToken = NextToken();

			ExpressionSyntax right;
			if (operatorToken.Type == TokenType.LessThan || operatorToken.Type == TokenType.GreaterThan) {
				right = ParseFile();
			} else {
				right = ParseBinaryExpression(precedence);
			}

			left = new BinaryExpressionSyntax(left, operatorToken, right);
		}

		return left;
	}

	private SyntaxToken Peek(int offset) {
		var index = _position + offset;
		if (index >= _tokens.Length)
			return _tokens[^1];

		return _tokens[index];
	}

	private SyntaxToken Current => Peek(0);

	private SyntaxToken NextToken() {
		var current = Current;
		_position++;
		return current;
	}

	private SyntaxToken MatchToken(TokenType type) {
		if (Current.Type == type)
			return NextToken();

		return new SyntaxToken(type, null);
	}
}
