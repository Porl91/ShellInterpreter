using System.Collections.Concurrent;

namespace ShellInterpreter;

public static class EnvironmentVariables {
	public static ConcurrentBag<string> Path = new();
}