using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CocosSharp;

namespace CocosSharp_Spine
{
    class AppDelegate : CCApplicationDelegate
    {
        void SetupLogger()
        {
            // Example of using a custom log action as a Lambda.
            CCLog.Logger = (format, args) =>
            {
                System.Diagnostics.Debug.WriteLine(String.Format("[{0}] ", DateTime.Now) + format, args);
            };
        }

        void SetupContent(CCApplication application)
        {
            application.ContentRootDirectory = "Content";
        }

        void SetupManagers(CCApplication application, CCWindow mainWindow)
        {
            Scenes.Init(application, mainWindow);
        }

        void SetupGameModel()
        {
            GameModel.Init();
        }

        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {
            base.ApplicationDidFinishLaunching(application, mainWindow);

            CCSize windowSize = mainWindow.WindowSizeInPixels;
            float desiredWidth = mainWindow.WindowSizeInPixels.Width;
            float desiredHeight = mainWindow.WindowSizeInPixels.Height;

            // This will set the world bounds to (0,0, w, h)
            // CCSceneResolutionPolicy.ShowAll will ensure that the aspect ratio is preserved
            CCScene.SetDefaultDesignResolution(desiredWidth, desiredHeight, CCSceneResolutionPolicy.ShowAll);

            SetupLogger();
            SetupContent(application);
            SetupManagers(application,mainWindow);
            SetupGameModel();
        }

        public override void ApplicationDidEnterBackground(CCApplication application)
        {
            base.ApplicationDidEnterBackground(application);
        }

        public override void ApplicationWillEnterForeground(CCApplication application)
        {
            base.ApplicationWillEnterForeground(application);
        }
    }
}
