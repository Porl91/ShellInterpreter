namespace ShellInterpreter;

public interface IFileSystem {
	Stream GetFileStream(string filepath, FileMode fileMode, FileAccess fileAccess);
	void DeleteFile(string filepath);
}
