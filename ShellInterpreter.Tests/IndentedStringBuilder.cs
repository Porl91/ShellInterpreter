using System.Text;

namespace ShellInterpreter.Tests;

public sealed class IndentedStringBuilder {
	private const int _indentScale = 2;

	private readonly StringBuilder _sb;

	private int _x;

	public IndentedStringBuilder() {
		_sb = new StringBuilder();
	}

	public void AppendLine(string value) {
		_sb.AppendLine($"{new string(' ', _x * _indentScale)}{value}");
	}

	public void Clear() {
		_sb.Clear();
	}

	public void Indent() {
		_x++;
	}

	public void Outdent() {
		_x--;
	}

	public override string ToString() {
		return _sb.ToString();
	}
}