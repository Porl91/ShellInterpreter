using System.Collections.Concurrent;

namespace ConsoleApp4.Tests;

public sealed class FileSystemMock : IFileSystem {
	private readonly ConcurrentDictionary<string, Stream> _fileStreams = new();

	public Stream GetFileStream(string filepath, FileMode fileMode, FileAccess fileAccess) {
		filepath = Path.GetFullPath(filepath);

		if (!_fileStreams.ContainsKey(filepath)) {
			_fileStreams[filepath] = new SingletonMemoryStream();
		}

		_fileStreams[filepath].Position = 0;

		return _fileStreams[filepath];
	}

	public void DeleteFile(string filepath) {
		filepath = Path.GetFullPath(filepath);

		_fileStreams[filepath].Dispose();
		_fileStreams.TryRemove(filepath, out _);
	}
}

public sealed class SingletonMemoryStream : MemoryStream {
	protected override void Dispose(bool disposing) {
		// Disabled
	}

	public override ValueTask DisposeAsync() {
		// Disabled
		return ValueTask.CompletedTask;
	}

	public override void Close() {
		// Disabled
	}
}