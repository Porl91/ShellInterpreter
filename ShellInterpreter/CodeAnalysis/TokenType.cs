namespace ShellInterpreter.CodeAnalysis;

public enum TokenType {
	BadToken,
	String,
	LessThan,
	GreaterThan,
	Pipe,
	LineBreak,
	Whitespace,
	EndOfFile
}

internal static class TokenTypeExtensions {
	public static int GetBinaryOperatorPrecedence(this TokenType source) {
		return source switch {
			TokenType.LessThan or TokenType.GreaterThan => 2,
			TokenType.Pipe => 1,
			_ => 0,
		};
	}
}