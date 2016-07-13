namespace Izzo.VR
{
    using System;
    using UnityEngine;
    using UnityEngine.VR;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  A null VR device implementation.      </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public sealed class VRDeviceNULLImpl : IVRDevice
    {
        public bool enabled
        {
            get { return false; }
            set
            {
                if( value )
                {
                    VRSettings.loadedDevice = VRDeviceType.None;
                }
            }
        }        

        public bool isSupported { get { return true; } }
        public string family { get { return "None"; } }
        public string model { get { return "None"; } }

        public float renderQuality { get; set; }    
        public float renderScale { get; set; }        

        public void DrawFrame() {}
        public void Update() {}
        public void UpdateCamera( Camera camera ) {}
    }
}