using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	public class ValidationResult
	{
		public List<string> errorMessages;

		public ValidationResult()
		{
			errorMessages = new List<string>();
		}
		public void ReportValidationError(string s)
		{
			errorMessages.Add(s);
		}
	}
}
