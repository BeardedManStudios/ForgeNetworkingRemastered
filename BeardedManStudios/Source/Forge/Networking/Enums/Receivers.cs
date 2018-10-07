/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// This is often used in conjunction with RPC or WriteCustom in order to limit who gets the call
	/// </summary>
	public enum Receivers
	{
		Target = 0, // Used for direct messages to clients
		All = 1,
		AllBuffered = 2,
		Others = 3,
		OthersBuffered = 4,
		Server = 5,
		AllProximity = 6,
		OthersProximity = 7,
		Owner = 8,
		MessageGroup = 9,
		ServerAndOwner = 10,
        AllProximityGrid = 11,
        OthersProximityGrid = 12
    }
}