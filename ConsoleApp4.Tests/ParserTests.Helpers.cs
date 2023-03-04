using ConsoleApp4.CodeAnalysis;

using System.Collections.Immutable;

namespace ConsoleApp4.Tests;

public partial class ParserTests {
	public static string SerialiseTree(ExpressionSyntax rootSyntax) {
		var sb = new IndentedStringBuilder();

		AppendNode(rootSyntax);

		void AppendNode(ExpressionSyntax node) {
			sb.AppendLine(node.ToString());
			sb.Indent();

			var children = node switch {
				StringExpressionSyntax _ => ImmutableArray.Create<ExpressionSyntax>(),
				StringListExpressionSyntax list => list.Values.As<ExpressionSyntax>(),
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
