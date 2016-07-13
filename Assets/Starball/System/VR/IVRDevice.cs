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
    using UnityEngine;

    public interface IVRDevice
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Returns whether the VR device is supported 
        ///            on the current platform.              </summary>
        //::::::::::::::::::::::::::::::::::
        bool isSupported { get; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Enables, disables or gets the enabled status
        ///            of the VR device.                     </summary>
        //::::::::::::::::::::::::::::::::::
        bool enabled { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the render scale.        </summary>
        /// <remarks>
        ///     The renderScale can be used to control the size of
        ///     the internal render buffer of the VR device. Default
        ///     is 1. Higher values will result into higher quality
        ///     crisper images, while lower values will reduce the
        ///     quality, but increase the performance. Changing the 
        ///     render scale may cause realocation of internal buffers 
        ///     or changing of the resolution of a device; 
        ///     as such it should not be used every frame.   </remarks>
        //::::::::::::::::::::::::::::::::::
        float renderScale { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the render quality.      </summary>
        /// <remarks>
        ///     Just like renderScale this property can be used to
        ///     control the tradeoff between perfomance and quality.
        ///     It is meant to be used as a general quality control
        ///     that can innexpensively be used to fine-tune the
        ///     performance based on per-frame basis when supported. 
        ///     Unlike renderScale changing the renderQuality should 
        ///     not reallocate resources unnecessarily or result in
        ///     drawing commands which would cause expensive changes
        ///     of rendering state.                          </remarks>
        //::::::::::::::::::::::::::::::::::
        float renderQuality { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets the name of the 
        ///            family of the VR device.              </summary>
        //::::::::::::::::::::::::::::::::::
        string family { get; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets the model of 
        ///            family of the VR device.              </summary>
        //::::::::::::::::::::::::::::::::::
        string model { get; }

        //=============================================================
        /// <summary>  Updates the state of the VR devices.  </summary>
        /// <remarks>
        ///     This is called each frame on the active VR device
        ///     regardless of whether the device was enabled or not.
        ///                                                  </remarks>
        //==================================
        void Update();

        //=============================================================
        /// <summary>  Sends a single frame to the VR device.</summary>
        /// <remarks>
        ///     DrawFrame is called on the active VR device once
        ///     it has been successfully enabled. This function 
        ///     is called right after Unity has finished renderig
        ///     the current frame.                           </remarks>
        //==================================
        void DrawFrame();

        //=============================================================
        /// <summary>  Updates a camera based on the input orientation
        ///            and position from the VR device.      </summary>
        /// <remarks>
        ///     This function is called right before unity has
        ///     started rendering and is used to position the virtual
        ///     camera within the scene based on the input.  </remarks>
        /// <param name="camera"> The camera to be updated.    </param>
        //==================================
        void UpdateCamera( Camera camera );
    }
}