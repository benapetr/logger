using System.IO;
using System.Xml;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System;

namespace logger
{
	public class Writer
	{
		public class Line : IComparable
		{
			public string text = null;
			public string file = null;

			public Line(string Text, string File)
			{
				text = Text;
				file = File;
			}
		}


		public static bool IsWriting = true;
		private static System.Threading.Thread thread;
		private static List<Line> db = new List<Line>();

		public static void Init()
		{
			thread = new System.Threading.Thread(exec);
			thread.Start();
		}

		public static void InsertText(string data, string filename)
		{
			lock (db)
			{
				db.Add(new Line(data, filename));
			}
		}

		private static void exec()
		{
			try
			{
				while (IsWriting)
				{
					List<Line> temp = new List<Line>();
					lock (db)
					{
						temp.AddRange (db);
						db.Clear();
					}
					foreach (Line line in temp)
					{
						try
						{
							System.IO.File.AppendAllText (line.file, line.text + "\n");
						}catch (Exception fail)
						{
							MainClass.exceptionHandler (fail);
							MainClass.DebugLog("Unable to store a line, returning back to db");
							lock (db)
							{
								db.Add(line);
							}
							continue;
						}
					}
					System.Threading.Thread.Sleep (8000);
				}
			} catch (Exception fail)
			{
				MainClass.exceptionHandler (fail);
				MainClass.Log ("ERROR:  Writer is down");
			}
		}
	}
}

