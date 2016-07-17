/*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + *\
+                                                                              +
+     Starball - multiplayer ball sports game with spinning star drones.       +
+     Copyright (C) 2016  SuperIzzo                                            +
+                                                                              +
+     This file is part of Starball.                                           +
+                                                                              +
+     Mulsh is free software: you can redistribute it and/or modify            +
+     it under the terms of the GNU General Public License as published by     +
+     the Free Software Foundation, either version 3 of the License, or        +
+     ( at your option) any later version.                                     +
+                                                                              +
+     Mulsh is distributed in the hope that it will be useful,                 +
+     but WITHOUT ANY WARRANTY; without even the implied warranty of           +
+     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            +
+     GNU General Public License for more details.                             +
+                                                                              +
+     You should have received a copy of the GNU General Public License        +
+     along with Mulsh.  If not, see<http://www.gnu.org/licenses/>.            +
+                                                                              +
\*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + */
namespace Izzo.Networking
{
    using System.Collections;
    using UnityEngine.Networking;

#if UNITY_EDITOR && DEDICATED_SERVER
    using UnityEditor.Callbacks;
#endif

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Removes components based on whether the game
    ///            starts as a client or a server.       </summary>
    /// <remarks>
    ///     Clients remove all server components; 
    ///     Servers remove all client components;
    ///     Hosts keep everything.
    /// </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public abstract class NetworkStripper : NetworkBehaviour
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Removes server-only behaviour.        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected abstract void StripClient();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Removes client-only behaviour.        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected abstract void StripServer();

        //=============================================================
        /// <summary> Removes client-only behaviour if the server
        ///           is not also a client.                  </summary>
        /// <remarks> 
        ///     Local clients are never initialized when this method 
        ///     is called. In order to check if there are any local
        ///     clients we need to wait for one frame. The actual 
        ///     removing is then done in OnLateStartServer().</remarks>
        //==================================
        public override void OnStartServer()
        {
            StartCoroutine( InvokeOnLateStartServer() );
        }

        //=============================================================
        /// <summary> Removes server-only behaviour if the client
        ///           is not also a server.                  </summary>
        //==================================
        public override void OnStartClient()
        {
            if( !isServer )
            {
                StripClient();
                Destroy( this );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Actually removes client-only behaviour if the 
        ///           server is not also a client.           </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void OnLateStartServer()
        {
            if( !isClient )
            {
                StripServer();
            }
            Destroy( this );
        }        

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Invokes InvokeOnLateStartServer() after 
        ///           one frame delay.                       </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        IEnumerator InvokeOnLateStartServer()
        {
            yield return null;
            OnLateStartServer();
        }

#if UNITY_EDITOR && DEDICATED_SERVER
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Strips dedicated servers 
        ///            during the build.                     </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [PostProcessBuild( -3 )]
        [PostProcessScene( -3 )]
        public static void OnPostprocessBuild()
        {
            foreach( var serverStripper in FindObjectsOfType<NetworkStripper>() )
            {
                if( serverStripper.isActiveAndEnabled )
                {
                    serverStripper.StripServer();
                }
            }
        }
#endif
    }
}