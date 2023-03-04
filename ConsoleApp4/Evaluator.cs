using ConsoleApp4.CodeAnalysis;

using System.Data;

namespace ConsoleApp4;

public sealed class Evaluator {
	private readonly ExpressionSyntax _root;
	private readonly IFileSystem _fileSystem;
	private readonly List<string> _errors = new();

	private ProcessExecutor _executor;

	public Evaluator(ExpressionSyntax root, IFileSystem fileSystem) {
		_root = root;
		_fileSystem = fileSystem;
	}

	public EvaluationResult Evaluate() {
		_executor = new ProcessExecutor(_fileSystem);

		try {
			EvaluateNode(_root);
		} catch (ProcessNotFoundException ex) {
			_errors.Add($"Command '{ex.ProcessId}' not found");
		}

		return new EvaluationResult(_executor.LastExitCode, _executor.LastStdout, _errors);
	}

	private void EvaluateNode(ExpressionSyntax node) {
		switch (node) {
			case StringExpressionSyntax strExpr:
				EvaluateCommand(strExpr);
				break;
			case StringListExpressionSyntax strListExpr:
				EvaluateCommandList(strListExpr);
				break;
			case BinaryExpressionSyntax binaryExpr:
				EvaluateBinaryExpression(binaryExpr);
				break;
		}
	}

	private void EvaluateBinaryExpression(BinaryExpressionSyntax expression) {
		switch (expression.OperatorToken.Type) {
			case TokenType.Pipe:
				if (expression.Left is StringListExpressionSyntax l) {
					EvaluateCommandList(l);
				} else {
					EvaluateNode(expression.Left);
				}

				if (expression.Right is StringListExpressionSyntax r) {
					EvaluateCommandList(r);
				} else {
					EvaluateNode(expression.Right);
				}
				break;
			case TokenType.GreaterThan:
			case TokenType.LessThan:

				// TODO: "expression.Left" can be a binary expression.

				EvaluateRedirect((StringListExpressionSyntax)expression.Left,
					(StringListExpressionSyntax)expression.Right,
					expression.OperatorToken.Type);
				break;
		}
	}

	private void EvaluateCommand(StringExpressionSyntax node) {
		var processId = (string)node.Value.Value;
		_executor.Pipe(processId, Array.Empty<string>());
	}

	private void EvaluateCommandList(StringListExpressionSyntax node) {
		var processId = (string)node.Values.First().Value.Value;
		var arguments = node.Values.Skip(1).Select(i => (string)i.Value.Value).ToArray();
		_executor.Pipe(processId, arguments);
	}

	private void EvaluateRedirect(StringListExpressionSyntax command,
		StringListExpressionSyntax file,
		TokenType direction) {
		var processId = (string)command.Values.First().Value.Value;

		var arguments = command.Values.Skip(1).Select(i => (string)i.Value.Value).ToArray();
		var filename = (string)file.Values.First().Value.Value;

		if (direction == TokenType.GreaterThan) {
			_executor.RedirectOutput(processId, arguments, filename);
		} else {
			_executor.RedirectInput(processId, arguments, filename);
		}
	}
}
