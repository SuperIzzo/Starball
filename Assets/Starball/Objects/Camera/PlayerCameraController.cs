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
    using Izzo.VR;
    using Izzo.Input;
    using System.Collections.Generic;
    using System;

    public class PlayerCameraController : MonoBehaviour, ITargetOfInterestObserver
    {
        [SerializeField]
        private Transform _minicamTarget = null;

        [SerializeField]
        private float _minicamFOV = 30f;

        private const float permanentViewSwitchThreshold = 0.3f;
        private const float autoRotationSpeed = 0.5f;
        private const float manualRotationOverwriteThreshold = 0.2f;
        private const float manualRotationOverwriteTime = 10;

        private IList<ITargetOfInterest> targetsOfInterest;

        private float sensitivity = 0.5f;
        private float prevMouseX;
        private float prevMouseY;
        private bool lookingAtMinicam;
        
        private float viewSwitchTimer = 0;
        private float manualOverwriteTimer = 0;

        private Quaternion originalOrientation;
        private Quaternion preResetOrientation;

        private float originalFOV;
        private float preResetFOV;

        private new Camera camera
        {
            get
            {
                if( !_camera )
                {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
        }
        private Camera _camera;


        public void AddTargetOfInterest( ITargetOfInterest item )
        {
            if( targetsOfInterest==null )
            {
                targetsOfInterest = new List<ITargetOfInterest>();
            }

            targetsOfInterest.Add( item );
        }

        public bool RemoveTargetOfInterest( ITargetOfInterest item )
        {
            if( targetsOfInterest != null )
            {
                return targetsOfInterest.Remove( item );
            }
            else
            {
                return false;
            }
        }

        protected void Start()
        {
            Cursor.visible = false;
            originalOrientation = camera.transform.rotation;
            originalFOV = camera.fieldOfView;
        }

        void Update()
        {
            if( !VRManager.isVREnabled )
            {
                CameraControl();
            }
        }

        private void CameraControl()
        {
            HandleMinicamSwitch();

            if( !lookingAtMinicam )
            {
                HandleManualLook();
                HandleAutoLook();
            }
        }

        private void HandleManualLook()
        {
            float viewX = InputManager.GetAxis( "View Horizontal" );
            float viewY = InputManager.GetAxis( "View Vertical" );

            camera.transform.Rotate( Vector3.up, viewX * sensitivity, Space.World );
            camera.transform.Rotate( Vector3.left, viewY * sensitivity, Space.Self );

            manualOverwriteTimer -= Time.deltaTime;
            if( manualOverwriteTimer<0 )
            {
                manualOverwriteTimer = 0;
            }

            if( ( viewX*viewX + viewY*viewY)>manualRotationOverwriteThreshold )
            {
                manualOverwriteTimer = manualRotationOverwriteTime;
            }            
        }

        private void HandleAutoLook()
        {            
            Vector3 pointOfInterest = GetBestPointOfInterest();
            Vector3 forward = pointOfInterest - camera.transform.position;

            float rotationRate = autoRotationSpeed;
            rotationRate *= Time.deltaTime;
            rotationRate *= 1 - manualOverwriteTimer / manualRotationOverwriteTime; 

            Quaternion rotation = camera.transform.rotation;
            Quaternion target = Quaternion.LookRotation( forward, Vector3.up );
            var newRotation = Quaternion.Slerp( 
                            rotation, 
                            target, 
                            rotationRate );

            camera.transform.rotation = newRotation;            
        }        

        private void HandleMinicamSwitch()
        {
            bool resetDown = InputManager.GetButtonDown( "View Reset" );
            bool resetUp = InputManager.GetButtonUp( "View Reset" );
            bool resetHeld = InputManager.GetButton( "View Reset" );

            if( resetDown )
            {
                lookingAtMinicam = !lookingAtMinicam;
                viewSwitchTimer = 0;

                if( lookingAtMinicam )
                {
                    preResetOrientation = camera.transform.rotation;
                    preResetFOV = camera.fieldOfView;
                    camera.transform.LookAt( _minicamTarget );
                    camera.fieldOfView = _minicamFOV;
                }
                else
                {
                    camera.transform.rotation = originalOrientation;
                    camera.fieldOfView = originalFOV;
                }
            }

            if( resetUp )
            {
                if( viewSwitchTimer >= permanentViewSwitchThreshold )
                {
                    lookingAtMinicam = !lookingAtMinicam;

                    if( lookingAtMinicam )
                    {
                        camera.transform.LookAt( _minicamTarget );
                        camera.fieldOfView = _minicamFOV;
                    }
                    else
                    {
                        camera.transform.rotation = preResetOrientation;
                        camera.fieldOfView = preResetFOV;
                    }
                }
            }

            if( resetHeld )
            {
                viewSwitchTimer += Time.deltaTime;
            }
        }

        private Vector3 GetBestPointOfInterest()
        {
            Vector3 pointOfInterest = Vector3.zero;

            if( targetsOfInterest != null )
            {
                float totalWeight = 0;
                foreach( ITargetOfInterest target in targetsOfInterest )
                {
                    pointOfInterest += target.position * target.weight;
                    totalWeight += target.weight;
                }

                pointOfInterest /= totalWeight;
            }

            return pointOfInterest;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if( targetsOfInterest != null )
            {
                Vector3 pointOfInterest = GetBestPointOfInterest();

                float maxWeight = 0;
                foreach( ITargetOfInterest target in targetsOfInterest )
                {
                    if( target.weight>maxWeight )
                    {
                        maxWeight = target.weight;
                    }
                }

                foreach( ITargetOfInterest target in targetsOfInterest )
                {
                    Gizmos.color = new Color(0,1,1, target.weight/maxWeight );
                    Gizmos.DrawLine( target.position, pointOfInterest );
                }
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere( pointOfInterest, 0.3f );
            }
        }
#endif
    }    
}