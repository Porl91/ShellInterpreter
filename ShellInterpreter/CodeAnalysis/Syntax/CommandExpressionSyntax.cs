using System.Collections.Immutable;
using System.Text;

namespace ShellInterpreter.CodeAnalysis;

public sealed class CommandExpressionSyntax : ExpressionSyntax {
	public override SyntaxType Type => SyntaxType.Command;

	public ImmutableArray<string> Values { get; }

	public CommandExpressionSyntax(ImmutableArray<string> values) {
		Values = values;
	}

	public override string ToString() {
		var sb = new StringBuilder();
		sb.Append($"{Type} \"{Values[0]}\"");

		if (Values.Length > 1) {
			sb.Append($" ({string.Join(", ", Values.Skip(1))})");
		}

		return sb.ToString();
	}
}
