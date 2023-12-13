namespace ShellInterpreter;

public sealed class FileSystem : IFileSystem {
	public Stream GetFileStream(string filepath, FileMode fileMode, FileAccess fileAccess) {
		return new FileStream(filepath, fileMode, fileAccess);
	}

	public void DeleteFile(string filepath) {
		File.Delete(filepath);
	}
}