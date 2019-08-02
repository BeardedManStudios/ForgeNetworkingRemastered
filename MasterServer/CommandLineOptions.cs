using CommandLine;
using CommandLine.Text;

namespace MasterServer
{
	class CommandLineOptions
	{
		[Option('p', "port", DefaultValue = "15940", HelpText = "The port the server will listen on")]
		public string Port { get; set; }

		[Option('h', "host", DefaultValue = "0.0.0.0", HelpText = "The ip address the server will listen on")]
		public string Host { get; set; }

		[Option('e', "elorange", DefaultValue = 0)]
		public int EloRange { get; set; }

		[Option('d', "daemon", DefaultValue = false)]
		public bool IsDaemon {get; set;}

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
