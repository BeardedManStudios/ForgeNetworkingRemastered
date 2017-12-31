# Extending Generated Classes

You may have noticed by now that Forge Networking Remastered (FNR) will generate network code through the Network Contract Wizard (NCW) . The main purpose of this is to completely remove reflection and make the system easier to test and debug. The generated code will hook into the Forge Networking core framework and setup hooks and connections so that you, as the user of the API, does not have to do all that repetitive work and you can just focus on developing your product.

If you have looked through the generated code you may have seen that the classes that are generated are **partial** classes. If you are unfamiliar with partial classes, we would like to invite you to review the standard [MSDN C# documentation](https://msdn.microsoft.com/en-us/library/wa80x488.aspx) for **partial classes**. If you are familiar with the concept of partial classes then you are already a step closer to being able to easily extend the generated code.

In many cases it would be advantageous for us to be able to **add more** code to generated code. This is because we may have multiple different objects inheriting or using the classes that are generated. In this scenario, it would not make sense to write another class and make sure to remember to attach it to every object that uses the class in question. There are scenarios where you would just like to be able to manually add more options to a generated class for further use or for utility sake. To do this, all you need to do is create a new class with the same name as the target class in question. Then make sure to place the keyword **partial** just before you type the **class** keyword.

```csharp
public partial class MyClass //…
```

Partial classes are essentially compiled together into one combined class, so it is like editing the source file without having to actually change the source file.