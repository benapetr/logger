using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace logger
{
	public class Logger
	{
		public static bool ValidName(string name)
		{
			if (name == null || name == "")
			{
				MainClass.DebugLog("Empty project name, ignoring");
				return false;
			}
			if (name.Contains (":") ||
			    name.Contains("*") ||
			    name.Contains(" ") ||
			    name.Contains("/") ||
			    name.Contains("\\") ||
			    name.Contains("?"))
			{
				MainClass.DebugLog("Dangerous char in project name: >>" + name + "<<");
				return false;
			}
			return true;
		}

		public static string Unix()
		{
			return (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds.ToString ();
		}

		public static bool Write(string log, string file, string section, int type)
		{
			try
			{
				if (!Directory.Exists(Configuration.Root + Path.DirectorySeparatorChar + file))
				{
					Directory.CreateDirectory(Configuration.Root + Path.DirectorySeparatorChar + file);
				}

				// format the name of file
				switch (type)
				{
				case 3:
				case 1:
				case 5:
					section = section + "_" + DateTime.Now.ToString ("yyyy_MM_dd");
					break;
				}

				string suffix = ".txt";

				switch(type)
				{
				case 2:
				case 3:
					suffix = ".html";
					break;
				case 4:
				case 5:
					suffix = ".xml";
					break;
				}

				string name = Configuration.Root +
					Path.DirectorySeparatorChar + file + Path.DirectorySeparatorChar + 
						file + "_" + section + suffix;
				if (section == "" || section == null)
				{
					name = Configuration.Root +
						Path.DirectorySeparatorChar + file + Path.DirectorySeparatorChar + 
							file + suffix;
				}

				string data = DateTime.Now.ToString () + ": " + log;
				// format text

				switch(type)
				{
				case 2:
				case 3:
					data = "<font class=\"timestamp\">" + DateTime.Now.ToString () +
						"</font><font class=\"text\">: " + System.Web.HttpUtility.HtmlEncode( log ) + "</font><br>";
					break;
				case 4:
				case 5:
					data = "<data time=\""+Unix ()+"\">" + System.Web.HttpUtility.HtmlEncode( log ) + "</data>";
					break;
				}

				Writer.InsertText (data, name);
				return true;
			} catch (Exception fail)
			{
				MainClass.exceptionHandler(fail);
			}
			return false;
		}

	}
}

