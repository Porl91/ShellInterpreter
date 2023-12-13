namespace ShellInterpreter.CodeAnalysis;

public sealed class SyntaxToken {
	public SyntaxToken(TokenType type, object value) {
		Type = type;
		Value = value;
	}

	public TokenType Type { get; }
	public object Value { get; }

	public override string ToString() {
		if (Value is null) {
			return Type.ToString();
		}

		return $"{Type}: {Value}";
	}
}