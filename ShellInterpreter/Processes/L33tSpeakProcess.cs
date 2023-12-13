using System.Text;

namespace ShellInterpreter.Processes;

[ProcessId("l33t")]
public sealed class L33tSpeakProcess : Process {
	public L33tSpeakProcess(Stream standardIn, Stream standardOut, Stream standardError) : base(standardIn, standardOut, standardError) {
	}

	public override int Execute(string[] args) {
		using var reader = new StreamReader(StandardIn);

		var value = reader.ReadToEnd();

		for (var i = 0; i < value.Length; i++) {
			StandardOut.Write(Encoding.UTF8.GetBytes(value[i].ToString()
				.Replace('e', '3').Replace('E', '3')
				.Replace('a', '4').Replace('A', '4')
				.Replace('l', '7').Replace('L', '7')
				.Replace('i', '1').Replace('I', '1')
			));
		}

		return 0;
	}
}
