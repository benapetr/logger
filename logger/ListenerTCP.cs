using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;

namespace logger
{
	public class ListenerTCP
	{
		public static ulong OpenConnections = 0;
		public static ulong Connections = 0;

		public static void Exec()
		{
			MainClass.DebugLog("Listening on TCP port " + Configuration.Port);
			System.Net.Sockets.TcpListener server = new System.Net.Sockets.TcpListener(IPAddress.Any, Configuration.Port);
			server.Start();
			
			while(true)
			{
				try
				{
					System.Net.Sockets.TcpClient connection = server.AcceptTcpClient();
					Thread _client = new Thread(HandleClient);
					_client.Start(connection);
					
				} catch (Exception fail)
				{
					MainClass.exceptionHandler(fail);
				}
			}
		}

		private bool ValidName(string name)
		{
			if (name == null || name == "")
			{
				return false;
			}
			if (name.Contains (":") ||
			    name.Contains("*") ||
			    name.Contains(" ") ||
			    name.Contains("/") ||
			    name.Contains("\\") ||
			    name.Contains("?") ||
			    name.Contains(""))
			{
				return false;
			}
			return true;
		}

		private static bool Write(string log, string file, string section, int type)
		{
			try
			{
				if (!Directory.Exists(Configuration.Root + Path.DirectorySeparatorChar + file))
				{
					Directory.CreateDirectory(Configuration.Root + Path.DirectorySeparatorChar + file);
				}
				string name = file + "_" + section + ".txt";
				Writer.InsertText (DateTime.Now.ToString () + ": " + log, name);
				return true;
			} catch (Exception fail)
			{
				MainClass.exceptionHandler(fail);
			}
			return false;
		}

		private static void HandleClient(object data)
		{
			try
			{
				System.Net.Sockets.TcpClient connection = (System.Net.Sockets.TcpClient) data;
				MainClass.DebugLog("Incoming connection from: " + connection.Client.RemoteEndPoint.ToString());
				Connections++;
				OpenConnections++;
				connection.NoDelay = true;
				System.Net.Sockets.NetworkStream ns = connection.GetStream();
				System.IO.StreamReader Reader = new System.IO.StreamReader(ns);
				string text;
				string timestamp = "";
				// give the user access to global cache
				// save the reference to global cache because we might need it in future
				System.IO.StreamWriter Writer = new System.IO.StreamWriter(ns);
				while (connection.Connected && !Reader.EndOfStream)
				{
					text = Reader.ReadLine();
					string command = text;
					List<string> list = new List<string>();
					string parameters = "";
					if (command.Contains(" "))
					{
						parameters = command.Substring(command.IndexOf(" ") + 1);
						command = command.Substring(0, command.IndexOf(" "));
						if (parameters.Contains (" "))
						{
							list.AddRange (parameters.Split (" "));
						}
					}

					switch (command.ToLower ())
					{
					case "s":
					case "store":
						if (list.Count < 3)
						{
							Writer.WriteLine ("ERROR: you are missing parameters for this command");
							Writer.Flush();
							continue;
						}
						string project = list[0];
						string section = null;
						if (file.Contains (":"))
						{
							section = file.Substring (file.IndexOf(":") + 1);
							file = file.Substring(0, file.IndexOf(":"));
							if (!ValidName(section))
							{
								Writer.WriteLine ("ERROR: you provided invalid section name");
								Writer.Flush();
								continue;
							}
						}
						if (!ValidName(File))
						{
							Writer.WriteLine ("ERROR: you provided invalid section name");
							Writer.Flush();
							continue;
						}

						int type = 0;
						string l = text.Substring(list[0].Length + list[1].Length + 2);

						if (!int.TryParse (list[1], out type))
						{
							Writer.WriteLine ("ERROR: you provided invalid log type");
							Writer.Flush();
							continue;
						}

						if (Write (l, project, section, type))
						{
							Writer.WriteLine ("STORED");
							Writer.Flush();
							continue;
						}

						Writer.WriteLine ("ERROR: internal error, check debug log");
						Writer.Flush();
						continue;
					case "quit":
						connection.Close ();
						OpenConnections--;
						return;
					}
				}
			} catch (Exception fail)
			{
				MainClass.exceptionHandler(fail);
			}
			OpenConnections--;
		}
	}
}
