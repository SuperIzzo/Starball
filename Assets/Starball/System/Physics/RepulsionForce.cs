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
namespace Izzo.Physics
{
    using UnityEngine;

    [AddComponentMenu("Physics/Repulsion-Attraction Force")]

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Applies a repulsion force at point.   </summary>
    /// <remarks>
    ///     This is a simple representation of a repulsion force,
    ///     suitible for implementing, propelers, fans, magnets, 
    ///     and so on. The force is applied to a parent rigid body 
    ///     at the point of the transform of this object and 
    ///     opposite to its forward direction.
    /// </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class RepulsionForce : MonoBehaviour
    {
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The amount of repulsion force applied." +
          "\nUse a negative value to attract objects instead."       )]
        //----------------------------------        
        float _forceAmount = 1;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The maximal distance at which the force is aplied."       )]
        //----------------------------------        
        float _maxDistance = 1;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Collision layers to include or ignore.\n" +
          "(e.g. magnets only affect certain objects.)"              )]
        //----------------------------------
        LayerMask _pushAgainst;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Whether this force should affect objects "+
          "other than the this (default is true)."                   )]
        //----------------------------------
        bool _pushOtherObjects = true;

        //-------------------------------------------------------------
        /// <summary>  Reference to a parent rigid body.     </summary>
        //----------------------------------
        Rigidbody rigidBody;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets up references.                   </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void Awake()
        {
            rigidBody = GetComponentInParent<Rigidbody>();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Applies the repulsion force.          </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void FixedUpdate()
        {
            RaycastHit hitInfo;
            bool hasHit = Physics.Raycast( transform.position,
                                       transform.forward,
                                       out hitInfo,
                                       _maxDistance,
                                       _pushAgainst );
            if( hasHit )
            {
                float distanceFactor = 1 - hitInfo.distance /_maxDistance;
                float finalForceAmount = _forceAmount * distanceFactor;
                Vector3 force = transform.forward * finalForceAmount;
                rigidBody.AddForceAtPosition( -force, transform.position );

                if( _pushOtherObjects && hitInfo.rigidbody )
                {
                    hitInfo.rigidbody.AddForceAtPosition( force, 
                                                          hitInfo.point );
                }
            }
        }
    }
}