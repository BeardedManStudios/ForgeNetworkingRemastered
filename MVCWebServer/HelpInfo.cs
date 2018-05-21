using System;

namespace BeardedManStudios.Forge.MVCWebServer
{
    internal class HelpInfo : Attribute
    {
        public string description;
        public string example;
        public string[] parameterInfo;

        public HelpInfo(string description, string[] parameterInfo, string example)
        {
            this.description = description;
            this.parameterInfo = parameterInfo;
            this.example = example;
        }

        public override string ToString()
        {
            string response = description;
            if (parameterInfo != null && parameterInfo.Length > 0)
            {
                response += "<br />Parameters:<ol>";

                foreach (string p in parameterInfo)
                    response += "<li>" + p + "</li>";

                response += "</ol>";
            }
            else
                response += "<br />";

            response += "Example: <i>" + example + "</i>";

            return response;
        }
    }
}