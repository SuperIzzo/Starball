using UnityEngine;

namespace Izzo.VR
{
    public interface IVRDevice
    {
        bool isSupported { get; }
        bool enabled { get; set; }

        void Update();
        void DrawFrame();
        void UpdateCamera( Camera camera );
    }
}