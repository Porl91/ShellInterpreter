using System.Reflection;

namespace ShellInterpreter;

public sealed class ProcessExecutor {
	private readonly IFileSystem _fileSystem;

	public int LastExitCode = 0;
	public Stream LastStdOut = null;
	private Dictionary<string, Type> _cachedProcessTypes;

	public ProcessExecutor(IFileSystem fileSystem) {
		_fileSystem = fileSystem;
	}

	public void Execute(string processName, string[] args) {
		Stream stdin;

		if (LastStdOut is not null) {
			stdin = LastStdOut;
			stdin.Position = 0;
		} else {
			stdin = new MemoryStream();
		}

		var stdout = new MemoryStream();
		var stderr = new MemoryStream();

		var type = GetProcessTypeById(processName);
		var process = (Process)Activator.CreateInstance(type, stdin, stdout, stderr);

		LastExitCode = process.Execute(args);

		stdin.Position = 0;
		stdout.Position = 0;

		LastStdOut = stdout;
	}

	public void CopyTo(Stream stream) {
		// "LastStdOut" should not be null but this point, as it's created by the process execution 
		// on the left hand side of the binary expression "proc > file"
		if (LastStdOut is null) {
			return;
		}

		LastStdOut.CopyTo(stream);

		LastStdOut.Position = 0;
		stream.Position = 0;
	}

	public void CopyFrom(Stream stream) {
		// This will be used when a node in the expression tree isn't executable (as as such won't have 
		// it's own standard IO streams), but we want to redirect output from it (i.e. a file).
		// This is the result of a binary expression such as "proc < file", which reverses the evaluation 
		// order of "proc" and "file", so the standard output stream will be created here, which "proc" 
		// will use in place of it's own standard input when it executes.
		LastStdOut = new MemoryStream();

		stream.CopyTo(LastStdOut);

		LastStdOut.Position = 0;
		stream.Position = 0;
	}

	private Type GetProcessTypeById(string id) {
		_cachedProcessTypes ??= EnvironmentVariables.Path.Select(Type.GetType).ToDictionary(t => {
			return t.GetCustomAttribute<ProcessIdAttribute>(false)?.Value
				?? t.AssemblyQualifiedName;
		}, t => t, StringComparer.OrdinalIgnoreCase);

		if (!_cachedProcessTypes.ContainsKey(id)) {
			throw new ProcessNotFoundException(id);
		}

		return _cachedProcessTypes[id];
	}
}
