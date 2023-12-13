using ShellInterpreter.CodeAnalysis;

namespace ShellInterpreter;

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

		return new EvaluationResult(_executor.LastExitCode, _executor.LastStdOut, _errors);
	}

	private void EvaluateNode(ExpressionSyntax node) {
		switch (node) {
			case CommandExpressionSyntax strListExpr:
				EvaluateCommand(strListExpr);
				break;
			case BinaryExpressionSyntax binaryExpr:
				EvaluateBinaryExpression(binaryExpr);
				break;
		}
	}

	private void EvaluateBinaryExpression(BinaryExpressionSyntax expression) {
		switch (expression.OperatorToken.Type) {
			case TokenType.Pipe:
				EvaluateNode(expression.Left);
				EvaluateNode(expression.Right);
				break;
			case TokenType.GreaterThan: {
					EvaluateNode(expression.Left);

					if (expression.Right is FileExpressionSyntax fileExpr) {
						var fileStream = _fileSystem.GetFileStream(fileExpr.Value, FileMode.OpenOrCreate, FileAccess.ReadWrite);

						_executor.CopyTo(fileStream);
					} else {
						EvaluateNode(expression.Right);
					}

					break;
				}
			case TokenType.LessThan: {
					if (expression.Right is FileExpressionSyntax fileExpr) {
						var fileStream = _fileSystem.GetFileStream(fileExpr.Value, FileMode.Open, FileAccess.Read);

						_executor.CopyFrom(fileStream);
					} else {
						EvaluateNode(expression.Right);
					}

					EvaluateNode(expression.Left);
					break;
				}
		}
	}

	private void EvaluateCommand(CommandExpressionSyntax node) {
		var processId = node.Values.First();
		var arguments = node.Values.Skip(1).ToArray();
		_executor.Execute(processId, arguments);
	}
}
