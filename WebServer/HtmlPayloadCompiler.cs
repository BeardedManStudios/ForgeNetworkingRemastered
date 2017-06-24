using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.WebServer;

namespace BeardedManStudios.WebServer
{
    public class HtmlPayloadCompiler
    {
        private const string html_payload_filename = "HTMLPayload.html";
        private string raw_payload, combined_html = "", combined_scripts = "", combined_styles = "";
        private bool compilerMarksRemoved = false;

        public string[] compiled_file_names { get; private set; }

        public HtmlPayloadCompiler(params string[] directories)
        {
            if (!Directory.Exists("www") || !Directory.Exists("www/modules"))
                throw new ForgeWebServerException("Cannot create HtmlPayloadCompiler - missing www/modules directory!");

            if(!File.Exists("www/modules/"+ html_payload_filename))
                throw new ForgeWebServerException("Cannot create HtmlPayloadCompiler - the \"" + html_payload_filename + "\" doesn't exist in the www/modules directory!");

            raw_payload = File.ReadAllText("www/modules/"+html_payload_filename);

            foreach (string directory in directories)
            {
                compileDirectory(directory);
            }
        }

        public void compileDirectory(string path)
        {
            if(!Directory.Exists(path))
                throw new ForgeWebServerException("Attempted to compile a directory that doesn't exist");
            if(compilerMarksRemoved)
                throw new ForgeWebServerException("Attempted to compile a directory, but the compiler has already finished");

            string[] files_in_dir = Directory.GetFiles(path);

            for (int i = 0; i < files_in_dir.Length; i++)
            {
                if (compileHtmlFile(files_in_dir[i]))
                    files_in_dir[i] = convertPathToFile(files_in_dir[i]);
                else
                    files_in_dir[i] = "";
            }

            files_in_dir = bringToFrontOfList("Home", files_in_dir);
            addFileNames(files_in_dir);
            
            raw_payload = raw_payload.Replace("{modules}", combined_html + "{modules}");
            raw_payload = raw_payload.Replace("{scripts}", combined_scripts + "{scripts}");
            raw_payload = raw_payload.Replace("{styles}", combined_styles + "{styles}");
        }

        private void combinePayload()
        {
            raw_payload = raw_payload.Replace("{modules}", "");
            raw_payload = raw_payload.Replace("{scripts}", "");
            raw_payload = raw_payload.Replace("{styles}", "");
            compilerMarksRemoved = true;
        }

        public string getPayload()
        {
            if (!compilerMarksRemoved)
                combinePayload();
            return raw_payload;
        }

        private void addFileNames(string[] input_array)
        {
            if(compiled_file_names == null)
                compiled_file_names = new string[0];

            string[] new_filename_array = new string[compiled_file_names.Length + input_array.Length]; 
            compiled_file_names.CopyTo(new_filename_array, 0);
            input_array.CopyTo(new_filename_array, compiled_file_names.Length);
            compiled_file_names = new_filename_array;
        }

        /// <summary>
        /// This method adds the html file's content to the combined_html buffer and the file's scripts to the combined_script buffer.
        /// </summary>
        /// <param name="path">Location of the file to compile</param>
        /// <returns>Returns true if the files is compiled successfully and false if the file fails to compile</returns>
        private bool compileHtmlFile(string path)
        {
            string filename = convertPathToFile(path);
            if (!path.Contains(".html") || !File.Exists(path) || path.Contains(html_payload_filename))
                return false;

            string content = File.ReadAllText(path);
            string body_content = content.Split(new[] {"<body>", "</body>"}, StringSplitOptions.None)[1];
            
            body_content = body_content.Replace("<div class='module'>", "<div class=\"module\">");
            if (!body_content.Contains("<div class=\"module\">"))
                return false;

            body_content = body_content.Replace("class=\"module\"", string.Format("class=\"module\" style=\"display:none;\" id=\"{0}-module\"", filename));

            combined_html += body_content;
            if (content.Contains("<script>") && content.Contains("</script>"))
                combined_scripts += content.Split(new[] { "<script>", "</script>" }, StringSplitOptions.None)[1];

            if (content.Contains("<style>") && content.Contains("</style>"))
                combined_styles += content.Split(new[] { "<style>", "</style>" }, StringSplitOptions.None)[1];
            return true;
        }

        private string convertPathToFile(string path)
        {
            return path.Split(new[] { "/", "\\", ".html" }, StringSplitOptions.RemoveEmptyEntries).Last();
        }
        
        private string[] bringToFrontOfList(string target, string[] input_array, int index = 0)
        {
            List<string> collection = new List<string>(input_array);
            collection.Remove(target);
            collection.Insert(0, target);
            collection.Remove("");
            return collection.ToArray();
        }
    }
}
