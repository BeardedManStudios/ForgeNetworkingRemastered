# Web Server Command Plugins
As you know by now, the webserver comes with a fancy console which allows you to type in commands and execute some logic on your server. One thing that we thought would be needed is for you to have the ability to create your own commands.

## Create the plugin class
When creating a plugin for the webserver, you will need to derive from the `ICommandPlugin` interface which is in the `BeardedManStudios.Forge.MVCWebServer.Plugins` namespace, so be sure to add it to your using statements. Below you will see a code example of a plugin. This plugin must have the `Execute` method and it must return a `string` as well as take in a `string[]` of commands. If you typed into the command box **testing brent farris** the output would be **Hello Brent!**. However if you typed in **testing "Brent Farris"** the output would be **Hello Brent Farris!**. As you can see, the quotes will group parts of the command together. Lastly, parameters are not required (because of how we wrote our class) so **testing** by itself will respond with **HELLO!**.

```csharp
public class TestingPlugin : ICommandPlugin
{
    public string Execute(string[] commands)
    {
        if (commands == null || commands.Length == 0)
            return "HELLO!";

        return "Hello " + commands[0] + "!";
    }
}
```

## Add our plugin
Adding a plugin is very easy, let's say you have a web server running, all you need to do is call the `TryAddPlugin` method. Below is a couple examples of this in action.

#### Without help strings
```csharp
ForgeWebServer ws = new ForgeWebServer(server, 15942, dict);
ws.Start();

ws.TryAddPlugin("testing", new TestingPlugin());
```

#### With help strings
```csharp
ForgeWebServer ws = new ForgeWebServer(server, 15942, dict);
ws.Start();

ws.TryAddPlugin("testing", new TestingPlugin(), "Do a test log", "testing brent", "arg1", "arg2", "arg3", "...");
```

When you supply help strings, then when you type **help** into the command box, it will print out the help messages to let the user know what the command does and what arguments it expects.

## Done
Well that is it, once you create the command class and implement the `ICommandPlugin` interface, then do a call to `TryAddPlugin` with your plugin class, then you are ready to start using the command in the web browser.

### Notes
You can create and add plugins to your web server at any time in your code. So you can potentially create commands to spawn objects, kill player characters, render a cube for no reason, pretty much anything you can imagine.