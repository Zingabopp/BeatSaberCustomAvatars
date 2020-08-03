using System;
using Valve.VR;

namespace CustomAvatar.Tracking.OpenVR
{
    internal class OpenVRException : Exception
    {
        public ETrackedDeviceProperty Property { get; }
        public ETrackedPropertyError Error { get; }

        public OpenVRException(string message, ETrackedDeviceProperty property, ETrackedPropertyError error) : base(message)
        {
            Property = property;
            Error = error;
        }
    }
}
