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
namespace Izzo.Starball
{
    using Utility;
    using UnityEngine;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary> Handles global game rules.             </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [DisallowMultipleComponent]
    [AddComponentMenu( "Starball/Game/Simple Game Mode" )]

    public class SimpleGameMode : MonoBehaviour, IPlayerSpawnListener
    {
        //-------------------------------------------------------------
        [SerializeField,Tooltip
        ( "The star prefab to spawn for players."                    )]
        //----------------------------------        
        GameObject _starPrefab = null;

        //=============================================================
        /// <summary> Initializes the players as they spawn. </summary>
        /// <param name="player"> The player that spawned.     </param>
        //==================================
        public void OnSpawnPlayer( Player player )
        {
            if( _starPrefab )
            {
                GameObject star = Entity.Spawn( _starPrefab,
                                                player.transform.position,
                                                player.transform.rotation );
                player.star = new Star( star );
            }
        }
    }
}