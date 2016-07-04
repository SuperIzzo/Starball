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

    [AddComponentMenu( "Starball/Player/Player Manager" )]

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    ///<summary>  An utility class that handles players. </summary>
    ///<design>
    ///     This is a lazy static inteface class.
    ///     It is a staic class, an interface and implementation
    ///     all in one. In future it may split into three. 
    ///     To facilitate this, do not add non-static 
    ///     public methods or properties.                 </design>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class PlayerManager : NetworkBehaviour
    {
        private SyncListUInt _playerNetIDs = new SyncListUInt();

        //.............................................................
        /// <summary>  Gets the active PlayerManager 
        ///            in the scene.                         </summary>
        //..................................
        private static PlayerManager active
        {
            get
            {
                if( !_active )
                {
                    _active = FindObjectOfType<PlayerManager>();
                }
                return _active;
            }
        }
        private static PlayerManager _active;

        //.............................................................
        /// <summary>  Adds a new player to the manager.     </summary>
        /// <param name="playerController">
        ///     A networked player object from UNET.           </param>
        /// <returns> A game player object.                  </returns>
        //..................................
        internal static Player AddPlayer( PlayerController playerController )
        {
            return active.AddPlayerImpl( playerController );
        }

        //.............................................................
        /// <summary>  Removes a player from the manager.    </summary>
        /// <param name="playerController">
        ///     A networked player object from UNET.           </param>
        //..................................
        internal static void RemovePlayer( PlayerController playerController )
        {
            active.RemovePlayerImpl( playerController );
        }

        //.............................................................
        /// <summary>  Adds a new player to the manager.     </summary>
        /// <param name="playerController">
        ///     A networked player object from UNET.           </param>
        /// <returns> A game player object.                  </returns>
        //..................................
        [Server]
        private Player AddPlayerImpl( PlayerController playerController )
        {
            Player player = null;

            NetworkInstanceId playerNetID = playerController.unetView.netId;

            int globalPlayerID = GetGlobalIDForNetID( playerNetID );
            if( globalPlayerID >= 0 )
            {
                Debug.LogError( "Player with unique netID (" +
                                 playerNetID.Value + ") already exist" );
            }
            else
            {
                lock( _playerNetIDs )
                {
                    globalPlayerID = GetEmptyGlobalID();
                    if( globalPlayerID >= 0 )
                    {
                        _playerNetIDs[globalPlayerID] = playerNetID.Value;
                    }
                    else
                    {
                        _playerNetIDs.Add( playerNetID.Value );
                    }

                }

                player = new Player( playerNetID );
            }

            return player;
        }

        //.............................................................
        /// <summary>  Removes a player from the manager.    </summary>
        /// <param name="playerController">
        ///     A networked player object from UNET.           </param>
        //..................................
        private void RemovePlayerImpl( PlayerController playerController )
        {
            lock( _playerNetIDs )
            {
                NetworkInstanceId playerNetID = playerController.unetView.netId;
                int globalPlayerID = GetGlobalIDForNetID( playerNetID );
                if( globalPlayerID >= 0 )
                {
                    _playerNetIDs[globalPlayerID] =
                        NetworkInstanceId.Invalid.Value;
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Returns the game player id associated with
        ///            the give networ instance id.          </summary>
        /// <param name="playerNetID"> UNET instance id.       </param>
        /// <returns>  The registered global id of a player. </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private int GetGlobalIDForNetID( NetworkInstanceId playerNetID )
        {
            return _playerNetIDs.IndexOf( playerNetID.Value );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Returns an unassigned game player id. </summary>
        /// <returns>  An unassigned global id; or -1 if all currently
        ///            set global ids are registered to players.
        ///                                                  </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private int GetEmptyGlobalID()
        {
            return GetGlobalIDForNetID( NetworkInstanceId.Invalid );
        }
    }
}