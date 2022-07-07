using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



public static class API
{
	public delegate void PipeDelegate();

	public enum injectionResult
	{
		RobloxNotFound,
		InjectionFailed,
		AlreadyInjected,
		DLLBlocked,
		InjectorBlocked,
		AlreadyInjecting,
		Success
	}

	private static int injectionms;

	private static int pipems;

	private static bool injecting;

	public static event PipeDelegate onInjected;

	public static Task<injectionResult> Inject()
	{
		TaskCompletionSource<injectionResult> res = new TaskCompletionSource<injectionResult>();
		if (!injecting)
		{
			new Thread((ThreadStart)async delegate
			{
				injecting = true;
				injectionms = 0;
				pipems = 0;
				if (Execution.Exists())
				{
					res.SetResult(injectionResult.AlreadyInjected);
					injecting = false;
				}
				else if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
				{
					res.SetResult(injectionResult.RobloxNotFound);
					injecting = false;
				}
				else
				{
					try
					{
						using WebClient webClient = new WebClient();
						webClient.DownloadFile("https://github.com/FLUORESCENTXX/Sploiter", "Sploiter.dll");
					}
					catch (Exception ex)
					{
						res.SetResult(injectionResult.DLLBlocked);
						File.WriteAllText("a.d", ex.ToString());
						injecting = false;
						return;
					}
					try
					{
						using WebClient webClient2 = new WebClient();
						if (!Directory.Exists("bin"))
						{
							Directory.CreateDirectory("bin");
						}
						webClient2.DownloadFile("https://github.com/FLUORESCENTXX/Sploiter", "bin\\injector.exe");
					}
					catch
					{
						res.SetResult(injectionResult.InjectorBlocked);
						injecting = false;
						return;
					}
					Process proc = new Process
					{
						StartInfo =
						{
							UseShellExecute = false,
							CreateNoWindow = true,
							WindowStyle = ProcessWindowStyle.Hidden,
							WorkingDirectory = Environment.CurrentDirectory,
							FileName = Environment.CurrentDirectory + "\\bin\\injector.exe"
						}
					};
					proc.Start();
					while (!proc.HasExited)
					{
						if (injectionms >= 10000)
						{
							res.SetResult(injectionResult.InjectionFailed);
							injecting = false;
							return;
						}
						if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
						{
							res.SetResult(injectionResult.RobloxNotFound);
							injecting = false;
							return;
						}
						await Task.Delay(200);
						injectionms += 200;
					}
					while (!Execution.Exists())
					{
						if (pipems >= 20000)
						{
							res.SetResult(injectionResult.InjectionFailed);
							injecting = false;
							return;
						}
						if (Process.GetProcessesByName("RobloxPlayerBeta").Length < 1)
						{
							res.SetResult(injectionResult.RobloxNotFound);
							injecting = false;
							return;
						}
						await Task.Delay(200);
						pipems += 200;
					}
					if (API.onInjected != null)
					{
						API.onInjected();
					}
					res.SetResult(injectionResult.Success);
					injecting = false;
				}
			}).Start();
			return res.Task;
		}
		res.SetResult(injectionResult.AlreadyInjecting);
		return res.Task;
	}
}