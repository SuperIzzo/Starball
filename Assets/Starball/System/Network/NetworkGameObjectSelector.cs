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

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Removes game objects based on whether the game
    ///            starts as a client or a server.       </summary>
    /// <remarks>
    ///     Clients remove all server game objects; 
    ///     Servers remove all client game objects;
    ///     Hosts keep everything.
    /// </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [AddComponentMenu( "Network/NetworkObjectSelector" )]

    public class NetworkGameObjectSelector : NetworkStripper
    {
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of objects that only clients should have. "        )]
        //----------------------------------        
        private GameObject[] _clientOnlyObjects = null;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of objects that only servers should have. "        )]
        //----------------------------------
        private GameObject[] _serverOnlyObjects = null;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Removes server-only game objects.      </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override void StripClient()
        {
            foreach( GameObject serverObject in _serverOnlyObjects )
            {
                Destroy( serverObject );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Removes client-only game objects.      </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override void StripServer()
        {
            foreach( GameObject clientObject in _clientOnlyObjects )
            {
                Destroy( clientObject );
            }
        }
    }
}