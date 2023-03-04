using ConsoleApp4.CodeAnalysis;

namespace ConsoleApp4.Tests;

public sealed partial class ParserTests {
	[Fact]
	public void ParserTests_StringListsAreSupported() {
		var parser = new Parser("tr a-z A-Z");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			StringList
			  String: tr
			  String: a-z
			  String: A-Z
			""",
			result);
	}

	[Fact]
	public void ParserTests_QuotedStringsAreSupported() {
		var parser = new Parser("echo \"hello world 123\"");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			StringList
			  String: echo
			  String: hello world 123
			""",
			result);
	}

	[Fact]
	public void ParserTests_QuotedStringsWithEscapedQuotesAreSupported() {
		var parser = new Parser("echo \"hello \\\"world\\\" 123\"");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			StringList
			  String: echo
			  String: hello "world" 123
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
			  StringList
			    String: cat
			    String: test.txt
			  StringList
			    String: tr
			    String: a-z
			    String: A-Z
			""",
			result);
	}

	[Fact]
	public void ParserTests_RedirectLeftToRightAreSupported() {
		var parser = new Parser("cat test.txt > result");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			GreaterThan
			  StringList
			    String: cat
			    String: test.txt
			  StringList
			    String: result
			""",
			result);
	}

	[Fact]
	public void ParserTests_RedirectRightToLeftAreSupported() {
		var parser = new Parser("result < cat test.txt");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			LessThan
			  StringList
			    String: result
			  StringList
			    String: cat
			    String: test.txt
			""",
			result);
	}

	[Fact]
	public void ParserTests_Precedence_RedirectBeforePipe() {
		var parser = new Parser("cat test.txt | result < cat2.txt");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Pipe
			  StringList
			    String: cat
			    String: test.txt
			  LessThan
			    StringList
			      String: result
			    StringList
			      String: cat2.txt
			""",
			result);
	}

	[Fact]
	public void ParserTests_Precedence_RedirectBeforePipe_Reversed() {
		var parser = new Parser("cat2.txt > result | tr");
		var syntaxRoot = parser.Parse();
		var result = SerialiseTree(syntaxRoot);

		Assert.Equal($"""
			Pipe
			  GreaterThan
			    StringList
			      String: cat2.txt
			    StringList
			      String: result
			  StringList
			    String: tr
			""",
			result);
	}
}