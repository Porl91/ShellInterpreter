public sealed class EvaluationResult {
	public EvaluationResult(int exitCode, Stream outputStream, IEnumerable<string> errors) {
		ExitCode = exitCode;
		OutputStream = outputStream;
		Errors = errors;
	}

	public int ExitCode { get; }
	public Stream OutputStream { get; }
	public IEnumerable<string> Errors { get; }
}