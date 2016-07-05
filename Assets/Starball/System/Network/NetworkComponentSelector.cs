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
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Networking;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Removes components based on whether the game
    ///            starts as a client or a server.       </summary>
    /// <remarks>
    ///     Clients remove all server components; 
    ///     Servers remove all client components;
    ///     Hosts keep everything.
    /// </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [AddComponentMenu( "Network/NetworkComponentSelector" )]

    public class NetworkComponentSelector : NetworkBehaviour
    {
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of components that only clients should have. "     )]
        //----------------------------------        
        private Component[] _clientOnlyComponents = null;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of components that only servers should have. "     )]
        //----------------------------------
        private Component[] _serverOnlyComponents = null;

        //=============================================================
        /// <summary> Removes client-only components if the server
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
        /// <summary> Removes server-only components if the client
        ///           is not also a server.                  </summary>
        //==================================
        public override void OnStartClient()
        {
            if( !isServer )
            {
                foreach( Component serverComponent in _serverOnlyComponents )
                {
                    Destroy( serverComponent );
                }
                Destroy( this );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Actually removes client-only components if the 
        ///           server is not also a client.           </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void OnLateStartServer()
        {
            if( !isClient )
            {
                foreach( Component clientComponent in _clientOnlyComponents )
                {
                    Destroy( clientComponent );
                }
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
    }
}