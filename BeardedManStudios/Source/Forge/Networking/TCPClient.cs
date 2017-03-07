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

using BeardedManStudios.Threading;
using System;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	public class TCPClient : TCPClientBase
	{
		protected override void Initialize(string host, ushort port)
		{
			base.Initialize(host, port);
			InitializeTCPClient(host, port);
		}

		protected void InitializeTCPClient(string host, ushort port)
		{
			//Set the port
			SetPort(port);

			// Startup the read thread and have it listening for any data from the server
			Task.Queue(ReadAsync);
		}

		private void ReadAsync()
		{
			try
			{
				while (IsBound)
				{
					switch (Read())
					{
						case ReadState.Void:
							break;
						case ReadState.Continue:
							Thread.Sleep(10);
							break;
						case ReadState.Disconnect:
							return;
					}
				}
			}
			catch (Exception e)
			{
				// There was in issue reading or executing from the network so disconnect
				// TODO:  Add more logging here and an exception with an inner exception being
				// the exception that was thrown
				Logging.BMSLog.LogException(e);
				//Console.WriteLine("CRASH: " + e.Message);
				Disconnect(true);
			}
		}
	}
}