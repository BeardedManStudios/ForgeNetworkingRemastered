/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

using System.Collections.Generic;

namespace BeardedManStudios
{
	// A simple dummy class for parsing the string arguments of a generic program, including flags
	public static class ArgumentParser
	{
		/// <summary>
		/// Parse a string array for the various arguments that were supplied
		/// </summary>
		/// <param name="args">The arguments that were sent to the program</param>
		/// <returns>A dictionary of arguments with a key being the index or the flag starting with "-"</returns>
		public static Dictionary<string, string> Parse(string[] args)
		{
			Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

			int floatingArgIndex = 0;
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("--"))
					parsedArgs.Add(args[i], args[i]);
				else if (args[i].StartsWith("-"))
					parsedArgs.Add(args[i++], args[i]);
				else
					parsedArgs.Add((floatingArgIndex++).ToString(), args[i]);
			}

			return parsedArgs;
		}
	}
}