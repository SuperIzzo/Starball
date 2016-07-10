namespace Izzo.VR
{
    using System;
    using UnityEngine;
    using UnityEngine.VR;

    public class VRDeviceSimpleSplitScreenImpl : IVRDevice
    {
        public virtual bool isSupported { get { return true; } }

        private bool _enabled;
        public bool enabled
        {
            get
            {
                return _enabled;
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

        protected virtual bool OnEnable()
        {
            VRSettings.loadedDevice = VRDeviceType.Split;
            VRSettings.enabled = true;

            return true;
        }

        protected virtual void OnDisable()
        {
            if( VRSettings.loadedDevice == VRDeviceType.Split )
            {
                VRSettings.enabled = false;
            }
        }

        public virtual void Update()
        {
            // HOOK            
        }

        public virtual void DrawFrame()
        {
            // HOOK
        }

        public virtual void UpdateCamera( Camera camera )
        {
            // HOOK
        }
    }
}