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
    /// This interface is to verify that the client server connection is
    /// valid. This is an optional system.
    /// </summary>
    public interface IUserAuthenticator
    {
        /// <summary>
        /// Issues a challenge to send to the user for authentication. Returns false if authentication is not required.
        /// </summary>
        bool IssueChallenge(NetWorker networker, NetworkingPlayer player, ref BMSByte challenge);

        /// <summary>
        /// Verifies the challenge issued by the server and generates a response. Returns false if the challenge failed and the user should disconnect.
        /// </summary>
        bool AcceptChallenge(NetWorker networker, BMSByte challenge, ref BMSByte response);

        /// <summary>
        /// Verifies the responce of the user, returns true if the responce is verified. Returns false if the user should be rejected and disconnected.
        /// </summary>
        bool VerifyResponse(NetWorker networker, NetworkingPlayer player, BMSByte response);
    }
}
