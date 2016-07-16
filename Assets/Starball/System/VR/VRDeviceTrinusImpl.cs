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
#define TRINUS_VR
#define USE_SYS_DRAW

#if TRINUS_VR
using corelib.util;
using System.Collections.Generic;
using UnityEngine;

namespace Izzo.VR
{
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Trinus VR plugin implementation.      </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class VRDeviceTrinusImpl : VRDeviceSimpleSplitScreenImpl
    {
        private const int maxBufferHeight = 720;
        private const int maxScreenHeight = 1080;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  true if Trinus is supported on the 
        ///            current platfotm; false otherwise.    </summary>
        //::::::::::::::::::::::::::::::::::
        public override bool isSupported { get { return base.isSupported; } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the render scale.        </summary>
        //::::::::::::::::::::::::::::::::::
        public override float renderScale
        {
            get
            {
                return _renderScale;
            }
            set
            {
                _renderScale = value;
                if( trinusManager != null &&
                    trinusManager.getStatus() == DataStructs.STATUS.STREAMING )
                {
                    SetGameResolution( screenBufferHeight );
                }
            }
        }
        private float _renderScale = 1;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the render quality.      </summary>
        //::::::::::::::::::::::::::::::::::
        public override float renderQuality
        {
            get
            {
                return _renderQuality;
            }
            set
            {
                _renderQuality = Mathf.Clamp01( value );
            }
        }
        private float _renderQuality = 1;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Rutrns the family name of the VR device. </summary>
        //::::::::::::::::::::::::::::::::::
        public override string family
        {
            get { return "Trinus VR"; }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Returns the HMD model name.           </summary>
        //::::::::::::::::::::::::::::::::::
        public override string model
        {
            get
            {
                if( trinusManager != null )
                {
                    return trinusManager.getDeviceInfo().brand;
                }
                else
                {
                    return "";
                }
            }
        }

        //.............................................................
        /// <summary> Gets the screen width of the VR device.</summary>
        //..................................
        private int deviceScreenWidth
        {
            get
            {
                if( trinusManager != null )
                {
                    return trinusManager.getDeviceInfo().width;
                }
                else
                {
                    return 0;
                }
            }
        }

        //.............................................................
        /// <summary>Gets the screen height of the VR device.</summary>
        //..................................
        private int deviceScreenHeigh
        {
            get
            {
                if( trinusManager != null )
                {
                    return trinusManager.getDeviceInfo().height;
                }
                else
                {
                    return 0;
                }
            }
        }

        //.............................................................
        /// <summary>  Gets the screen aspect ration 
        ///            of the VR device.                     </summary>
        //..................................
        private float deviceScreenAspect
        {
            get
            {
                return deviceScreenWidth / (float) deviceScreenHeigh;
            }
        }

        //.............................................................
        /// <summary> Gets the screen buffer height. </summary>
        //..................................
        private int screenBufferHeight
        {
            get
            {
                int targetHeight = Mathf.Min( deviceScreenHeigh, Screen.height );
                targetHeight = Mathf.Min( maxBufferHeight, targetHeight );
                return targetHeight;
            }
        }

        //-------------------------------------------------------------
        /// <summary>  A unique trinus project id.           </summary>
        //----------------------------------
        private string projectID = "Starball";

        //-------------------------------------------------------------
        /// <summary> Internal trinus plugin manager reference. </summary>
        //----------------------------------
        private corelib.Manager trinusManager;

        //-------------------------------------------------------------
        /// <summary>  Input acceleration from 
        ///            the VR device accelerometer.          </summary>
        //----------------------------------
        private Vector3 acceleration;

        //-------------------------------------------------------------
        /// <summary>  Input rotation from 
        ///            the VR device gyroscope.              </summary>
        //----------------------------------
        private Quaternion rotation;

        //-------------------------------------------------------------
        /// <summary>  The image sent to the VR device,
        ///            when streaming.                       </summary>
        //----------------------------------
        private Texture2D screenBuffer = null;

        //-------------------------------------------------------------
        /// <summary>  The portion of the screen sent to the 
        ///            VR device when streaming.              </summary>
        //----------------------------------
        private Rect screenBufferRect = new Rect();

        //-------------------------------------------------------------
        /// <summary>  The previous screen width for which the
        ///            VR screen buffer was sized.           </summary>
        //----------------------------------
        private int lastScreenWidth;

        //-------------------------------------------------------------
        /// <summary>  The previous screen height for which the
        ///            VR screen buffer was sized.           </summary>
        //----------------------------------
        private int lastScreenHeight;

#if USE_SYS_DRAW
        //-------------------------------------------------------------
        /// <summary>  System bitmap used by Trinus for
        ///            optimisations when available.         </summary>
        //----------------------------------
        private System.Drawing.Bitmap bitmap = null;

        //-------------------------------------------------------------
        /// <summary>  System bitmap used by Trinus for
        ///            optimisations when available.         </summary>
        //----------------------------------
        private System.Drawing.Rectangle bitsRect;
#endif


        //=============================================================
        /// <summary>  Updates the Trinus VR device.         </summary>
        //==================================
        public override void Update()
        {
            if( trinusManager != null )
            {
                UpdateTrinus();
            }
        }

        //=============================================================        
        /// <summary>  Updates an in-game camera 
        ///            based on VR input.                    </summary>
        /// <param name="camera">  The camera to be updated.   </param>
        /// //==================================
        public override void UpdateCamera( Camera camera )
        {
            camera.transform.localRotation = rotation;
        }

        //=============================================================
        /// <summary>  Draws a single frame 
        ///            onto the Trinus VR device.            </summary>
        //==================================
        public override void DrawFrame()
        {
            if( trinusManager.getStatus() == DataStructs.STATUS.STREAMING )
            {
                if( lastScreenWidth != Screen.width ||
                    lastScreenHeight != Screen.height )
                {
                    SetBufferResolution( Screen.width, Screen.height );
                }

                lock( this )
                {
                    if( screenBuffer != null && trinusManager.isReadyForFrame() )
                    {
                        screenBuffer.ReadPixels( screenBufferRect, 0, 0, false );
#if USE_SYS_DRAW
                        byte[] bytes = screenBuffer.GetRawTextureData();

                        trinusManager.sendNextFrame( bytes,
                            (int) screenBufferRect.width,
                            (int) screenBufferRect.height,
                            (int) (renderQuality * 100) );
#else
                        byte[] bytes = screenBuffer.EncodeToJPG(
                            (int) renderQuality*100 );
                        trinusManager.sendNextFrame( bytes );
#endif
                    }
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Returns whether the VR device 
        ///            is functioning.                       </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override bool IsEnabled()
        {
            return base.IsEnabled()
                && trinusManager != null
                && trinusManager.getStatus() == DataStructs.STATUS.STREAMING;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Enables the devices.                  </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override bool OnEnable()
        {
            base.OnEnable();

            if( trinusManager == null )
            {
                trinusManager = new corelib.Manager( projectID );
            }

            return true;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Disables the device.                  </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected override void OnDisable()
        {
            base.OnDisable();
            DisconnectTrinus();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Handles the trinus server.            </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void UpdateTrinus()
        {
            DataStructs.STATUS status = trinusManager.getStatus();

            switch( status )
            {
                case DataStructs.STATUS.IDLE:
#if DEBUG
                    DataStructs.RESULT error =
                            trinusManager.getLastResult();

                    if( error.code == DataStructs.RESULT.CODE.ERROR )
                    {
                        Debug.Log( "Trinus error: " + error +
                                    "\n" + trinusManager.getSimpleLog() );
                    }
#endif
                    DataStructs.RESULT result = ConnectTrinus();

                    if( result.subCode ==
                        DataStructs.RESULT.SUBCODE.SUCCESS )
                    {
                        Debug.Log( "Connecting Trinus..." );
                    }
                    else
                    {
                        Debug.LogError(
                            "Trinus connection failed.\n" + result );
                    }
                    break;

                case DataStructs.STATUS.DISCONNECTED:
                    DataStructs.RESULT disconnectResult = trinusManager.getLastResult ();
                    Debug.Log( "Trinus disconnected (Reason: " + disconnectResult + ")" );
                    if( disconnectResult.code == DataStructs.RESULT.CODE.ERROR )
                    {
                        Debug.LogError( "disconnection error: " +
                                        disconnectResult.detail );
                    }
                    trinusManager.resetDisconnection();
                    break;

                case DataStructs.STATUS.CONNECTING:
                    string info = trinusManager.getLastResult().detail;
                    if( !string.IsNullOrEmpty( info ) )
                    {
                        Debug.Log( "Connecting Trinus: step - " + info );

                    }
                    break;

                case DataStructs.STATUS.CONNECTED:
#if DEBUG
                    Debug.Log( "Trinus connected, starting streaming "
                        + trinusManager.getLastResult().detail );
#endif
                    StartStreaming();
                    break;

                case DataStructs.STATUS.STREAMING:
                    GrabSensorData();
                    break;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Connects the Trinus server to 
        ///            a VR device client on the network.    </summary>
        /// <returns> The result of the operation.           </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private DataStructs.RESULT ConnectTrinus()
        {
            DataStructs.SETTINGS settings = DataStructs.SETTINGS.defaultSettings();
            settings.fake3DMode = DataStructs.Fake3DMode.DISABLED;
            settings.videoPort = 7777;
            settings.sensorPort = 5555;
            settings.videoQuality = (int) (renderQuality * 100);
#if USE_SYS_DRAW
            settings.convertImage = true;
#endif
            trinusManager.setSettings( settings );

            return trinusManager.connectAsync();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Disconnects the Trinus server from
        ///            a VR device client.                   </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void DisconnectTrinus()
        {
            lock( this )
            {
                Object.Destroy( screenBuffer );
                screenBuffer = null;

                if( trinusManager != null )
                {
#if DEBUG
                    Debug.Log( "Trinus END" );
#endif
                    trinusManager.disconnect();
                    Debug.Log( trinusManager.getSimpleLog() );
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Updates the sensor data on the server 
        ///            from readings from the client device. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void GrabSensorData()
        {
            DataStructs.SENSORS sensorData =
                            trinusManager.getSensorData ();

            acceleration.Set( sensorData.accelX, sensorData.accelY, sensorData.accelZ );
            rotation.Set(
                    sensorData.quatX,
                    sensorData.quatY,
                    sensorData.quatZ,
                    sensorData.quatW );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Initiates streaming to the device.    </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void StartStreaming()
        {
            SetGameResolution( screenBufferHeight );
            trinusManager.startStreaming();
            SetBufferResolution( Screen.width, Screen.height );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        /// <summary> Changes the VR device resolution.      </summary>
        /// <remarks>
        ///     This method should be called whenever there's a 
        ///     resolution change (and trinus is streaming). </remarks>
        /// <param name="screenWidth"> 
        ///     The new viewport width.                        </param>
        /// <param name="screenHeight"> 
        ///     The new viewport height.                       </param>
        /// <param name="fake3D"> 
        ///     Whether to draw true stereoscopic image or 
        ///     emulate it from a mono camera render.          </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void SetBufferResolution(
            int screenWidth,
            int screenHeight,
            DataStructs.Fake3DMode fake3D = DataStructs.Fake3DMode.DISABLED )
        {
            if( trinusManager != null )
            {
                // cache the screen resolution, 
                // to that we know when it changes
                lastScreenWidth = screenWidth;
                lastScreenHeight = screenHeight;

                screenBufferRect =
                    GetIdealBufferSize( screenWidth, screenHeight );

                int bufferWidth =  (int) screenBufferRect.width;
                int bufferHeight = (int) screenBufferRect.height;

                trinusManager.resetView( bufferWidth,
                                         bufferHeight,
                                         fake3D );
                lock( this )
                {
                    if( screenBuffer != null )
                    {
                        Object.Destroy( screenBuffer );
                        screenBuffer = null;
                    }
#if USE_SYS_DRAW
                    if( bitmap != null )
                    {
                        bitmap.Dispose();
                    }

                    bitmap = new System.Drawing.Bitmap(
                        bufferWidth,
                        bufferHeight,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb );
#endif
                    screenBuffer = new Texture2D(
                        bufferWidth,
                        bufferHeight,
                        TextureFormat.RGB24,
                        false );
                    screenBuffer.filterMode = FilterMode.Trilinear;
                }
            }
#if DEBUG
            Debug.Log( "Stream resolution set to " +
                (int) screenBufferRect.width + "x" +
                (int) screenBufferRect.height );
#endif
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Returns the ideal buffer screen area that
        ///            preserves the VR screen aspect ratio. </summary>
        /// <remarks>
        ///     When the game runs in windowed mode the window size
        ///     will automatically be set to a resoultion that fits
        ///     the aspect ratio of the HMD screen.
        /// </remarks>
        /// <param name="screenWidth">
        ///     The game screen width (desktop).               </param>
        /// <param name="screenHeight"></param>
        ///     The game screen height (desktop).              </param>
        /// <returns> 
        ///     The buffer size that fits the best 
        ///     in the game window.                          </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private Rect GetIdealBufferSize( int screenWidth, int screenHeight )
        {
            float bufferX = 0;
            float bufferY = 0;
            float bufferWidth = screenWidth;
            float bufferHeight = screenHeight;

            float gameScreenAspect = screenWidth / (float) screenHeight;

            // VR device is wider than the desktop
            if( deviceScreenAspect > gameScreenAspect )
            {
                bufferWidth = screenWidth;
                bufferHeight = bufferWidth / deviceScreenAspect;
                bufferX = 0;
                bufferY = (screenHeight - bufferHeight) / 2;
            }
            // VR device is taller than the desktop
            else if( deviceScreenAspect < gameScreenAspect )
            {
                bufferHeight = screenHeight;
                bufferWidth = bufferWidth * deviceScreenAspect;
                bufferX = (screenWidth - bufferWidth) / 2;
                bufferY = 0;
            }

            return new Rect( bufferX, bufferY, bufferWidth, bufferHeight );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets the game screen resolution.      </summary>
        /// <remarks>
        ///     The resolution is set to the aspect ratio of the 
        ///     HMD if possible and (for now) is capped at 1080p.
        ///     When the game is played in windowed mode, the
        ///     game resolution can be set to arbitrary height,
        ///     in fullscreen, the closest supported resolution
        ///     is selected in terms of screen area. Changing the 
        ///     game resolution will also update the resolution
        ///     of the VR device.                            </remarks>
        /// <param name="height"> The height of the resolution.</param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void SetGameResolution( int height )
        {
            int resHeight = 
                (int) Mathf.Min( height * renderScale, maxScreenHeight );
            int resWidth =  (int) (resHeight * deviceScreenAspect);

            if( Screen.fullScreen )
            {
                Resolution selected = 
                    FindClosestScreenResolution(resWidth, resHeight);

                if( selected.width != 0 )
                {
                    resWidth = selected.width;
                    resHeight = selected.height;
                }
            }

            if( Screen.width != resWidth || Screen.height != resHeight )
            {
                Screen.SetResolution( resWidth, resHeight, Screen.fullScreen );
            }

            SetBufferResolution( Screen.width, Screen.height );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        /// <summary> Returns a list of supported resolutions 
        ///           on the game host device.               </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static Resolution FindClosestScreenResolution( 
            int idealWidth, 
            int idealHeight )
        {
            const float areaWeight = 0.000001f;
            const float aspectWeight = 1;

            Resolution closestResolution = new Resolution();

            int idealResArea = idealWidth * idealHeight;
            float idealAspect = (float) idealWidth/idealHeight;
            float bestHeuristicScore = float.PositiveInfinity;

            foreach( Resolution resolution in Screen.resolutions )
            {
                int resolutionArea = 
                    resolution.width * resolution.height;

                float resolutionAspect = 
                    (float) resolution.width / resolution.height;

                int areaDifference =
                    Mathf.Abs( resolutionArea - idealResArea );

                float aspectDifference =
                    Mathf.Abs( resolutionAspect - idealAspect );

                float heuristicScore = areaDifference * areaWeight;
                heuristicScore += aspectDifference * aspectWeight;

                if( heuristicScore < bestHeuristicScore )
                {
                    bestHeuristicScore = heuristicScore;
                    closestResolution = resolution;
                }
            }

            return closestResolution;
        }
    }
}
#else

namespace Izzo.VR
{
    using System;
    using UnityEngine;

    public class VRDeviceTrinusImpl : IVRDevice
    {
        public bool enabled
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool isSupported { get { return false; } }

        public void DrawFrame()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void UpdateCamera( Camera camera )
        {
            throw new NotImplementedException();
        }
    }
}

#endif