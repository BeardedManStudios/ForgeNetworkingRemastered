using System;

namespace BeardedManStudios.Forge.Networking.Generated
{
    [AttributeUsage(AttributeTargets.All)]
    public class GeneratedRPCVariableNamesAttribute : System.Attribute
    {
        public readonly string JsonData;

        public GeneratedRPCVariableNamesAttribute(string data)
        {
            this.JsonData = data;
        }

        public override string ToString()
        {
            return string.Format("d:{0}", JsonData);
        }
    }
}
