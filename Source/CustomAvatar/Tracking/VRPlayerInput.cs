﻿using System;
using CustomAvatar.Avatar;
using CustomAvatar.Utilities;
using DynamicOpenVR.IO;
using UnityEngine;
using Zenject;

namespace CustomAvatar.Tracking
{
    internal class VRPlayerInput : IAvatarInput
    {
        public bool allowMaintainPelvisPosition => _avatarSettings.allowMaintainPelvisPosition;

        public event Action inputChanged;

        private readonly TrackedDeviceManager _deviceManager;
        private readonly LoadedAvatar _avatar;
        private readonly Settings _settings;
        private readonly Settings.AvatarSpecificSettings _avatarSettings;

        private readonly SkeletalInput _leftHandAnimAction;
        private readonly SkeletalInput _rightHandAnimAction;

        private Pose _previousWaistPose;
        private Pose _previousLeftFootPose;
        private Pose _previousRightFootPose;

        private bool _shouldTrackFullBody =>
            _avatar.isIKAvatar &&
            (
                _avatarSettings.bypassCalibration ||
                !_avatarSettings.useAutomaticCalibration && _avatarSettings.fullBodyCalibration.isCalibrated ||
                _avatarSettings.useAutomaticCalibration && _settings.automaticCalibration.isCalibrated
            );

        [Inject]
        internal VRPlayerInput(TrackedDeviceManager trackedDeviceManager, LoadedAvatar avatar, Settings settings, Settings.AvatarSpecificSettings avatarSettings)
        {
            _deviceManager = trackedDeviceManager;
            _avatar = avatar;
            _settings = settings;
            _avatarSettings = avatarSettings;

            _deviceManager.deviceAdded += OnDevicesUpdated;
            _deviceManager.deviceRemoved += OnDevicesUpdated;
            _deviceManager.deviceTrackingAcquired += OnDevicesUpdated;
            _deviceManager.deviceTrackingLost += OnDevicesUpdated;
            
            _leftHandAnimAction  = new SkeletalInput("/actions/customavatars/in/lefthandanim");
            _rightHandAnimAction = new SkeletalInput("/actions/customavatars/in/righthandanim");
        }

        public bool TryGetHeadPose(out Pose pose)      => TryGetPose(_deviceManager.head, out pose);
        public bool TryGetLeftHandPose(out Pose pose)  => TryGetPose(_deviceManager.leftHand, out pose);
        public bool TryGetRightHandPose(out Pose pose) => TryGetPose(_deviceManager.rightHand, out pose);

        public bool TryGetWaistPose(out Pose pose)
        {
            Pose correction;

            if (_avatarSettings.useAutomaticCalibration)
            {
                correction = _settings.automaticCalibration.pelvis;

                Quaternion rotationOffset = Quaternion.Euler(0, (int) _settings.automaticCalibration.waistTrackerPosition, 0);

                correction.position -= Quaternion.Inverse(rotationOffset) * (Vector3.forward * _settings.automaticCalibration.pelvisOffset);
                correction.rotation *= rotationOffset;
            }
            else
            {
                correction = _avatarSettings.fullBodyCalibration.pelvis;
            }

            if (!TryGetTrackerPose(_deviceManager.waist, _previousWaistPose, correction, _settings.fullBodyMotionSmoothing.waist, out pose))
            {
                return false;
            }

            _previousWaistPose = pose;
            return true;
        }

        public bool TryGetLeftFootPose(out Pose pose)
        {
            Pose correction;

            if (_avatarSettings.useAutomaticCalibration)
            {
                correction = _settings.automaticCalibration.leftLeg;
                correction.position -= Vector3.up * _settings.automaticCalibration.legOffset;
            }
            else
            {
                correction = _avatarSettings.fullBodyCalibration.leftLeg;
            }

            if (!TryGetTrackerPose(_deviceManager.leftFoot, _previousLeftFootPose, correction, _settings.fullBodyMotionSmoothing.feet, out pose))
            {
                return false;
            }

            _previousLeftFootPose = pose;
            return true;
        }

        public bool TryGetRightFootPose(out Pose pose)
        {
            Pose correction;

            if (_avatarSettings.useAutomaticCalibration)
            {
                correction = _settings.automaticCalibration.rightLeg;
                correction.position -= Vector3.up * _settings.automaticCalibration.legOffset;
            }
            else
            {
                correction = _avatarSettings.fullBodyCalibration.rightLeg;
            }

            if (!TryGetTrackerPose(_deviceManager.rightFoot, _previousRightFootPose, correction, _settings.fullBodyMotionSmoothing.feet, out pose))
            {
                return false;
            }

            _previousRightFootPose = pose;
            return true;
        }

        public bool TryGetLeftHandFingerCurl(out FingerCurl curl)
        {
            SkeletalSummaryData leftHandAnim = _leftHandAnimAction.summaryData;

            if (!_leftHandAnimAction.isActive || leftHandAnim == null)
            {
                curl = null;
                return false;
            }

            curl = new FingerCurl(leftHandAnim.thumbCurl, leftHandAnim.indexCurl, leftHandAnim.middleCurl, leftHandAnim.ringCurl, leftHandAnim.littleCurl);
            return true;
        }

        public bool TryGetRightHandFingerCurl(out FingerCurl curl)
        {
            SkeletalSummaryData rightHandAnim = _rightHandAnimAction.summaryData;

            if (!_rightHandAnimAction.isActive || rightHandAnim == null)
            {
                curl = null;
                return false;
            }

            curl = new FingerCurl(rightHandAnim.thumbCurl, rightHandAnim.indexCurl, rightHandAnim.middleCurl, rightHandAnim.ringCurl, rightHandAnim.littleCurl);
            return true;
        }

        public void Dispose()
        {
            _deviceManager.deviceAdded -= OnDevicesUpdated;
            _deviceManager.deviceRemoved -= OnDevicesUpdated;
            _deviceManager.deviceTrackingAcquired -= OnDevicesUpdated;
            _deviceManager.deviceTrackingLost -= OnDevicesUpdated;

            _leftHandAnimAction.Dispose();
            _rightHandAnimAction.Dispose();
        }

        private bool TryGetPose(TrackedDeviceState device, out Pose pose)
        {
            if (!device.found || !device.tracked)
            {
                pose = Pose.identity;
                return false;
            }

            pose = new Pose(device.position, device.rotation);
            return true;
        }

        private bool TryGetTrackerPose(TrackedDeviceState device, Pose previousPose, Pose correction, Settings.TrackedPointSmoothing smoothing, out Pose pose)
        {
            if (!_shouldTrackFullBody || !TryGetPose(device, out Pose currentPose))
            {
                pose = Pose.identity;
                return false;
            }

            Quaternion correctedRotation = currentPose.rotation * correction.rotation;
            Vector3 correctedPosition = currentPose.position + correctedRotation * correction.position; // correction is forward-facing by definition

            pose = new Pose(Vector3.Lerp(previousPose.position, correctedPosition, smoothing.position), Quaternion.Slerp(previousPose.rotation, correctedRotation, smoothing.rotation));
            return true;
        }

        private void OnDevicesUpdated(TrackedDeviceState state, DeviceUse use)
        {
            inputChanged?.Invoke();
        }
    }
}
