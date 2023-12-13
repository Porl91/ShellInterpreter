namespace ShellInterpreter;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ProcessIdAttribute : Attribute {
	public ProcessIdAttribute(string value) {
		Value = value;
	}

	public string Value { get; }
}