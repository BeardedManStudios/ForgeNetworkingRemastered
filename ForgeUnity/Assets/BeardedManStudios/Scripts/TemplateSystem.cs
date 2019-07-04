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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BeardedManStudios.Templating
{
	public sealed class TemplateSystem
	{
		private string targetTemplate = string.Empty;
		private Dictionary<string, object> replaces = new Dictionary<string, object>();
		private enum State
		{
			None = 0x0,
			ForEach = 0x1,
			ForEvery = 0x2
		}
		private State state = State.None;
		private object[] iteratee;
		private int currentIterateeIndex = 0;
		private int loopStart = -1;
		private bool emptyArray = false;

		public TemplateSystem(string text)
		{
			targetTemplate = text;
		}

		public void AddVariable(string key, object val)
		{
			replaces.Add(key, val);
		}

		public string Parse()
		{
			List<string> lines = new List<string>(targetTemplate.Replace("\r\n", "\n").Split('\n'));

			List<string> finalLines = new List<string>(lines.Count);

			int offset = 0;
			bool parsed = false;
			bool skipLine = false;
			State foundState = State.None;
			StringBuilder sb;
			for (int i = 0; i < lines.Count; i++)
			{
				parsed = false;
				skipLine = false;
				sb = new StringBuilder(lines[i]);

				while (true)
				{
					string current = sb.ToString();
					int parseStart = current.IndexOf(">:", offset);
					if (parseStart < 0)
						break;

					parseStart += 2;
					int parseEnd = current.IndexOf(":<", offset + parseStart);

					if (parseEnd < 0)
						throw new Exception("There was a parse start but no end on line " + (i + 1));

					string contents = current.Substring(parseStart, parseEnd - parseStart);

					sb.Remove(parseStart - 2, parseEnd - parseStart + 4);

					if (CheckState(contents, ref foundState))
					{
						skipLine = true;

						// If we have left the loop
						if (foundState == State.None)
						{
							if (loopStart == -1)
								continue;

							if (++currentIterateeIndex >= iteratee.Length)
							{
								state &= ~(State.ForEach | State.ForEvery);
								iteratee = null;
								loopStart = -1;
								emptyArray = false;
							}
							else
							{
								i = loopStart - 1;
								break;
							}
						}
						else if (foundState == State.ForEach || foundState == State.ForEvery)
							loopStart = i + 1;

						continue;
					}

					if (!emptyArray)
						sb.Insert(parseStart - 2, ParseLine(contents));

					parsed = true;
				}

				string built = sb.ToString();

				if (parsed && built.Trim().Length == 0)
					lines.RemoveAt(i--);
				else if (!skipLine && !emptyArray)
					finalLines.Add(built);
			}

			return string.Join(Environment.NewLine, finalLines.ToArray());
		}

		private bool CheckState(string contents, ref State foundState)
		{
			if (contents.StartsWith("ENDFOREACH"))
			{
				if ((state & State.ForEach) == 0)
					throw new Exception("A foreach has ended before the start of the loop");

				foundState = State.None;
				return true;
			}
			if (contents.StartsWith("ENDFOREVERY"))
			{
				if ((state & State.ForEvery) == 0)
					throw new Exception("A foreach has ended before the start of the loop");

				foundState = State.None;
				return true;
			}
			else if (contents.StartsWith("FOREACH"))
			{
				if ((state & State.ForEach) != 0 || (state & State.ForEvery) != 0)
					throw new Exception("A loop is already in execution and in this version nested foreach loops are not allowed");

				state |= State.ForEach;

				string iterateeName = contents.TrimStart("FOREACH ".ToCharArray());

				if (!replaces.ContainsKey(iterateeName))
					throw new Exception("No variable with the name " + iterateeName + " could be located");

				iteratee = (object[])replaces[iterateeName];

				if (iteratee.Length == 0)
					emptyArray = true;

				currentIterateeIndex = 0;
				foundState = State.ForEach;
				return true;
			}
			else if (contents.StartsWith("FOREVERY"))
			{
				if ((state & State.ForEach) != 0 || (state & State.ForEvery) != 0)
					throw new Exception("A loop is already in execution and in this version nested foreach loops are not allowed");

				state |= State.ForEvery;

				string iterateeName = contents.TrimStart("FOREVERY ".ToCharArray());

				if (!replaces.ContainsKey(iterateeName))
					throw new Exception("No variable with the name " + iterateeName + " could be located");

				iteratee = (object[])replaces[iterateeName];

				if (iteratee.Length == 0 || ((object[])iteratee[0]).Length == 0)
					emptyArray = true;

				currentIterateeIndex = 0;
				foundState = State.ForEvery;
				return true;
			}

			return false;
		}

		private string ParseLine(string contents)
		{
			if (contents.StartsWith("[") && contents.EndsWith("]"))
			{
				if (contents == "[i]" && iteratee != null)
					return FormatReturn(iteratee[currentIterateeIndex]);
				else if (contents == "[idx]" && iteratee != null)
					return FormatReturn(currentIterateeIndex);
				else
				{
					var idxStr = contents.TrimStart('[').TrimEnd(']');
					int idx = -1;
					if (int.TryParse(idxStr, out idx))
						return FormatReturn(((object[])iteratee[currentIterateeIndex])[idx]);
					else
						throw new Exception("The index " + idxStr + " is not an integer");
				}
			}
			else if (contents == "ELSEIF")
			{
				if (currentIterateeIndex == 0)
					return "if";
				else
					return "else if";
			}

			if (replaces.ContainsKey(contents))
				return FormatReturn(replaces[contents]);

			return string.Empty;
		}

		private string FormatReturn(object data)
		{
			if (data is bool)
				return data.ToString().ToLower();
			else if (data is float)
			{
				float fData = (float) data;
				return fData.ToString(CultureInfo.InvariantCulture) + "f";
			}
			return data.ToString();
		}
	}
}