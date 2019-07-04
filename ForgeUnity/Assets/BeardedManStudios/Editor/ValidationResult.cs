using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	public class ValidationResult
	{
		public bool Result { get; private set; }
		public List<string> errorMessages;

		public ValidationResult()
		{
			Result = true;
			errorMessages = new List<string>();
		}
		public void ReportValidationError(string s)
		{
			Result = false;
			errorMessages.Add(s);
		}

        ///<summary>
        ///Package a ValidationResult inside this validation result
        ///</summary>
        public void Capture(ValidationResult v)
        {
            Result = v.Result && Result;
            errorMessages.AddRange(v.errorMessages);
        }
	}
}
