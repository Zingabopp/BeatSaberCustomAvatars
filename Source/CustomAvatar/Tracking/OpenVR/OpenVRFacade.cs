using DynamicOpenVR.Exceptions;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Animations;
using Valve.VR;

namespace CustomAvatar.Tracking.OpenVR
{
    using OpenVR = Valve.VR.OpenVR;

    internal class OpenVRFacade
    {
        public string[] GetTrackedDeviceSerialNumbers()
        {
            string[] serialNumbers = new string[OpenVR.k_unMaxTrackedDeviceCount];

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                serialNumbers[i] = GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_SerialNumber_String);
            }

            return serialNumbers;
        }

        public int GetInt32TrackedDeviceProperty(uint deviceIndex, ETrackedDeviceProperty property)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            int result = OpenVR.System.GetInt32TrackedDeviceProperty(deviceIndex, property, ref error);

            if (error != ETrackedPropertyError.TrackedProp_Success)
            {
                throw new OpenVRException($"Failed to get property '{property}' for device at index {deviceIndex}", property, error);
            }

            return result;
        }

        public float GetFloatTrackedDeviceProperty(uint deviceIndex, ETrackedDeviceProperty property)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            float result = OpenVR.System.GetFloatTrackedDeviceProperty(deviceIndex, property, ref error);

            if (error != ETrackedPropertyError.TrackedProp_Success)
            {
                throw new OpenVRException($"Failed to get property '{property}' for device at index {deviceIndex}", property, error);
            }

            return result;
        }

        public string GetStringTrackedDeviceProperty(uint deviceIndex, ETrackedDeviceProperty property)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            uint length = OpenVR.System.GetStringTrackedDeviceProperty(deviceIndex, property, null, 0, ref error);

            if (error != ETrackedPropertyError.TrackedProp_Success && error != ETrackedPropertyError.TrackedProp_BufferTooSmall)
            {
                throw new OpenVRException($"Failed to get property '{property}' for device at index {deviceIndex}: {error}", property, error);
            }

            if (length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder((int)length);
                OpenVR.System.GetStringTrackedDeviceProperty(deviceIndex, property, stringBuilder, length, ref error);

                if (error != ETrackedPropertyError.TrackedProp_Success)
                {
                    throw new OpenVRException($"Failed to get property '{property}' for device at index {deviceIndex}: {error}", property, error);
                }

                return stringBuilder.ToString();
            }

            return null;
        }
    }
}
