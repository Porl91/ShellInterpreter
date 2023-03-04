using System.Collections.Immutable;

namespace ConsoleApp4.CodeAnalysis;

public sealed class StringListExpressionSyntax : ExpressionSyntax {
	public override SyntaxType Type => SyntaxType.StringList;

	public ImmutableArray<StringExpressionSyntax> Values { get; }

	public StringListExpressionSyntax(ImmutableArray<StringExpressionSyntax> values) {
		Values = values;
	}

	public override string ToString() {
		return Type.ToString();
	}
}
