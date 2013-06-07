using System;

namespace logger
{
	class MainClass
	{
		/// <summary>
		/// Log the specified text.
		/// </summary>
		/// <param name="text">Text.</param>
		public static void Log(string text)
		{
			Console.WriteLine(DateTime.Now.ToString() + ": " + text);
		}
		
		/// <summary>
		/// Debug log.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="verbosity">Verbosity.</param>
		public static void DebugLog(string text, int verbosity = 1)
		{
			if (Configuration.Verbosity >= verbosity)
			{
				Log ("DEBUG: " + text);
			}
		}
		
		/// <summary>
		/// Exceptions the handler.
		/// </summary>
		/// <param name="exception">Exception.</param>
		public static void exceptionHandler(Exception exception)
		{
			Log ("EXCEPTION: " + exception.Message + "\n\n" + exception.StackTrace + "\n\n" + exception.Source);
		}

		public static bool CheckStorage()
		{
			if (Configuration.Storage == Configuration.StorageType.file)
			{
				if (Configuration.Root == null)
				{
					Log ("FATAL: Undefined storage root");
					return false;
				}
				if (System.IO.Directory.Exists(Configuration.Root))
				{
					return true;
				}
				Log ("FATAL: Storage directory: " + Configuration.Root + " doesn't exist");
				return false;
			}
			Log ("FATAL: Unknown storage selected!");
			return false;
		}

		public static void Main (string[] args)
		{
			try
			{
				Log ("Logger " + Configuration.Version + " starting");
				if (CheckStorage ())
				{
					DebugLog ("Starting writer");
					Writer.Init ();
					DebugLog ("Starting tcp listener");
					ListenerTCP.Exec ();
					return;
				}
				Log ("FATAL: Error loading storage");
				return;
			} catch (Exception fail)
			{
				exceptionHandler(fail);
			}
		}
	}
}
