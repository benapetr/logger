/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/


using System;
using System.Collections.Generic;

namespace logger
{
	public class Terminal
	{
		private static void PrintConf()
		{
			Console.WriteLine ("<configuration>\n" +
			                   "  <port>"+ Configuration.Port.ToString () +"</port>\n" +
			                   "  <root>/var/logger</root>\n" +
			                   "  <requireauth>false</requireauth>\n" +
			                   "  <user>userdata</user>\n" +
			                   "</configuration>\n");
		}
		
		private static void PrintHelp()
		{
			Console.WriteLine("Usage: logger [-vh]\n\n" +
			                  "This is an advanced logging server. See https://github.com/benapetr/logger for more information.\n\n" +
			                  "Parameters:\n" +
			                  "  -v (--verbose): increase verbosity\n" +
			                  "  -h (--help): display help\n" +
			                  "  --print-conf: print a cofiguration file to screen\n" +
			                  "  -c --config-file file: load a configuration file at specified path\n");
		}
		
		private static void Read(string path)
		{
			Configuration.ConfigurationFile = path;
			Configuration.Read();
		}
		
		/// <summary>
		/// Parse the specified args.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public static bool Parse(string[] args)
		{
			List<string> parameters = new List<string>();
			parameters.AddRange(args);
			int i = 0;
			foreach(string xx in parameters)
			{
				if (xx.StartsWith("-v"))
				{
					Configuration.Verbosity++;
					int curr = 2;
					while (curr < xx.Length)
					{
						if (xx[curr] == 'v')
						{
							Configuration.Verbosity++;
						}
						curr++;
					}
				}else
				{
					switch(xx)
					{
					case "--verbose":
						Configuration.Verbosity++;
						break;
					case "-h":
					case "--help":
						PrintHelp();
						return true;
					case "-c":
					case "--config-file":
						if (parameters.Count <= i+1)
						{
							Console.WriteLine("Option --config-file is missing a parameter!");
							return true;
						}
						Read(parameters[i+1]);
						break;
					case "--print-conf":
						PrintConf();
						return true;;
					}
				}
				i++;
			}
			return false;
		}
	}
}
