public abstract class Process {
	public Process(Stream standardIn, Stream standardOut, Stream standardError) {
		StandardIn = standardIn;
		StandardOut = standardOut;
		StandardError = standardError;
	}

	public Stream StandardIn { get; }
	public Stream StandardOut { get; }
	public Stream StandardError { get; }

	public abstract int Execute(string[] args);
}
