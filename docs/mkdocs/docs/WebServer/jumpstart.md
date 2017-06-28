# Web Server Jumpstart
Getting up and running with the web server is easier than ever. Just include the plugin into your project, new up the object and start/stop the server at will. The namespace for the web server is `BeardedManStudios.Forge.MVCWebServer`, so if you need to include using statements or view things in the object browser, take a look in this namespace.

## Including plugin into project
You will notice a **MVCWebServer.dll** file in the unzipped folder for the web server. You just need to copy this file into your **Bearded Man Studios Inc/Plugins** folder. That is it, you've installed the web server plugin to your project.

## Starting a Web Server
Starting a web server is as simple as calling 2 lines of code as seen below.

```csharp
ForgeWebServer ws = new ForgeWebServer(server, pages, ForgeWebServer.DEFAULT_PORT);
ws.Start();
```

I know what you are saying, "what is server... and what are pages?". Good questions; see below:
- **server**: This is the `NetWorker` for the server
- **pages**: This is a `Dictionary<string, string>()` of web pages (html). The key string is the name of the page, like **"index"** and the value is the html of the page. The pages you add here will be viewable from the web interface. So let's say you add a page **cat** and the html `<h1>Hello World</h1>`. Then if you went to `locahost:15942/cat` in any web browser, you would see a big bold **Hello World** heading on the page.

We have added a few default pages as part of the package in the **Resources** folder. If you want a magical function that automatically maps all the pages you put into this folder, well... here you go:

```csharp
string pathToFiles = "fnwww/html";
Dictionary<string, string> pages = new Dictionary<string, string>();
TextAsset[] assets = Resources.LoadAll<TextAsset>(pathToFiles);
foreach (TextAsset a in assets)
    pages.Add(a.name, a.text);

ForgeWebServer ws = new ForgeWebServer(server, pages, ForgeWebServer.DEFAULT_PORT);
ws.Start();

// ...
```

## Stopping a Web Server
When you are closing your connection/application that has a running web server, you will want to close the connection for the web server as well. You can see how to close the connection to the web server below:

```csharp
ForgeWebServer ws = new ForgeWebServer(server, pages, ForgeWebServer.DEFAULT_PORT);
ws.Start();

// ...

ws.Stop();
```

## Notices & Warnings!
**STOP, BEFORE YOU LEAVE THIS PAGE!** There are a few things to keep in mind when using the web server.
1. If you disconnect your server, you **MUST** disconnect your web server as well. Since this is a plugin system, you need to remember to do this.
2. You **MUST** remember to close the web server on application quit.
3. The commands are run on a separate thread so you need to use [MainThreadManager](/UnityIntegration/running-unity-specific-code-on-the-main-thread.md) for any code that requires to be on the main thread