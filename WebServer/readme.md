# Introduction to Forge Web Server
The Forge Web Server allows you to connect to, view information, run commands and manage a running forge server through your browser. The Forge Web Server can be run from a Unity instance or from any of the standalone forge instances. 

Fundamentally, the web server is a (REST-like) API. It sends an initial payload of html when a user first opens the url. All of the html is packed into this inital request and response. After this all of the updates to the page in the browser are done by modifying the html with javascript. To get live data from the server, the browser page requests to the Forge Web Server for specific pieces of data. You can add new pages and add new types of requests for the forge web server to handle.

Handling the web server in this way is very powerful indeed, as it is incredibly effecient compared to a standard web server and it also makes it a lot easier if you want to connect any other service on the internet to a Forge server.

Right now there is only the core system required to provide the service. It isn't stable enough for the addon to be used in a production but you can begin testing and working with the system now. And the default modules available in the legacy web server addon aren't yet available.
# How to add Forge Web Server to your project
Simple, build the project in visual studio.
**If you're using Unity**: Add the DLL to your Unity project.
**Otherwise**: Add the DLL as a reference to your project if you're not using this inside Unity.

Once you have the DLL in your project, make sure you're using the namespace.
```sh
using BeardedManStudios.WebServer;
```
Then you simply have to create a ForgeWebServer object and call the start method.
```sh
ForgeWebServer webserver = new ForgeWebServer(port);
webserver.Start();
```
# How to extend the Forge Web Server
Before getting started it's worth noting that it will help a lot if you have the following:
 - [Basic understanding of javascript (it's quite easy to pickup if you are familiar with another language).](http://www.w3schools.com/js/default.asp)
 - [Basic understanding of the steps involved in the HTTP protocol.](https://code.tutsplus.com/tutorials/http-the-protocol-every-web-developer-must-know-part-1--net-31177)
 - [Basic understanding of what an HTTP API is.](https://code.tutsplus.com/tutorials/a-beginners-guide-to-http-and-rest--net-16340)

##### Adding new pages to the Forge Web Server
In your project you should see a "www/modules/" directory, this is where the different pages listed across the top of the web server are read from. There should always be at least an "HTMLPayload.html" and a "Home.html" file in this directory which I will call *the modules directory*.

Then all you have to do is create a new .html file in this directory. You can open the "Home.html" file in the modules directory to get an example of how your html should look but here are requirements your html must follow:
 - Inside the body tags, you must have a div with a class='module' attribute (You can use either ' or " in the class attribute). If you don't have this the payload will fail to be setup and the web server will stop.
 - You can have custom javascript written inside a set of script tags either within the div class='module' tags or inside the head tags.
 - You cannot currently have multiple div class='module' inside one html file.

Once the file is added it will appear in the topbar and be able to be visited. This will only display the html and include the javascript from your file. If you want to continually update something in the html live from the server, you will need to create a new controller.
##### Adding new controllers/Adding new APIs to the Forge Web Server
Firstly, navigate to the "Controllers/" directory which I will call *the controllers directory*. You should see at least the "ApiController.cs" and "HomeController.cs" files. Create a new C# class in this directory. The name must end with "Controller".
Once this is done open your new class, make sure it inherits from the ApiController class. You will have to create a method that overrides the protected GetResponse method like so:
```sh
protected override string GetResponse() {
    return "Hello world";
}
```
Whatever this method returns in your class will be the output of this controller. To actually call the controller simply do an HTTP request to the following format:
```sh
{ip or domain name of your server}:{port of your server}/api/{controller name}
```
You can test it by opening the above url in your browser.
##### How to access your Controllers/API from the html in your module
Once you have a new module that you can visit in your browser and a controller. You can do an AJAX request using javascript or JQuery and apply the value to the page. For example:

Example HTML
```sh
<p>Number of players connected to server: <span id="playerCount">0</span></p>
```
Example Javascript
```sh
function getPlayerCount(){
    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementById("playerCount").innerHTML = this.responseText;
        }
    };
    request.open("GET", "/api/ConnectedPlayerCounter", true);
    request.send();
}
```
More coming on handling this elegantly...
# Known bugs
- Listener causes out of memory when leaving the server running.
- Names and CSS classnames need refactoring.

# To-Do list
 - Home page
 - Login page
 - Remote debug page
 - Example 'Spatializer' page
 - Console page reflection needs to not be handled at invocation time
 - Console page needs to be made more accessible and usable

# Dependencies
For anyone who is curious this doesn't introduce any new dependencies although it is likely you will want to use JSON.Net when extending the system.