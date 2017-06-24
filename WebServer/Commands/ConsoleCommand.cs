using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios.WebServer.Commands
{
    public class ForgeWebServerCommandException : BaseNetworkException
    {
        public ForgeWebServerCommandException(string message) : base(message)
        {

        }
    }

    abstract class ConsoleCommand
    {
        public bool constructCommand(string[] input)
        {
            FieldInfo[] fields = this.GetType().GetFields();
            if (input.Length != fields.Length)
                return false;
            object[] typeParsedInput;
            if (!getTypeParsedParams(this.GetType(), input, out typeParsedInput))
                return false;
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].SetValue(this, typeParsedInput[i]);
            }
            return true;
        }

        public static bool checkInputCanParse(string input, Type parse_target, out object result)
        {
            if (parse_target == typeof(string))
            {
                result = input;
                return true;
            }
            else if (parse_target == typeof(int))
            {
                int parsed_input;
                if (Int32.TryParse(input, out parsed_input))
                {
                    result = parsed_input;
                    return true;
                }
            }
            else if (parse_target == typeof(float))
            {
                float parsed_input;
                if (Single.TryParse(input, out parsed_input))
                {
                    result = parsed_input;
                    return true;
                }
            }
            else if (parse_target == typeof(bool))
            {
                bool parsed_input;
                if (Boolean.TryParse(input, out parsed_input))
                {
                    result = parsed_input;
                    return true;
                }
            }
            result = input;
            return false;
        }
        
        public bool getTypeParsedParams(Type command, object[] args, out object[] typeParsedResult)
        {
            List<object> routine_params = new List<object>();
            FieldInfo[] fields = command.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object parsed_obj;
                if (checkInputCanParse((string)args[i], fields[i].FieldType, out parsed_obj))
                {
                    routine_params.Add(parsed_obj);
                }
                else
                {
                    typeParsedResult = null;
                    return false;
                }
            }
            typeParsedResult = routine_params.ToArray();
            return true;
        }

        public abstract string runCommand();
    }
}
