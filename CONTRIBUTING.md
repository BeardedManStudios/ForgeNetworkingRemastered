# Contributing
:heart::+1: **Thank you so much for taking the time to contribute!** :+1::heart:

## Code standards
We have a worldwide community and respect everyones backgrounds and languages, however Forge Networking is primarily developed by English (US) speakers. Because of this the variable names and comments for this code base should be exclusively in English (US).

**Anything not listed here should follow the [C# standard](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)**

### Tabs vs Spaces?
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

For lambda expressions we have hanging brackets
```csharp
Call(() =>
{
    InnerCode();
});
```

For initializer lists we have hanging brackets
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
