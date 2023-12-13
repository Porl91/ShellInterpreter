namespace ShellInterpreter.CodeAnalysis;

public sealed class FileExpressionSyntax : ExpressionSyntax {
	public override SyntaxType Type => SyntaxType.File;

	public string Value { get; }

	public FileExpressionSyntax(string value) {
		Value = value;
	}

	public override string ToString() {
		return $"{Type} \"{Value}\"";
	}
}
