# Contributing
:heart::+1: **Thank you so much for taking the time to contribute!** :+1::heart:

## Code standards
We have a worldwide community and respect everyones backgrounds and languages, however Forge Networking is primarily developed by English (US) speakers. Because of this the variables, names, functions, comments, and everything else for this code base should be exclusively in English (US).

### .editorconfig
We use .editorconfig to help with conforming to our coding conventions. Please make sure your chosen editor has this enabled. Check [editorconfig.org](https://editorconfig.org/#download) to see if you need to install a plugin or your IDE has this built in.

**Anything not listed here should follow the [C# standard](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)**

### Tabs vs spaces?
Sorry space fans, we are tabs. Though I know that this is an age old argument in computer science, we had to go with one or the other.

### Accessors
Yes, we are aware that if you do not provide an accessor that it will default to `private`. However, we like knowing that others are doing what they do on purpose. For this reason you are required to put `private` in front of your private members.

Member fields are to be camel case starting with an alpha character. For the most part people don't write C# in notepad so do not worry about any prefixes with underscores, or special things like `b_` or any of that
```csharp
public int myNumber;
protected bool canHas;
private string catName;
```

Member properties are to be pascal case starting with an alpha character. Also their brackets are to be on 1 line if the behavior is simple. Otherwise you are to use the hanging brackets coding standards
```csharp
public int MyNumber { get; private set; }
public bool CanHas { get { return MyNumber > 0; } }
private string CatName
{
    get
    {
        if (MyNumber > 5)
            return "Pickles";
        else if (CanHas)
            return "Sure";
        else
            return "George?";
    }
}
```

### Brackets
For single line conditional statements we do not have any brackets, just the line. Preferrably we put the code on the line following the if statement but for simple things like a return a single line is acceptable
```csharp
if (1 == 1)
    DoSomethingFancy();
```

For multiple line conditional statements we put "hanging brackets" only"
```csharp
if (1 == 1)
{
    FirstThing();
    LastThing();
}
```

For infinite loops, while loops, and do-while loops, we need these to be exceptionally clear. So in the case of any of these we will use hanging brackets.
```csharp
while (true)
{
    DoSomethingFancy();
}

do
{
    DoSomethingFancy();
} while(true);

for(;;)
{
    DoSomethingFancy();
}
```

For lambda expressions we have hanging brackets
```csharp
Call(() =>
{
    InnerCode();
});
```

For initializer lists we have hanging brackets (no parenthisis needed)
```csharp
MyClass thing = new MyClass
{
    a = 1,
    b = 2,
    c = 3
};
```

### Comments
Single line comments should include a space after the slashes
```csharp
// Notice the space after the two slashes
```

Commenting out code should not include a space after the slashes
```csharp
//noSpace.Code();
```

TODO comments should include the space after the slashes and then 2 spaces after the colon following the `TODO`
```csharp
// TODO:  Notice the space after the slashes and the 2 spaces after the colon of todo
```

### Line length
We try to keep our line lengths short, try to stick around 90 characters maximum in the horizontal space and do not go any further than 120 characters. Or goals are to prevent any kind of horizontal scrolling or automatic line wrapping.

### Var vs type name
Though we accept code with `var` being used, it is hard to code review things listed as just `var` especially if they are the result of a function. If you are creating a new instance of something you can use `var` since the type name is in the line of declariation, otherwise please try to type the explicit type of the variable when it is the result of a function.

### Function length
We prefer very small, descriptive functions over long monolythic functions. Try to keep your functions to something around 4-9 lines. If your functions are longer than 9 lines, it probably is a good chance for breaking it into smaller local private methods or turning what is being processed into a `class` and handling the logic more inside of there. This is not only for code clarity and readability, but also it makes it much easier to debug knowing the names of functons and produces clear stack traces.

### Testing and TDD
We are currently trying out the Unity unit testing tools and they look promising so far. My hope is that we will be able to fall more into the TDD ([test driven development](https://en.wikipedia.org/wiki/Test-driven_development)) flow of programming so that we have many tests for the code to make sure it continues to work as expected for all of the functionality. Of course, I know it is very difficult to unit test a multi-threaded, network based application, but it is important for us to try as much as possible to achieve a full test coverage of the system.

Please make sure that you either create a new file in the [ForgeUnity/Assets/BeardedManStudios/Scripts/Editor/Tests](https://github.com/BeardedManStudios/ForgeNetworkingRemastered/tree/develop/ForgeUnity/Assets/BeardedManStudios/Scripts/Editor/Tests) folder. Or if you are adding new tests that are either missing or covering new functionality of a class, feel free to update existing tests.

### Naming
When contributing to Forge, we want to make sure to convey the ideas through names. One of the easiest ways to do so is to describe the design pattern you are attempting to use in the code as part of the name. Below is a list of design pattern names (with personal descriptions) that are you free to use.

<strong>Abstract factory</strong> - Provide an interface for creating families of related or dependent objects without specifying their concrete classes.

<strong>Builder</strong> - Separate the construction of a complex object from its representation so that the same construction process can create different representations.

<strong>Factory Method</strong> - Define an interface for creating an object, but let subclasses decide which class to instantiate. Factory Method lets a class defer instantiation to subclasses.

<strong>Prototype</strong> - Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.

<strong>Singleton</strong> - Ensure a class only has one instance, and provide a global point of access to it.

<strong>Adapter</strong> - Convert the interface of a class into another interface clients expect. Adapter lets classes work together that couldn't otherwise because of incompatible interfaces.

<strong>Bridge</strong> - Decouple an abstraction from its implementation so that the two can vary independently.

<strong>Composite</strong> - Compose objects into tree structures to represent part-whole hierarchies. Composite lets clients treat individual objects and compositions of objects uniformly.

<strong>Decorator</strong> - Attach additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality.

<strong>Façade</strong> - Provide a unified interface to a set of interfaces in a subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.

<strong>Flyweight</strong> - Use sharing to support large numbers of fine-graned objects efficiently.

<strong>Proxy</strong> - Provide a surrogate or placeholder for another object to control access to it.

<strong>Repository</strong> - The abstraction of data storage to allow for multiple different implementations where only one is selected but not known about by the repository user.

<strong>Chain of responsibility</strong> - Avoid coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Chain the receiving objects and pass the request along the chain until an object handles it.

<strong>Command</strong> - Encapsulate a request as an object, thereby letting you parameterize clients with different requests, queue or log requests, and support undoable operations.

<strong>Interpreter</strong> - Given a language, define a representation for its grammar along with an interpreter that uses the representation to interpret sentences in the language.

<strong>Iterator</strong> - Provide a way to access the elements of an aggregate object sequentially without exposing its underlying representation.

<strong>Mediator</strong> - Define an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently.

<strong>Memento</strong> - Without violating encapsulation, capture and externalize an object's internal state so that the object can be restored to this state later.

<strong>Observer</strong> - Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.

<strong>State</strong> - Allow an object to alter its behavior when its internal state changes. The object will appear to change its class.

<strong>Strategy</strong> - Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

<strong>Template method</strong> - Define the skeleton of an algorithm in a operation, deferring some steps to subclasses. Template Method lets subclasses redefine certain steps of an algorithm without changing the algorithm's structure.

<strong>Visitor</strong> - Represent an operation to be performed on the elements of an object structure. Visitor lets you define a new operation without changing the classes of the elements on which it operates.

<strong>Unit of work</strong> - This is a class that is responsible for keeping running modifications to commit to a repository in memory. When ready, the unit of work can be committed all in a single transaction. A unit of work can be thought of as a ledger/transaction for work done by a single request
