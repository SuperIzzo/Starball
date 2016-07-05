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
namespace Izzo.Utility
{
    using UnityEngine;
    using UnityEngine.Networking;
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  
    ///     An utility class tthat deals with GameObjects.
    ///                                                  </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public static class Entity
    {
        //=============================================================
        /// <summary>  Spawns a new game object instance.    </summary>
        /// <param name="entityType">
        ///     The game object type to spawn.                 </param>
        /// <param name="position">
        ///     The position of the new entity.                </param>
        /// <param name="rotation">
        ///     The rotation of the new entity.                </param>
        /// <returns> The new entity.                        </returns>
        //==================================
        public static GameObject Spawn( GameObject entityType,
                                        Vector3 position,
                                        Quaternion rotation )
        {
            var entity = GameObject.Instantiate( entityType,
                                                 position,
                                                 rotation ) as GameObject;

            // TODO: Rework, clients can't instantiate network identities
            if( NetworkServer.active
                && entity.GetComponent<NetworkIdentity>() )
            {
                NetworkServer.Spawn( entity );
            }

            return entity;
        }

        //=============================================================
        /// <summary>  Destroys a game object.               </summary>
        /// <param name="entity"> The instance to be destroyed.</param>
        //==================================
        public static void Destroy( GameObject entity )
        {
            if( entity.GetComponent<NetworkIdentity>() )
            {
                if( NetworkServer.active )
                {
                    NetworkServer.Destroy( entity );
                }
                else
                {
                    Debug.LogError( "Cannot destroy network instance `"
                                    + entity.name + "` from a client." );
                }
            }
            else
            {
                GameObject.Destroy( entity );
            }
        }
    }
}