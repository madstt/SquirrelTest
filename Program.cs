using System;
using System.Linq;
using System.Reflection;
using NuGet;
using Squirrel;

namespace SquirrelTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Squirrel Tester v.{0}", Assembly.GetExecutingAssembly().GetName().Version.Major);

			Update();

			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}

		private static void Update()
		{
			SemanticVersion newVersion = null;
			var updated = false;
			using (var mgr = new UpdateManager("C:\\Projects\\SquirrelTest\\Releases"))
			{
				var updateInfo = mgr.CheckForUpdate().Result;

				var currentVersion = updateInfo.CurrentlyInstalledVersion.Version;

				Console.WriteLine("Current version: {0}", currentVersion);

				if (updateInfo.ReleasesToApply.Any())
				{
					newVersion = updateInfo.FutureReleaseEntry.Version;
					Console.WriteLine("New version: {0}", newVersion);

					Console.Write("New version available. Install? (y/n) ");
					if (Console.ReadKey().Key == ConsoleKey.Y)
					{
						Console.WriteLine();
						Console.WriteLine("Beginning to download...");

						mgr.DownloadReleases(updateInfo.ReleasesToApply).Wait();
						Console.WriteLine("... done downloading.");

						Console.WriteLine("Beginning to apply release...");
						mgr.ApplyReleases(updateInfo).Wait();
						Console.WriteLine("... done applying.");

						Console.WriteLine("Creating uninstaller...");
						mgr.CreateUninstallerRegistryEntry().Wait();
						Console.WriteLine("... done creating uninstaller.");
						updated = true;
					}
				}
			}

			if (updated && newVersion != null)
			{
				Console.WriteLine();
				Console.Write("Version {0} is now applied. Restart application? (y/n)",
					newVersion);
				if (Console.ReadKey().Key == ConsoleKey.Y)
				{
					Console.WriteLine();
					Console.WriteLine("Initiating restart...");

					UpdateManager.RestartApp();
				}
			}
		}


	}
}
