using UnityEngine;

namespace CustomAvatar.Tracking
{
    internal interface ITrackedDeviceState
    {
        DeviceUse use { get; }
        Vector3 position { get; }
        Quaternion rotation { get; }
        bool isConnected { get; }
        bool isTracking { get; }
    }
}
