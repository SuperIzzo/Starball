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

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  A representation of a player object.  </summary>
    /// <design>
    ///     This is a facade class that wraps around common player
    ///     functionality and provides a single point of access.
    ///     Internally it encapsulates a networked player and 
    ///     only works with Unity's networking, but on the outside
    ///     it should look like it has nothing to do with that, so
    ///     please try to keep its interface clean and uncoupled.
    ///     
    ///     In addition this is a immutable object. Don not add 
    ///     more state variables unless you really have to.
    ///     _netID is all it needs.                       </design>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Player
    {
        //-------------------------------------------------------------
        /// <summary>  The network instance id
        ///            of the player object.                 </summary>
        //----------------------------------
        private readonly NetworkInstanceId  _netID;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets the transform
        ///            of the player game object.            </summary>
        //::::::::::::::::::::::::::::::::::
        public Transform transform
        {
            get { return gameObject.transform; }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the star of the player.  </summary>
        //::::::::::::::::::::::::::::::::::
        public Star star
        {
            get { return starController ? starController.star : null; }
            set { if( starController ) starController.star = value; }
        }

        //.............................................................
        /// <summary>  Gets the player game object.          </summary>
        //..................................
        private GameObject gameObject
        {
            get
            {
                if( !_gameObject )
                {
                    if( NetworkServer.active )
                    {
                        _gameObject = NetworkServer.FindLocalObject( _netID );
                    }
                    else
                    {
                        _gameObject = ClientScene.FindLocalObject( _netID );
                    }
                }
                return _gameObject;
            }
        }
        private GameObject _gameObject;

        //.............................................................
        /// <summary>  Gets the star controller component 
        ///            of the player game object.            </summary>
        //..................................
        private PlayerStarController starController
        {
            get
            {
                if( !_starController && gameObject )
                {
                    _starController =
                        gameObject.GetComponent<PlayerStarController>();
                }
                return _starController;
            }
        }
        private PlayerStarController _starController;


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Constructs a new player 
        ///            with a NetworkInstanceId.             </summary>
        /// <param name="netID"></param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        internal Player( NetworkInstanceId netID )
        {
            _netID = netID;
        }

        //=============================================================
        /// <summary>  Returns wether two players 
        ///            are one and the same.                 </summary>
        //==================================
        public static bool operator ==( Player a, Player b )
        {
            return a._netID == b._netID;
        }

        //=============================================================
        /// <summary>  Returns wether two players 
        ///            are different entities.               </summary>
        //==================================
        public static bool operator !=( Player a, Player b )
        {
            return a._netID != b._netID;
        }

        //=============================================================
        /// <summary>  Returns wether this player 
        ///            is equal to the given object.         </summary>
        //==================================
        public override bool Equals( object other )
        {
            return (other as Player) == this;
        }

        //=============================================================
        /// <summary>  Returns a unique hashing code 
        ///            for this player.                      </summary>
        //==================================
        public override int GetHashCode()
        {
            return _netID.GetHashCode();
        }
    }
}