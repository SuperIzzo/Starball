namespace Izzo.VR
{
    using System.Collections;
    using UnityEngine;

    public class VRManager : MonoBehaviour
    {
        private IVRDevice[] vrDevices;
        private IVRDevice activeVRDevice;        

        // Use this for initialization
        void Start()
        {
            activeVRDevice = new VRDeviceTrinusImpl();
            activeVRDevice.enabled = true;

            StartCoroutine( Stream() );
        }

        public void OnDestroy()
        {
            activeVRDevice.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if( activeVRDevice!=null )
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
                activeVRDevice.DrawFrame();
            }
        }

        private static IVRDevice[] GetRegisteredDevices()
        {
            return new IVRDevice[]
            {
                new VRDeviceSimpleSplitScreenImpl(),
                new VRDeviceTrinusImpl()
            };
        }
    }
}