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
							list.AddRange (parameters.Split (' '));
						}
					}

					string project = null;
					string section = null;
					string l = null;
					string token = null;
					int type = 0;

					switch (command.ToLower ())
					{
					case "n":
					case "s":
					case "store":
						if (Configuration.RequireAuth)
						{
							Writer.WriteLine ("ERROR: you need to authenticate to log here");
							Writer.Flush();
							continue;
						}
						if (list.Count < 3)
						{
							Writer.WriteLine ("ERROR: you are missing parameters for this command");
							Writer.Flush();
							continue;
						}
						project = list[0];
						section = null;
						if (project.Contains (":"))
						{
							section = project.Substring (project.IndexOf(":") + 1);
							project = project.Substring(0, project.IndexOf(":"));
							if (!Logger.ValidName(section))
							{
								Writer.WriteLine ("ERROR: you provided invalid section name");
								Writer.Flush();
								continue;
							}
						}
						if (!Logger.ValidName(project))
						{
							Writer.WriteLine ("ERROR: you provided invalid section name");
							Writer.Flush();
							continue;
						}

						if (Auth.RequireLogin (project))
						{
							Writer.WriteLine ("ERROR: you need to authenticate to log here");
							Writer.Flush();
							continue;
						}

						type = 0;
						l = text.Substring(list[0].Length + list[1].Length + command.Length + 3);

						if (!int.TryParse (list[1], out type))
						{
							Writer.WriteLine ("ERROR: you provided invalid log type");
							Writer.Flush();
							continue;
						}

						if (Logger.Write (l, project, section, type))
						{
							if (command != "n")
							{
								Writer.WriteLine ("STORED");
								Writer.Flush();
							}
							continue;
						}

						Writer.WriteLine ("ERROR: internal error, check debug log");
						Writer.Flush();
						continue;
					case "a":
						if (list.Count < 4)
						{
							Writer.WriteLine ("ERROR: you are missing parameters for this command");
							Writer.Flush();
							continue;
						}
						project = list[0];
						section = null;
						if (project.Contains (":"))
						{
							section = project.Substring (project.IndexOf(":") + 1);
							project = project.Substring(0, project.IndexOf(":"));
							if (!Logger.ValidName(section))
							{
								Writer.WriteLine ("ERROR: you provided invalid section name");
								Writer.Flush();
								continue;
							}
						}
						if (!Logger.ValidName(project))
						{
							Writer.WriteLine ("ERROR: you provided invalid section name");
							Writer.Flush();
							continue;
						}
						
						token = list[2];
						
						type = 0;
						l = text.Substring(list[0].Length + list[1].Length + command.Length + token.Length + 4);

						if (!Auth.Login (project, token))
						{
							Writer.WriteLine ("ERROR: you provided invalid token");
							Writer.Flush();
							continue;
						}

						if (!int.TryParse (list[1], out type))
						{
							Writer.WriteLine ("ERROR: you provided invalid log type");
							Writer.Flush();
							continue;
						}
						
						if (Logger.Write (l, project, section, type))
						{
							if (command != "n")
							{
								Writer.WriteLine ("STORED");
								Writer.Flush();
							}
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
					Writer.WriteLine ("ERROR");
					Writer.Flush();
				}
			} catch (Exception fail)
			{
				MainClass.exceptionHandler(fail);
			}
			OpenConnections--;
		}
	}
}

