using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BeardedManStudios
{
	/// <summary>
	/// A simple dummy class for parsing the string arguments of a generic program, including flags
	/// </summary>
	public static class ArgumentParser
	{
		/// <summary>
		/// Matches:
		///		-key
		///		-key=value
		///		--key
		///		--key=value
		/// </summary>
		private const string ARGUMENT_PATTERN = @"^-{1,2}(?'key'[^\s=]+)(=(?'value'[^\s-=]+))?$";

		/// <summary>
		/// Parse a string array for the various arguments that were supplied.
		/// For example:
		///		["--a=1", "-b=2", "--c", "-d", "e", "f", "g"]
		/// Results in:
		///		{"a":"1", "b":"2", "c":null, "d":null, "0":"e", "1":"f", "2":"g"}
		/// </summary>
		/// <param name="args">The arguments that were sent to the program</param>
		/// <returns>
		/// A dictionary of arguments with the key being the flag (or the index for floating args)
		/// and the value being the associated value (or argument for floating args).
		/// </returns>
		public static Dictionary<string, string> ParseArguments(params string[] args)
		{
			Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

			int floatingArgIndex = 0;
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];

				string value;
				string key = ParseArgument(arg, out value) ?? floatingArgIndex++.ToString();

				parsedArgs[key] = value;
			}

			return parsedArgs;
		}

		/// <summary>
		/// Given an input argument in one of the following formats:
		/// 	-key
		///		-key=value
		///		--key
		///		--key=value
		///		value
		/// 
		/// Produces the following key/value pair:
		///		{"key", null}
		///		{"key", "value"}
		///		{"key", null}
		///		{"key", "value"}
		///		{null, "value"}
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string ParseArgument(string arg, out string value)
		{
			if (arg == null)
				throw new ArgumentNullException("arg");

			value = arg;

			Match match = Regex.Match(arg, ARGUMENT_PATTERN);
			if (!match.Success)
				return null;

			value = match.Groups["value"].Value;
			return match.Groups["key"].Value;
		}
	}
}
