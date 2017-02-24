# Network Logging

Forge Networking has it's own logging system built in for the purposes of debugging and tracing any errors that may happen during network communication. When an exception is thrown on the networking layer, the logging system will pick that up and process the log. There are currently 2 main forms of logging that is built into the system, the in-game logging and the file system logging. We do not recommend using the in-game logging for any production environment, however we do suggest that you enable file logging for production environments.

The Forge Networking logger will only log network exceptions so it will not have any impact on performance, except if there are many exceptions being thrown (which shouldn't happen unless there is a critical process error in the game). This log does a little more than just log network exceptions however; it also logs exceptions that are thrown by your code during the network transport time. Things such as an invalid parameter to an RPC or an invalid cast being performed by your code. Because of this, we recommend always having this log available when testing your code in order to reduce support tickets on our end and in order to keep you as productive as possible towards your awesome product!

**How To Enable Logging**

1. Navigate to Bearded Man Studios IncScripts->Logging->ResourcesBMSLogger
2. Having the BMSLogger selected, you can then check 'Log To File'

This will then allow you to log all exceptions and common networking debug logs into the logging folder (including logs that you write).

**How to Enable Visible Logging**

Note: This will allow you to see the logs in the build itself on any device.

1. Navigate to Bearded Man Studios IncScripts->Logging->ResourcesBMSLogger
2. Having the BMSLogger selected, you can then check 'Logger Visible'

The final location of this log file will be dependent on whether it is in the editor or a build.

Editor location: Root Unity DirectoryAsset Logsbmslog.txt

Windows Build Location: ExecutableDirectoryExecutableName\_DataLogsbmslog.txt

**Using the Logger yourself for development purposes**

### Example Logging
```csharp
BeardedManStudios.Forge.Logging.BMSLog.Log("ANYTHING YOU WANT TO LOGHERE!");
BeardedManStudios.Forge.Logging.BMSLog.LogFormat("FOLLOWING A FORMAT[{0}]", "ANYTHING YOU WANT TO LOG HERE!");
BeardedManStudios.Forge.Logging.BMSLog.LogWarning("ANYTHING YOU WANT TO LOGHERE!");
BeardedManStudios.Forge.Logging.BMSLog.LogWarningFormat("FOLLOWING A FORMAT[{0}]", "ANYTHING YOU WANT TO LOG HERE!");
BeardedManStudios.Forge.Logging.BMSLog.LogException([System.Exception]); 
BeardedManStudios.Forge.Logging.BMSLog.LogException("ANYTHING YOU WANT TOLOG HERE!");
BeardedManStudios.Forge.Logging.BMSLog.LogExceptionFormat("FOLLOWING AFORMAT [{0}]", "ANYTHING YOU WANT TO LOG HERE!");```

Above is code examples of how to call the logger to use it for logging purposes.

Note: All exceptions will automatically be logged, so put them in places that should never be called frequently, useful for testing on builds and figuring out what went wrong on the network.