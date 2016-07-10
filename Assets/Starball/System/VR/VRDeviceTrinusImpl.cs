#define TRINUS_VR
#define USE_SYS_DRAW


using corelib.util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Izzo.VR
{
    public class VRDeviceTrinusImpl : VRDeviceSimpleSplitScreenImpl
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary> </summary>
        //::::::::::::::::::::::::::::::::::
#if TRINUS_VR        
        public override bool isSupported { get { return true; } }
#else
        public override bool isSupported { get { return false; } }
#endif


#if TRINUS_VR         
        private string projectID;
        private corelib.Manager trinusManager;

        // Camera
        private Vector3 acceleration;
        private Quaternion rotation;

        // Streaming
#if USE_SYS_DRAW
        private System.Drawing.Bitmap bitmap = null;
        private System.Drawing.Rectangle bitsRect;
#endif
        private byte[] bytes = null;
        private Texture2D output = null;
        private Rect screenRect = new Rect();
        private float displayScale = 1;


        protected override bool OnEnable()
        {
            bool success = base.OnEnable();

            if( success )
            {
                success &= InitTrinus();
            }

            return success;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            Disconnect();
        }

        private bool InitTrinus()
        {
            trinusManager = new corelib.Manager( projectID );

            return true;
        }

        public override void Update()
        {
            if( enabled )
            {
                MaintainConnection();
            }
        }

        private void MaintainConnection()
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
                    DataStructs.RESULT result = Connect();

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
                    DataStructs.DEVICE deviceInfo = trinusManager.getDeviceInfo ();
                    int targetHeight = Mathf.Min( deviceInfo.height, Screen.height );

                    //screen resolution should be equal or lower than device resolution
                    //use resolution to balance quality vs performance
                    //note: fullscreen mode will have resolution options restricted
                    SetScreenResolution( Mathf.Min( 720, targetHeight ) );

                    trinusManager.startStreaming();
                    ChangeResolution( Screen.width, Screen.height );
                    break;

                case DataStructs.STATUS.STREAMING:
                    DataStructs.SENSORS sensorData =
                            trinusManager.getSensorData ();

                    acceleration.Set( sensorData.accelX, sensorData.accelY, sensorData.accelZ );
                    rotation.Set(
                            sensorData.quatX,
                            sensorData.quatY,
                            sensorData.quatZ,
                            sensorData.quatW );
                    break;
            }
        }

        public override void UpdateCamera( Camera camera )
        {
            camera.transform.localRotation = rotation;
        }

        private DataStructs.RESULT Connect()
        {
            DataStructs.SETTINGS settings = DataStructs.SETTINGS.defaultSettings();
            settings.fake3DMode = DataStructs.Fake3DMode.DISABLED;
            settings.videoPort = 7777;
            settings.sensorPort = 5555;
            //settings.videoFormat = DataStructs.VideoFormat.H264;
            settings.videoQuality = 100;
#if USE_SYS_DRAW
            settings.convertImage = true;
#endif

            trinusManager.setSettings( settings );

            return trinusManager.connectAsync();
        }

        private void Disconnect()
        {
            lock( this )
            {
                UnityEngine.Object.Destroy( output );
                output = null;

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

        //<summary>
        //This method should be called whenever there's a resolution change (and trinus is streaming)
        //</summary>
        public void ChangeResolution( int width, int height )
        {
            ChangeResolution( width, height, DataStructs.Fake3DMode.DISABLED );
        }

        //<summary>
        //This method should be called whenever there's a resolution change (and trinus is streaming)
        //</summary>
        public void ChangeResolution( int width, int height, DataStructs.Fake3DMode fake3D )
        {
            if( trinusManager != null )
            {
                trinusManager.resetView( width, height, fake3D );

                lock( this )
                {
                    if( output != null )
                        UnityEngine.Object.Destroy( output );

                    output = null;

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

                    bitmap = new System.Drawing.Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
                    bitsRect = new System.Drawing.Rectangle( 0, 0, width, height );
#endif
                    output = new Texture2D( width, height, TextureFormat.RGB24, false );
                    output.filterMode = FilterMode.Trilinear;
                }
            }
#if DEBUG
            Debug.Log( "Stream resolution set to " + Screen.width + "x" + Screen.height );
#endif
        }

        public override void DrawFrame()
        {
            if( trinusManager.getStatus() == DataStructs.STATUS.STREAMING )
            {
                if( screenRect.width != Screen.width || screenRect.height != Screen.height )
                {
                    ChangeResolution( Screen.width, Screen.height );
                }

                lock( this )
                {
                    if( output != null && trinusManager.isReadyForFrame() )
                    {
                        output.ReadPixels( screenRect, 0, 0, false );
#if USE_SYS_DRAW
                        bytes = output.GetRawTextureData();

                        trinusManager.sendNextFrame( bytes,
                            bitsRect.Width,
                            bitsRect.Height,
                            100 );
#else
                        bytes = output.EncodeToJPG( 100 );
                        trinusManager.sendNextFrame( bytes );
#endif
                    }
                }
            }
        }

        public void SetScreenResolution( int h )
        {
            int resHeight = (int) Mathf.Min(1080, h * displayScale);
            int resWidth = (int) (resHeight * (16 / 9f));

            if( Screen.fullScreen )
            {
                Resolution[] res = GetValidResolutions();
                Resolution selected = new Resolution();
                int closest = 99999999;
                foreach( Resolution r in res )
                {
                    if( Mathf.Abs( r.width * r.height - resWidth * resHeight )
                        < Mathf.Abs( closest - resWidth * resHeight ) )
                    {
                        closest = r.width * r.height;
                        selected = r;
                    }
                }
                if( selected.width != 0 )
                {
                    resWidth = selected.width;
                    resHeight = selected.height;
                }

            }

            if( Screen.width != resWidth || Screen.height != resHeight )
                Screen.SetResolution( resWidth, resHeight, Screen.fullScreen );

            ChangeResolution( Screen.width, Screen.height );
        }

        public static Resolution[] GetValidResolutions()
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

#endif
    }
}