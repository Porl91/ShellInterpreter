using System.Text;

namespace ShellInterpreter.Processes;

[ProcessId("echo")]
public sealed class EchoProcess : Process {
	public EchoProcess(Stream standardIn, Stream standardOut, Stream standardError) : base(standardIn, standardOut, standardError) {
	}

	public override int Execute(string[] args) {
		for (var i = 0; i < args.Length; i++) {
			StandardOut.Write(Encoding.UTF8.GetBytes(args[i]));
		}

		return 0;
	}
}
