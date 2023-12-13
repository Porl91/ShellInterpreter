using ShellInterpreter.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using System.Text;

namespace ShellInterpreter.Tests;

public partial class EvaluatorTests {
	private string EvaluateSuccess(string input) {
		var parser = new Parser(input);
		var syntaxRoot = parser.Parse();
		var evaluation = new Evaluator(syntaxRoot, _fileSystem).Evaluate();

		Assert.Empty(evaluation.Errors);
		Assert.Equal(0, evaluation.ExitCode);
		Assert.NotNull(evaluation.OutputStream);

		using var reader = new StreamReader(evaluation.OutputStream);

		return reader.ReadToEnd();
	}

	private void AssertFile(string filepath, string content) {
		using var stream = _fileSystem.GetFileStream(filepath, FileMode.Open, FileAccess.Read);
		using var reader = new StreamReader(stream);

		Assert.Equal(content, reader.ReadToEnd());

		_fileSystem.DeleteFile(filepath);
	}

	[ProcessId("append")]
	public sealed class Append : Process {
		public Append(Stream standardIn, Stream standardOut, Stream standardError) : base(standardIn, standardOut, standardError) {
		}

		public override int Execute(string[] args) {
			StandardIn.CopyTo(StandardOut);
			StandardOut.Write(Encoding.UTF8.GetBytes(string.Join(" ", args)));
			return 0;
		}
	}

	[ProcessId("echo_input")]
	public sealed class EchoInput : Process {
		public EchoInput(Stream standardIn, Stream standardOut, Stream standardError) : base(standardIn, standardOut, standardError) {
		}

		public override int Execute(string[] args) {
			StandardIn.CopyTo(StandardOut);
			return 0;
		}
	}

	public sealed class EvaluatorFixture : IDisposable {
		public IServiceProvider ServiceProvider { get; }

		public EvaluatorFixture() {
			var services = new ServiceCollection();
			services.AddSingleton<IFileSystem, FileSystemMock>();

			ServiceProvider = services.BuildServiceProvider();

			EnvironmentVariables.Path.Add(typeof(Append).AssemblyQualifiedName);
			EnvironmentVariables.Path.Add(typeof(EchoInput).AssemblyQualifiedName);
		}

		public void Dispose() {
			EnvironmentVariables.Path.Clear();
		}
	}
}
