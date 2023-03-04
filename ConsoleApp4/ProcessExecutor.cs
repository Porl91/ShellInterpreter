using System.Reflection;

namespace ConsoleApp4;

public sealed class ProcessExecutor {
	private readonly IFileSystem _fileSystem;

	public int LastExitCode = 0;
	public Stream LastStdout = null;
	private Dictionary<string, Type> _cachedProcessTypes;

	public ProcessExecutor(IFileSystem fileSystem) {
		_fileSystem = fileSystem;
	}

	public void Pipe(string processName, string[] args) {
		var stdin = LastStdout ?? new MemoryStream();
		var stdout = new MemoryStream();
		var stderr = new MemoryStream();

		var type = GetProcessTypeById(processName);
		var process = (Process)Activator.CreateInstance(type, stdin, stdout, stderr);

		LastExitCode = process.Execute(args);

		stdout.Position = 0;

		LastStdout = stdout;
	}

	public void RedirectInput(string processName, string[] args, string filename) {
		var stdin = _fileSystem.GetFileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
		var stdout = new MemoryStream();
		var stderr = new MemoryStream();

		var type = GetProcessTypeById(processName);
		var process = (Process)Activator.CreateInstance(type, stdin, stdout, stderr);

		LastExitCode = process.Execute(args);

		stdout.Position = 0;

		LastStdout = stdout;
	}

	public void RedirectOutput(string processName, string[] args, string filename) {
		var stdin = LastStdout ?? new MemoryStream();
		var stdout = _fileSystem.GetFileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		var stderr = new MemoryStream();

		var type = GetProcessTypeById(processName);
		var process = (Process)Activator.CreateInstance(type, stdin, stdout, stderr);

		LastExitCode = process.Execute(args);

		stdout.Position = 0;

		LastStdout = stdout;
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
