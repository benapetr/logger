using System.Collections.Generic;
using System;
using System.IO;

namespace logger
{
	public class Auth
	{
		private static FileSystemWatcher fs = new FileSystemWatcher();
		public static Dictionary<string,string> db = new Dictionary<string, string>();

		public static bool RequireLogin(string name)
		{
			lock(db)
			{
				if (db.ContainsKey (name.ToLower ()))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Login(string user, string pass)
		{
			lock(db)
			{
				if (db.ContainsKey (user.ToLower ()))
				{
					return pass == db[user.ToLower()];
				}
			}
			return true;
		}

		private static void Reload(object sender, EventArgs e)
		{
			lock (db)
			{
				MainClass.DebugLog("Reloading user");
				List<string> file = new List<string>();
				file.AddRange(File.ReadAllLines (Configuration.UserDB));
				db.Clear();
				foreach (string line in file)
				{
					if (!line.Contains (":"))
					{
						MainClass.DebugLog("Invalid line: " + line);
						continue;
					}
					string[] info = line.Split (':');
					if(info.Length < 2)
					{
						MainClass.DebugLog("Invalid user: " + line);
						continue;
					}
					db.Add(info[0].ToLower (), info[1]);
				}
			}
		}

		public static bool Init()
		{
			if (Configuration.UserDB != null)
			{
				List<string> file = new List<string>();
				file.AddRange(File.ReadAllLines (Configuration.UserDB));
				foreach (string line in file)
				{
					if (!line.Contains (":"))
					{
						MainClass.DebugLog("Invalid line: " + line);
						continue;
					}
					string[] info = line.Split (':');
					if(info.Length < 2)
					{
						MainClass.DebugLog("Invalid user: " + line);
						continue;
					}
					db.Add(info[0].ToLower (), info[1]);
				}

				FileInfo f = new FileInfo(Configuration.UserDB);

				fs.Path = f.Directory.FullName;
				fs.Filter = f.Name;
				fs.Created += new FileSystemEventHandler(Reload);
				fs.Changed += new FileSystemEventHandler(Reload);
				fs.EnableRaisingEvents = true;
				return true;
			}
			if (Configuration.RequireAuth)
			{
				MainClass.Log ("FATAL: you require authentication but there is no user db defined");
				return false;
			}
			return true;
		}
	}
}

