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

using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

namespace logger
{
	public class Configuration
	{
		/// <summary>
		/// The version.
		/// </summary>
		public static string Version = "1.0.0";

		/// <summary>
		/// The port.
		/// </summary>
		public static int Port = 64385;

		/// <summary>
		/// The verbosity.
		/// </summary>
		public static int Verbosity = 0;

		/// <summary>
		/// The storage.
		/// </summary>
		public static StorageType Storage = StorageType.file;

		/// <summary>
		/// root
		/// </summary>
		public static string Root = null;

		/// <summary>
		/// password.
		/// </summary>
		public static string Password = null;

		/// <summary>
		/// The configuration file.
		/// </summary>
		public static string ConfigurationFile = null;

		public enum StorageType
		{
			file,
			mysql,
		}

		public static void Read()
		{
			if (ConfigurationFile == null)
			{
				return;
			}
			
			if (!File.Exists(ConfigurationFile))
			{
				MainClass.Log("There is no config file");
				return;
			}
			
			XmlDocument file = new XmlDocument();
			file.Load(ConfigurationFile);
			
			foreach (XmlNode item in file.ChildNodes[0])
			{
				switch(item.Name.ToLower())
				{
				case "root":
					Configuration.Root = item.InnerText;
					break;
				case "port":
					Configuration.Port = int.Parse(item.InnerText);
					break;
				}
			}
		}
	}
}

