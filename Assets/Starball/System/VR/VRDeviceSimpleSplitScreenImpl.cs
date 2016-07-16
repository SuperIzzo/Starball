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
namespace Izzo.VR
{
    using System;
    using UnityEngine;
    using UnityEngine.VR;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Simple split screen 
    ///            VR device implementation.             </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class VRDeviceSimpleSplitScreenImpl : IVRDevice
    {
        private const string splitScreenVRDeviceName = "split";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public virtual bool isSupported { get { return true; } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public bool enabled
        {
            get
            {
                return IsEnabled();
            }

            set
            {
                if( value && !_enabled )
                {
                    _enabled = OnEnable();
                }
                else if( !value && _enabled )
                {
                    OnDisable();
                    _enabled = false;
                }
            }
        }        
        private bool _enabled;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public virtual float renderScale
        {
            get { return VRSettings.renderScale; }
            set { VRSettings.renderScale = value; }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public virtual float renderQuality { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public virtual string family
        {
            get
            {
                return splitScreenVRDeviceName;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        ///  <inheritdoc/>
        //::::::::::::::::::::::::::::::::::
        public virtual string model
        {
            get
            {
                if( VRSettings.loadedDeviceName == splitScreenVRDeviceName )
                {
                    return VRDevice.model;
                }
                else
                {
                    return "";
                }
            }
        }

        //=============================================================
        ///  <inheritdoc/>
        //==================================
        public virtual void Update()
        {
            // HOOK            
        }

        //=============================================================
        ///  <inheritdoc/>
        //==================================
        public virtual void DrawFrame()
        {
            // HOOK
        }

        //=============================================================
        ///  <inheritdoc/>
        //==================================
        public virtual void UpdateCamera( Camera camera )
        {
            // HOOK
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Returns whether the device is enabled. </summary>
        /// <returns>  true if the VR device is supported, 
        ///            connected and loaded; false otherwise. </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected virtual bool IsEnabled()
        {
            return VRSettings.enabled && _enabled;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Called once when the VR device is about to
        ///            become enabled.                       </summary>
        /// <returns>  true if initialization is successful; 
        ///            false otherwise.                      </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected virtual bool OnEnable()
        {
            VRSettings.LoadDeviceByName( splitScreenVRDeviceName );
            VRSettings.enabled = true;

            return true;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Called once when the VR devices is about to
        ///            be disabled.                          </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected virtual void OnDisable()
        {
            if( VRSettings.loadedDeviceName == splitScreenVRDeviceName )
            {
                VRSettings.enabled = false;
            }
        }        
    }
}