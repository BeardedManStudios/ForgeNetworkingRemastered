namespace BeardedManStudios.Forge.MVCWebServer.Plugins
{
    public interface ICommandPlugin : IWebserverPlugin
    {
        string Execute(string[] commands);
    }
}