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

namespace BeardedManStudios.Forge.Networking
{
    /// <summary>
    /// This interface is to verify that the client server connection is
    /// valid. This is an optional system.
    /// </summary>
    public interface IUserAuthenticator
    {
        /// <summary>
        /// Issues a challenge to send to the user for authentication.
        /// </summary>
        /// <param name="networker">The networker the user connected to</param>
        /// <param name="player">The user connecting to the networker</param>
        /// <param name="issueChallengeAction">Callback to invoke when the user should be authenticated. Must be invoked with the given player and an optional challenge for the user</param>
        /// <param name="skipAuthAction">Callback to invoke when the user should be automatically authenticated. Must be invoked with the given player</param>
        void IssueChallenge(NetWorker networker, NetworkingPlayer player, Action<NetworkingPlayer, BMSByte> issueChallengeAction, Action<NetworkingPlayer> skipAuthAction);

        /// <summary>
        /// Verifies the challenge issued by the server and generates a response. 
        /// </summary>
        /// <param name="networker">The networker of the user</param>
        /// <param name="challenge">Incoming challenge from the server</param>
        /// <param name="authServerAction">Callback to invoke when the server is valid. Should return the auth response for the server</param>
        /// <param name="rejectServerAction">Callback to invoke when the server is invalid and the user should disconnect</param>
        void AcceptChallenge(NetWorker networker, BMSByte challenge, Action<BMSByte> authServerAction, Action rejectServerAction);

        /// <summary>
        /// Verifies the responce of the user, returns true if the responce is verified.
        /// </summary>
        /// <param name="networker">The networker of the user</param>
        /// <param name="player">The user connecting to the networker</param>
        /// <param name="response">Incoming response from the user</param>
        /// <param name="authUserAction">Callback to invoke when the user has been authorized. Must be invoked by the given player</param>
        /// <param name="rejectServerAction">Callback to invoke when the user is unauthorized and the user should disconnect. Must be invoked by the given player</param>
        void VerifyResponse(NetWorker networker, NetworkingPlayer player, BMSByte response, Action<NetworkingPlayer> authUserAction, Action<NetworkingPlayer> rejectUserAction);
    }
}
