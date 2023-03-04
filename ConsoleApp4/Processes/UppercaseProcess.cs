using System.Text;

namespace ConsoleApp4.Processes;

[ProcessId("up")]
public sealed class UppercaseProcess : Process {
	public UppercaseProcess(Stream standardIn, Stream standardOut, Stream standardError) : base(standardIn, standardOut, standardError) {
	}

	public override int Execute(string[] args) {
		using var reader = new StreamReader(StandardIn);

		var value = reader.ReadToEnd();

		for (var i = 0; i < value.Length; i++) {
			StandardOut.Write(Encoding.UTF8.GetBytes(value[i].ToString().ToUpper()));
		}

		return 0;
	}
}
