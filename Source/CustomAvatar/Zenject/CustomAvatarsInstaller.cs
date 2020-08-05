using System.Linq;
using CustomAvatar.Avatar;
using CustomAvatar.Configuration;
using CustomAvatar.Lighting;
using CustomAvatar.Logging;
using CustomAvatar.StereoRendering;
using CustomAvatar.Tracking.OpenVR;
using CustomAvatar.Tracking.UnityXR;
using CustomAvatar.Utilities;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Zenject;
using Logger = IPA.Logging.Logger;

namespace CustomAvatar.Zenject
{
    internal class CustomAvatarsInstaller : Installer
    {
        private Logger _logger;

        public CustomAvatarsInstaller(Logger logger)
        {
            _logger = logger;
        }

        public override void InstallBindings()
        {
            // logging
            Container.Bind<ILoggerProvider>().FromMethod((context) => context.Container.Instantiate<IPALoggerProvider>(new object[] { _logger })).AsTransient();

            // settings
            Container.BindInterfacesAndSelfTo<SettingsManager>().AsSingle();
            Container.Bind<Settings>().FromMethod((context) => context.Container.Resolve<SettingsManager>().settings);
            Container.BindInterfacesAndSelfTo<CalibrationData>().AsSingle();

            if (XRSettings.loadedDeviceName.Equals("openvr", System.StringComparison.InvariantCultureIgnoreCase) && OpenVR.IsRuntimeInstalled())
            {
                Container.BindInterfacesAndSelfTo<OpenVRFacade>().AsTransient();
                Container.BindInterfacesTo<OpenVRDeviceManager>().AsSingle().NonLazy();
            }
            else
            {
                Container.BindInterfacesTo<UnityXRDeviceManager>().AsSingle().NonLazy();
            }

            // managers
            Container.BindInterfacesAndSelfTo<PlayerAvatarManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MainCameraController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<KeyboardInputHandler>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShaderLoader>().AsSingle().NonLazy();

            Container.Bind<StereoRenderManager>().AsSingle();
            Container.Bind<AvatarLoader>().AsSingle();

            // helper classes
            Container.Bind<AvatarTailor>().AsTransient();
            Container.Bind<MirrorHelper>().AsTransient();
            Container.Bind<AvatarSpawner>().AsTransient();
            Container.Bind<GameScenesHelper>().AsTransient();

            // behaviours
            Container.Bind<MenuLightingController>().FromNewComponentOnNewGameObject().NonLazy();

            // not sure if this is a great idea but w/e
            if (!Container.HasBinding<MainSettingsModelSO>())
            {
                Container.Bind<MainSettingsModelSO>().FromInstance(Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().First());
            }
        }
    }
}
