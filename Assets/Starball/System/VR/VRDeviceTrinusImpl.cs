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
    public class VRDeviceTrinusImpl : VRDeviceSimpleSplitScreenImpl
    {
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
                    SetGameResolution( deviceScreenHeight );
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
        /// <summary>Gets the screen height on the VR device.</summary>
        //..................................
        private int deviceScreenHeight
        {
            get
            {
                DataStructs.DEVICE deviceInfo = trinusManager.getDeviceInfo ();
                int targetHeight = Mathf.Min( deviceInfo.height, Screen.height );
                targetHeight = Mathf.Min( 720, targetHeight );
                return targetHeight;
            }
        }

        //-------------------------------------------------------------
        /// <summary>  A unique trinus project id.           </summary>
        //----------------------------------
        private string projectID;

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
        private Rect screenRect = new Rect();

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
                if( screenRect.width != Screen.width || 
                    screenRect.height != Screen.height )
                {
                    SetVRDeviceResolution( Screen.width, Screen.height );
                }

                lock( this )
                {
                    if( screenBuffer != null && trinusManager.isReadyForFrame() )
                    {
                        screenBuffer.ReadPixels( screenRect, 0, 0, false );
#if USE_SYS_DRAW
                        byte[] bytes = screenBuffer.GetRawTextureData();

                        trinusManager.sendNextFrame( bytes,
                            (int) screenRect.width,
                            (int) screenRect.height,
                            (int) (renderQuality*100) );
#else
                        byte[] bytes = output.EncodeToJPG( 100 );
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
            settings.videoQuality = (int) (renderQuality*100);
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
            SetGameResolution( deviceScreenHeight );

            base.OnEnable();

            trinusManager.startStreaming();
            SetVRDeviceResolution( Screen.width, Screen.height );
        }   

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        /// <summary> Changes the VR device resolution.      </summary>
        /// <remarks>
        ///     This method should be called whenever there's a 
        ///     resolution change (and trinus is streaming). </remarks>
        /// <param name="width"> The new viewport width.       </param>
        /// <param name="height"> The new viewport height.     </param>
        /// <param name="fake3D"> 
        ///     Whether to draw true stereoscopic image or 
        ///     emulate it from a mono camera render.          </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void SetVRDeviceResolution(
            int width,
            int height,
            DataStructs.Fake3DMode fake3D = DataStructs.Fake3DMode.DISABLED )
        {
            if( trinusManager != null )
            {
                trinusManager.resetView( width, height, fake3D );

                lock( this )
                {
                    if( screenBuffer != null )
                    {
                        Object.Destroy( screenBuffer );
                        screenBuffer = null;
                    }

                    screenRect.x = 0;
                    screenRect.y = 0;
                    screenRect.width = width;
                    screenRect.height = height;
#if USE_SYS_DRAW
                    //adjusting width to avoid stride padding in bitmap
                    width = Mathf.FloorToInt( width * 3f / 4f ) / 3 * 4;

                    if( bitmap != null )
                    {
                        bitmap.Dispose();
                    }

                    bitmap = new System.Drawing.Bitmap(
                        width,
                        height,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb );
#endif
                    screenBuffer = new Texture2D(
                        width,
                        height,
                        TextureFormat.RGB24,
                        false );
                    screenBuffer.filterMode = FilterMode.Trilinear;
                }
            }
#if DEBUG
            Debug.Log( "Stream resolution set to " + Screen.width + "x" + Screen.height );
#endif
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets the game screen resolution.      </summary>
        /// <remarks>
        ///     For best results the resolution aspect is set 
        ///     to 16:9, and due to limitation the height on the 
        ///     VR device is capped to 1080p.
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
            int resHeight = (int) Mathf.Min(1080, height * renderScale);
            int resWidth = (int) (resHeight * (16 / 9f));

            if( Screen.fullScreen )
            {
                Resolution[] resolutions = GetValidGameResolutions();
                Resolution selected = new Resolution();

                int idealResArea = resWidth * resHeight;
                int lowestAreaDifference = int.MaxValue;

                foreach( Resolution resolution in resolutions )
                {
                    int currentResArea = resolution.width * resolution.height;
                    int areaDifference =
                        Mathf.Abs( currentResArea - idealResArea );

                    if( areaDifference < lowestAreaDifference )
                    {
                        lowestAreaDifference = areaDifference;
                        selected = resolution;
                    }
                }

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

            SetVRDeviceResolution( Screen.width, Screen.height );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
        /// <summary> Returns a list of supported resolutions 
        ///           on the game host device.               </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static Resolution[] GetValidGameResolutions()
        {
            Resolution[] res = Screen.resolutions;
            LinkedList<Resolution> vRes = new LinkedList<Resolution> ();
            foreach( Resolution r in res )
            {
                if( (float) r.width / r.height > 1.6 )//widescreen only
                {
                    vRes.AddFirst( r );
                }
            }

            Resolution[] result = new Resolution[vRes.Count];
            vRes.CopyTo( result, 0 );

            return result;
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