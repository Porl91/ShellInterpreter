namespace ConsoleApp4.CodeAnalysis;

public sealed class StringExpressionSyntax : ExpressionSyntax {
	public override SyntaxType Type => SyntaxType.String;

	public readonly SyntaxToken Value;

	public StringExpressionSyntax(SyntaxToken value) {
		Value = value;
	}

	public override string ToString() {
		return $"{Type}: {Value.Value}";
	}
}
