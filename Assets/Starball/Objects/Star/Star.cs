﻿/*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + *\
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
    /// <summary>  A representation of a Star object.    </summary>
    /// <design>
    ///     Star is an immutable facade class with a value 
    ///     'networkIdentity' and provides an interface 
    ///     to the star sub-systems.                      </design>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% 
    public class Star
    {
        //.............................................................
        /// <summary>  A reference to the star game object.  </summary>
        //..................................
        private GameObject gameObject { get; set; }

        //.............................................................
        /// <summary>  A reference to the StarModel.         </summary>
        //..................................
        private StarModel model
        {
            get
            {
                if( !_model )
                {
                    _model = gameObject.GetComponent<StarModel>();
                }
                return _model;
            }
        }
        private StarModel _model=null;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  A reference to the 
        ///            star NetworkIdentity component.       </summary>
        //::::::::::::::::::::::::::::::::::
        public NetworkIdentity networkIdentity
        {
            get
            {
                if( !_networkIdentity )
                {
                    _networkIdentity =
                        gameObject.GetComponent<NetworkIdentity>();
                }
                return _networkIdentity;
            }
        }
        private NetworkIdentity _networkIdentity=null;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets whether this Star can be controlled. </summary>
        //::::::::::::::::::::::::::::::::::    
        public bool isControllable
        {
            get { return model != null && model.isActiveAndEnabled; }
        }        

        //=============================================================
        /// <summary>  Constructs a new star.                </summary>
        /// <param name="starGameObject"> 
        ///     the root game object of the star instance      </param>
        //==================================
        public Star( GameObject starGameObject )
        {
            gameObject = starGameObject;
        }

        //=============================================================
        /// <summary>  Moves the star in the given direction.</summary>
        /// <param name="movementInput"> 
        ///     The direction and amount of movement.          </param>
        //==================================
        public void Move( Vector2 movementInput )
        {
            if( isControllable )
            {
                model.Move( movementInput );
            }
            else
            {
                Debug.LogError( "Attempting to command a non-controllable " +
                                "Star to move. Use `isControllable` to test " +
                                "whether the star can be moved." );
            }
        }

        //=============================================================
        /// <summary>  Spins the star in the given direction.</summary>
        /// <param name="spin">
        ///     The direction and amount of spin.              </param>
        //==================================
        public void Spin( float spin )
        {
            if( isControllable )
            {
                model.Spin( spin );
            }
            else
            {
                Debug.LogError( "Attempting to command a non-controllable " +
                                "Star to spin. Use `isControllable` to test " +
                                "whether the star can be spun." );
            }
        }
    }
}