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
    using UnityEngine;
    using UnityEngine.Networking;

    [AddComponentMenu("Starball/Network/NetworkManager")]

    public class StarballNetworkManager : NetworkManager
    {    
        public override void OnServerAddPlayer( NetworkConnection connection, 
                                                short playerControllerID )
        {
            GameObject playerGameObject = CreatePlayerGameObject();

            bool playerAdded =
                NetworkServer.AddPlayerForConnection( connection,
                                                      playerGameObject,
                                                      playerControllerID );
            if( playerAdded )
            {
                var playerController =
                    connection.playerControllers[playerControllerID];

                Player player = PlayerManager.AddPlayer( playerController );
                OnPlayerSpawn( player );
            }
        }

        public override void OnServerRemovePlayer( NetworkConnection conn,
                                                   PlayerController player )
        {
            PlayerManager.RemovePlayer( player );
            base.OnServerRemovePlayer( conn, player );
        }

        private GameObject CreatePlayerGameObject()
        {
            Vector3 playerPosition = GetPlayerSpawnPosition();

            var player = Instantiate( playerPrefab,
                                      playerPosition,
                                      Quaternion.identity ) as GameObject;
            return player;
        }

        private Vector3 GetPlayerSpawnPosition()
        {
            Transform playerSpawn = GetStartPosition();
            Vector3 playerPosition = Vector3.zero;

            if( playerSpawn )
            {
                playerPosition = playerSpawn.position;
            }

            return playerPosition;
        }

        private void OnPlayerSpawn( Player player )
        {
            IPlayerSpawnListener[] playerSpawnListeners =
                transform.root.GetComponentsInChildren<IPlayerSpawnListener>();

            foreach( var playerSpawnListener in playerSpawnListeners )
            {
                playerSpawnListener.OnSpawnPlayer( player );
            }
        }
    }
}