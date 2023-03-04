using System.Collections.Immutable;

namespace ConsoleApp4.CodeAnalysis;

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
		var root = ParseMember();
		MatchToken(TokenType.EndOfFile);
		return root;
	}

	private ExpressionSyntax ParseMember() {
		return ParseBinaryExpression();
	}

	private ExpressionSyntax ParsePrimaryExpression() {
		return ParseStringList();
	}

	private ExpressionSyntax ParseStringList() {
		var stringTokens = ImmutableArray.CreateBuilder<StringExpressionSyntax>();

		while (Current.Type == TokenType.String) {
			var stringToken = MatchToken(TokenType.String);

			stringTokens.Add(new StringExpressionSyntax(stringToken));
		}

		return new StringListExpressionSyntax(stringTokens.ToImmutable());
	}

	private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0) {
		var left = ParsePrimaryExpression();

		while (true) {
			var precedence = Current.Type.GetBinaryOperatorPrecedence();
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			var operatorToken = NextToken();
			var right = ParseBinaryExpression(precedence);
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
