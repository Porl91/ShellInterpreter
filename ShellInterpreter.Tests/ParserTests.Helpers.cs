using ShellInterpreter.CodeAnalysis;

using System.Collections.Immutable;

namespace ShellInterpreter.Tests;

public partial class ParserTests {
	public static string SerialiseTree(ExpressionSyntax rootSyntax) {
		var sb = new IndentedStringBuilder();

		AppendNode(rootSyntax);

		void AppendNode(ExpressionSyntax node) {
			sb.AppendLine(node.ToString());
			sb.Indent();

			var children = node switch {
				FileExpressionSyntax _ => ImmutableArray.Create<ExpressionSyntax>(),
				CommandExpressionSyntax _ => ImmutableArray.Create<ExpressionSyntax>(),
				BinaryExpressionSyntax branch => ImmutableArray.Create(branch.Left, branch.Right),
				_ => ImmutableArray.Create<ExpressionSyntax>()
			};

			foreach (var child in children) {
				AppendNode(child);
			}

			sb.Outdent();
		}

		return sb.ToString().Trim();
	}
}
