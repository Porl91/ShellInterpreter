using System.Collections.Concurrent;

namespace ConsoleApp4;

public static class EnvironmentVariables {
	public static ConcurrentBag<string> Path = new();
}