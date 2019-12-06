/*using System.Linq;
using BeatSaberMarkupLanguage;
using CustomUI.Utilities;
using HMUI;
using UnityEngine;

namespace CustomAvatar.UI
{
    class AvatarListFlowCoordinator : FlowCoordinator
    {
        private GameObject mainScreen;
        private Vector3 mainScreenScale;
        private DismissableNavigationController rightNavigationController;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            mainScreen = GameObject.Find("MainScreen");
            mainScreenScale = mainScreen.transform.localScale;

            if (firstActivation)
            {
                title = "Custom Avatars";

                ViewController contentViewController = BeatSaberUI.CreateViewController<MirrorViewController>();
                ViewController leftViewController = BeatSaberUI.CreateViewController<SettingsViewController>();
                ViewController rightViewController = BeatSaberUI.CreateViewController<AvatarListViewController>();

                rightNavigationController = BeatSaberUI.CreateDismissableNavigationController();

                SetViewControllerToNavigationConctroller(rightNavigationController, rightViewController);
                ProvideInitialViewControllers(contentViewController, leftViewController, rightNavigationController);
                
                mainScreen.transform.localScale = Vector3.zero;
            }

            rightNavigationController.didFinishEvent += OnFinishEvent;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            rightNavigationController.didFinishEvent -= OnFinishEvent;
        }

        private void OnFinishEvent(DismissableNavigationController navigationController)
        {
            mainScreen.transform.localScale = mainScreenScale;
            var mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlowCoordinator.InvokePrivateMethod("DismissFlowCoordinator", new object[] { this, null, false });
        }
    }
}*/