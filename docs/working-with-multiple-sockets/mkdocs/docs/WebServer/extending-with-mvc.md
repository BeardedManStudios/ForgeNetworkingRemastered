# Web Server Extending With MVC
Yes, the web server is a full blown website in it's own right. Being this, you may want to add new controllers and views to the site. We'll start by making a controller named "Test". This will make it so that you can go to **localhost:15942/test** and have your own custom view processed and shown.

#### Our custom "Test" controller
```csharp
public class Test : PageController
{
    public Test(NetWorker socket, ForgeWebServer webserver)
        : base(socket, webserver)
    {
        // Key string used in html like this: <% print %>
        variables.Add("print", Print);
    }

    public string Print()
    {
        return "HELLO TEST STRING ACTION!";
    }

    public override string Render()
    {
        return PresentHTML("test");
    }
}
```

As you can see from above, there are a lot of things going on, so we'll break them down one by one.
1. You **must** derive your new controller from the `PageController` class
2. You **must** create the constructor for your controller with the supplied input arguments
3. You should add any model pulling functions inside the constructor to the variables
4. You should create methods that are going to return the data needed for the variables
5. You **must** override the render method as it is abstract. This is where you return the HTML for your page. You can use the built in `PresentHTML` method you get from the parent class and pass in the name of your HTML page that will be presented.

When you add callbacks assigned to a string to `variables` as seen above with **print**, you are making it so that you can use the `string` response from those callbacks within your HTML page. See the **test.html** example below to see how it is used in the `<% %>` tags.

Now that you have your new controller, you are probably curious on how you can get it to be used while the web server is running. Below you will notice the line that has `TryAddController` which will try to add your new controller to the web server. Notice that all we have to do is new up the controller and pass it into this method.

#### Adding your new controller
```csharp
ForgeWebServer ws = new ForgeWebServer(server, 15942, dict);
ws.Start();

ws.TryAddController(new Test(server, ws));
```

Finally, we are ready to create our view. A view is something that the controller will present to the user when they request your controllers web page. We will name this file **test.html** because we are passing **test** into the `PresentHTML` of our controller. If we were to name this html file **cats** then we would need to update the controller to pass **"cats"** into the `PresentHTML` method of the `Render` function.

#### Our "test.html" view
```html
<html>
<head>
    <title>My Test Page</title>
</head>
<body>
    <h1>Hello <% print %></h1>
</body>
</html>
```

**Note if you are not sure what the HTML page is, be sure to check out the [Jumpstart Guide](jumpstart.md)**

## Done
That's it, you have your fancy new controller and view and you can start using it immediately inside of your web server.