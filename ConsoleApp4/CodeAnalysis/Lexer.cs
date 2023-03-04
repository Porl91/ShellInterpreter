using System.Text;

namespace ConsoleApp4.CodeAnalysis;

internal sealed class Lexer {
	private int _position;
	private int _start;
	private string _text;

	private TokenType _type;
	private object _value;

	public Lexer(string text) {
		_text = text;
	}

	public SyntaxToken Lex() {
		ReadTrivia();
		ReadToken();

		var type = _type;
		var value = _value;

		ReadTrivia();

		return new SyntaxToken(type, value);
	}

	private void ReadToken() {
		_type = TokenType.BadToken;
		_value = null;
		_start = _position;

		switch (Current) {
			case '\0':
				_type = TokenType.EndOfFile;
				break;
			case '<':
				_position++;
				_type = TokenType.LessThan;
				break;
			case '>':
				_position++;
				_type = TokenType.GreaterThan;
				break;
			case '|':
				_position++;
				_type = TokenType.Pipe;
				break;
			case '\n':
			case '\r':
				if (Current == '\r' && Lookahead == '\n') {
					_position += 2;
				} else {
					_position++;
				}
				_type = TokenType.LineBreak;
				break;
			case '"':
				ReadQuotedString();
				break;
			default:
				ReadString();
				break;
		}
	}

	private void ReadTrivia() {
		_type = TokenType.BadToken;
		_value = null;
		_start = _position;

		switch (Current) {
			case ' ':
			case '\t':
				ReadWhitespace();
				break;
		}
	}

	private void ReadString() {
		var done = false;

		while (!done) {
			switch (Current) {
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					if (char.IsWhiteSpace(Current)) {
						done = true;
					} else {
						_position++;
					}
					break;
			}
		}

		_type = TokenType.String;
		_value = _text.Substring(_start, _position - _start);
	}

	private void ReadQuotedString() {
		var done = false;

		// Skip the first quote.
		_position++;

		var sb = new StringBuilder();

		while (!done) {
			switch (Current) {
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				case '\\':
					_position++;
					sb.Append(Current);
					_position++;
					break;
				case '"':
					// Skip the ending quote.
					_position++;
					done = true;
					break;
				default:
					sb.Append(Current);
					_position++;
					break;
			}
		}

		_type = TokenType.String;
		_value = sb.ToString();
	}

	private void ReadWhitespace() {
		var done = false;

		while (!done) {
			switch (Current) {
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					if (!char.IsWhiteSpace(Current)) {
						done = true;
					} else {
						_position++;
					}
					break;
			}
		}

		_type = TokenType.Whitespace;
	}

	private char Current => Peek(0);
	private char Lookahead => Peek(1);

	private char Peek(int offset) {
		var p = _position + offset;

		return p < _text.Length
			? _text[p]
			: '\0';
	}
}
