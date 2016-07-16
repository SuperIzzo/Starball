namespace Izzo.VR
{
    using System.Collections;
    using UnityEngine;

    public class VRManager : MonoBehaviour
    {
        [SerializeField]
        private string vrDevice;

        private IVRDevice[] vrDevices;
        private IVRDevice activeVRDevice;

        // Use this for initialization
        void Start()
        {
            activeVRDevice = GetDeviceForName(vrDevice);
            activeVRDevice.enabled = true;

            foreach( string dev in UnityEngine.VR.VRSettings.supportedDevices )
            {
                Debug.Log( dev );
            }

            StartCoroutine( Stream() );
        }

        public void OnDestroy()
        {
            if( activeVRDevice != null )
            {
                activeVRDevice.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if( activeVRDevice != null )
            {
                activeVRDevice.Update();
                activeVRDevice.UpdateCamera( Camera.main );
            }
        }

        IEnumerator Stream()
        {
            WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

            while( true )
            {
                yield return endOfFrame;

                if( activeVRDevice != null && activeVRDevice.enabled )
                {
                    activeVRDevice.DrawFrame();
                }
            }
        }

        private IVRDevice GetDeviceForName( string deviceName )
        {
            IVRDevice[] devices = GetRegisteredDevices();
            foreach( IVRDevice device in devices )
            {
                if( device.family == deviceName )
                {
                    return device;
                }
            }
            return null;
        }

        private static IVRDevice[] GetRegisteredDevices()
        {
            return new IVRDevice[]
            {
                new VRDeviceNULLImpl(),
                new VRDeviceSimpleSplitScreenImpl(),
                new VRDeviceTrinusImpl()
            };
        }
    }
}