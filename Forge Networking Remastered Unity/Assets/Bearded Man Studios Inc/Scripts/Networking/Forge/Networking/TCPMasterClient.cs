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

using System;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// The purpose of this master client is for the master server to register
	/// </summary>
	public class TCPMasterClient : TCPClient
	{
		protected override void Initialize(string host, ushort port, bool pendCreates = false)
		{
            base.Initialize(host, port, pendCreates);
            base.InitializeTCPClient(host, port);
		}
	}
}
