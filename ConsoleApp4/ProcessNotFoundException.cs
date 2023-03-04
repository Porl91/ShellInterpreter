namespace ConsoleApp4;

public sealed class ProcessNotFoundException : Exception {
	public ProcessNotFoundException(string processId) {
		ProcessId = processId;
	}

	public string ProcessId { get; }
}
