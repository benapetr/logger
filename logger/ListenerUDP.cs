using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace logger
{
	public class ListenerUDP
	{
		private static System.Threading.Thread thread;
		
		public static void Init()
		{
			thread = new System.Threading.Thread(exec);
			thread.Start();
		}

		public class State
		{
			public IPEndPoint ipEndPoint;
			public UdpClient listener;
		}

		public static void exec()
		{
			try
			{
				while(true)
				{
					int recv;
					byte[] data = new byte[1024];
					IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Configuration.Port);
					
					Socket newsock = new Socket(AddressFamily.InterNetwork,
					                            SocketType.Dgram, ProtocolType.Udp);
					
					newsock.Bind(ipep);
					
					IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
					EndPoint Remote = (EndPoint)(sender);
					
					recv = newsock.ReceiveFrom(data, ref Remote);
					
					MainClass.DebugLog("Message received from: " + Remote.ToString());
					string t = Encoding.ASCII.GetString(data, 0, recv);
					List<string> xx = new List<string>();
					if (t.Contains ("\n"))
					{
						xx.AddRange (t.Split ('\n'));
					} else
					{
						xx.Add(t);
					}
					foreach (string text in xx)
					{
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
						
						switch (command.ToLower ())
						{
						case "s":
						case "store":
							if (list.Count < 3)
							{
								MainClass.DebugLog(Remote.ToString()+ ":   ERROR: you are missing parameters for this command");
								continue;
							}
							string project = list[0];
							string section = null;
							if (project.Contains (":"))
							{
								section = project.Substring (project.IndexOf(":") + 1);
								project = project.Substring(0, project.IndexOf(":"));
								if (!ListenerTCP.ValidName(section))
								{
									MainClass.DebugLog(Remote.ToString() + ":   ERROR: you provided invalid section name");
									continue;
								}
							}
							if (!ListenerTCP.ValidName(project))
							{
								MainClass.DebugLog(Remote.ToString() + ":   ERROR: you provided invalid section name");
								continue;
							}
							
							int type = 0;
							string l = text.Substring(list[0].Length + list[1].Length + command.Length + 3);
							
							if (!int.TryParse (list[1], out type))
							{
								MainClass.DebugLog ("ERROR: you provided invalid log type");
								continue;
							}
							
							if (ListenerTCP.Write (l, project, section, type))
							{
								MainClass.DebugLog ("STORED");
								continue;
							}
							
							MainClass.DebugLog ("ERROR: internal error, check debug log");
							continue;
						}
						newsock.Close ();
					}
				}
			}catch (Exception fail)
			{
				MainClass.exceptionHandler(fail);
			}
		}
	}
}

