using UnityEngine;

namespace CustomAvatar.Tracking.OpenVR
{
    internal class OpenVRDeviceState : ITrackedDeviceState
    {
        public DeviceUse use { get; }
        public uint deviceIndex { get; set; }
        public string modelName { get; set; }
        public string serialNumber { get; set; }
        public string role { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public bool isConnected { get; set; }
        public bool isTracking { get; set; }

        public OpenVRDeviceState(DeviceUse use)
        {
            this.use = use;

            deviceIndex = 0;
            modelName = null;
            serialNumber = null;
            role = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            isConnected = false;
            isTracking = false;
        }
    }
}
