using ConsoleApp4;
using ConsoleApp4.CodeAnalysis;
using ConsoleApp4.Processes;

EnvironmentVariables.Path.Add(typeof(EchoProcess).AssemblyQualifiedName);
EnvironmentVariables.Path.Add(typeof(L33tSpeakProcess).AssemblyQualifiedName);
EnvironmentVariables.Path.Add(typeof(UppercaseProcess).AssemblyQualifiedName);

var fileSystem = new FileSystem();
var parser = new Parser("echo \"hello world\" | up > out1.txt");
var syntaxRoot = parser.Parse();
var evaluation = new Evaluator(syntaxRoot, fileSystem).Evaluate();

if (evaluation.Errors.Any()) {
	Console.WriteLine(string.Join('\n', evaluation.Errors));
}

if (evaluation.OutputStream is null) {
	return;
}

using var reader = new StreamReader(evaluation.OutputStream);

var result = reader.ReadToEnd();

Console.WriteLine(result);
Console.WriteLine($"Exited with code {evaluation.ExitCode}");