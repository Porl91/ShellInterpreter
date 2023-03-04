namespace ConsoleApp4.CodeAnalysis;

public sealed class BinaryExpressionSyntax : ExpressionSyntax {
	public override SyntaxType Type => SyntaxType.BinaryExpression;

	public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) {
		Left = left;
		OperatorToken = operatorToken;
		Right = right;
	}

	public ExpressionSyntax Left { get; }
	public SyntaxToken OperatorToken { get; }
	public ExpressionSyntax Right { get; }

	public override string ToString() {
		return OperatorToken.ToString();
	}
}