using UnityEngine;

namespace CustomAvatar.Tracking.UnityXR
{
    internal class UnityXRDeviceState : ITrackedDeviceState
    {
        public DeviceUse use { get; }
        public string name { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public bool isConnected { get; set; }
        public bool isTracking { get; set; }

        public UnityXRDeviceState(DeviceUse use)
        {
            this.use = use;

            name = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            isConnected = false;
            isTracking = false;
        }
    }
}
