﻿using Microsoft.Extensions.DependencyInjection;

using System.Text;

using static ShellInterpreter.Tests.EvaluatorTests;

namespace ShellInterpreter.Tests;

public sealed partial class EvaluatorTests : IClassFixture<EvaluatorFixture> {
	private readonly EvaluatorFixture _processFixture;
	private readonly IFileSystem _fileSystem;

	public EvaluatorTests(EvaluatorFixture processFixture) {
		_processFixture = processFixture;

		_fileSystem = _processFixture.ServiceProvider.GetRequiredService<IFileSystem>();
	}

	[Fact]
	public void EvaluatorTests_PipesAreSupported() {
		var result = EvaluateSuccess("append test1 | append test2");

		Assert.Equal("test1test2", result);
	}

	[Fact]
	public void EvaluatorTests_Redirects_LeftToRight_AreSupported() {
		var result = EvaluateSuccess("append test1 > test1.txt");

		Assert.Equal("test1", result);
		AssertFile("test1.txt", "test1");
	}

	[Fact]
	public void EvaluatorTests_Redirects_RightToLeft_AreSupported() {
		var fileStreamTmp = _fileSystem.GetFileStream("test2_tmp.txt", FileMode.Open, FileAccess.ReadWrite);
		fileStreamTmp.Write(Encoding.UTF8.GetBytes("test2_tmp"));

		var result = EvaluateSuccess($"echo_input < test2_tmp.txt > test2.txt");

		Assert.Equal("test2_tmp", result);
		AssertFile("test2.txt", "test2_tmp");
	}
}