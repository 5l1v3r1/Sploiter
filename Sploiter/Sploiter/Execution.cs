using Sploiter.Sploiter;
using System.IO;

namespace Sploiter.Sploiter { }



public static class Execution
{
	public enum ExecutionResult
	{
		Success,
		DLLNotFound,
		PipeNotFound,
		Failed
	}

	private static Pipe mainP = new Pipe("Sploiter");

	public static bool Exists()
	{
		return mainP.Exists();
	}

	public static ExecutionResult Execute(string script)
	{
		if (!mainP.Exists())
		{
			return ExecutionResult.PipeNotFound;
		}
		if (!File.Exists("Sploiter.dll"))
		{
			return ExecutionResult.DLLNotFound;
		}
		try
		{
			mainP.Write(script);
			return ExecutionResult.Success;
		}
		catch
		{
			return ExecutionResult.Failed;
		}
	}
}

