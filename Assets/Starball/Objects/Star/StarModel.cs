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

    [DisallowMultipleComponent]
    [AddComponentMenu( "Starball/Star/Star Model" )]
    [RequireComponent( typeof( Rigidbody ) )]

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  A gameplay model. Controls the physical
    ///            movement of a star.                   </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    public class StarModel : MonoBehaviour
    {
        //-------------------------------------------------------------	
        [SerializeField, Tooltip(
        "The movement speed of the star (in newtons).              " )]
		//----------------------------------
        private float _movementSpeed = 5;

        //-------------------------------------------------------------	
        [SerializeField, Tooltip(
        "The rotation speed of the star (in newtons).              " )]
		//----------------------------------
        private float _spinSpeed = 10;

        //-------------------------------------------------------------
        /// <summary>  Movement direction provided by input. </summary>
        //----------------------------------
        private Vector2 _inputDirection = Vector2.down;

        //-------------------------------------------------------------
        /// <summary>  Rotation as provided by input.        </summary>
        //----------------------------------
        private float _inputSpin = 0.0f;

        //.............................................................
        /// <summary> A reference to the Rigidbody component.</summary>
        //..................................
        private Rigidbody rigidBody { get; set; }

        //=============================================================
        /// <summary> Commands the star to move in direction.</summary>
        /// <param name="direction"> 
        ///     Indicates the direction and the amount of 
        ///     movement of the star. Capped to a magnitude of 1.
        /// </param>
        //==================================
        public void Move( Vector2 direction )
        {
            float magnitude = direction.magnitude;
            if( magnitude <= 1 )
            {
                _inputDirection = direction;
            }
            else
            {
                _inputDirection = direction / magnitude;
            }
        }

        //=============================================================
        /// <summary> Commands the star to spin in direction.</summary>
        /// <param name="direction"> 
        ///     Indicates the direction and the amount of 
        ///     movement of the star. Capped to a magnitude of 1.
        /// </param>
        //==================================
        public void Spin( float spin )
        {
            _inputSpin = Mathf.Clamp( spin, -1, 1 );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  UNITY CALLBACK.                       </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = 200;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  UNITY CALLBACK.                       </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void FixedUpdate()
        {
            DoMove();
            DoSpin();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Actually moves the star.              </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void DoMove()
        {
            Vector3 force = new Vector3( _inputDirection.x,
                                         0,
                                         _inputDirection.y );
            force *= _movementSpeed;

            rigidBody.AddForce( force );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Actually spins the star.              </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void DoSpin()
        {
            Vector3 torque = new Vector3( 0, _inputSpin * _spinSpeed, 0 );
            rigidBody.AddRelativeTorque( torque );
        }
    }
}