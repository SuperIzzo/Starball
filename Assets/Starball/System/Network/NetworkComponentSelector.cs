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
    using System.Collections.Generic;
    using UnityEngine;

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

    public class NetworkComponentSelector : NetworkStripper
    {
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of components that only clients should have. "     )]
        //----------------------------------        
        private List<Component> _clientOnlyComponents = null;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of components that only servers should have. "     )]
        //----------------------------------
        private List<Component> _serverOnlyComponents = null;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Removes server-only components.        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override void StripClient()
        {
            foreach( Component serverComponent in _serverOnlyComponents )
            {
                Destroy( serverComponent );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Removes client-only components.        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override void StripServer()
        {
            foreach( Component clientComponent in _clientOnlyComponents )
            {
                Destroy( clientComponent );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Adds a client-only component to the list. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void AddClientOnlyComponent( Component component )
        {
            _clientOnlyComponents.Add( component );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Adds a server-only component to the list. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void AddServerOnlyComponent( Component component )
        {
            _serverOnlyComponents.Add( component );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Clears the client-only component list. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void ClearClientOnlyComponents()
        {
            _clientOnlyComponents.Clear();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Clears the server-only component list. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void ClearServerOnlyComponents()
        {
            _serverOnlyComponents.Clear();
        }
    }
}