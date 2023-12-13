namespace ShellInterpreter.CodeAnalysis;

public abstract class ExpressionSyntax {
	public abstract SyntaxType Type { get; }
	public abstract override string ToString();
}
