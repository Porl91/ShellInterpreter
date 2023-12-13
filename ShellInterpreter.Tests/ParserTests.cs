using ShellInterpreter.CodeAnalysis;

namespace ShellInterpreter.Tests;

public sealed partial class ParserTests {
	[Fact]
	public void ParserTests_StringListsAreSupported() {
		var parser = new Parser("tr a-z A-Z");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Command "tr" (a-z, A-Z)
			""",
			result);
	}

	[Fact]
	public void ParserTests_QuotedStringsAreSupported() {
		var parser = new Parser("echo \"hello world 123\"");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Command "echo" (hello world 123)
			""",
			result);
	}

	[Fact]
	public void ParserTests_QuotedStringsWithEscapedQuotesAreSupported() {
		var parser = new Parser("echo \"hello \\\"world\\\" 123\"");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Command "echo" (hello "world" 123)
			""",
			result);
	}

	[Fact]
	public void ParserTests_PipesAreSupported() {
		var parser = new Parser("cat test.txt | tr a-z A-Z");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Pipe
			  Command "cat" (test.txt)
			  Command "tr" (a-z, A-Z)
			""",
			result);
	}

	[Fact]
	public void ParserTests_RedirectLeftToRightAreSupported() {
		var parser = new Parser("cat test.txt > test2.txt");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			GreaterThan
			  Command "cat" (test.txt)
			  File "test2.txt"
			""",
			result);
	}

	[Fact]
	public void ParserTests_RedirectRightToLeftAreSupported() {
		var parser = new Parser("echo < test.txt");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			LessThan
			  Command "echo"
			  File "test.txt"
			""",
			result);
	}
}