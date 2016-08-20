using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CocosSharp;

namespace CocosSharp_Spine
{
    public static class Scenes
    {
        static CCApplication _application = null;
        static CCWindow _mainWindow = null;
        static CCScene _activeScene = null;
        static CCDirector _director = null;

        public static void Init(CCApplication application, CCWindow mainWindow)
        {
            _application = application;
            _mainWindow = mainWindow;
            _director = new CCDirector();
            _mainWindow.AddSceneDirector(_director);
            _activeScene = new CCScene(_mainWindow);
            _mainWindow.RunWithScene(_activeScene);
            _mainWindow.StatsScale = 1;
        }

        public static CCPoint center
        {
            get
            {
                return new CCPoint(_activeScene.ContentSize.Width / 2, 
                    _activeScene.ContentSize.Height / 2);
            }
        }

        public static void ExitApplication()
        {
            // Disabled for now as it throws an exception!!!
            //_application.ExitGame();
        }

        public static void ToggleStats()
        {
            _mainWindow.DisplayStats = !_mainWindow.DisplayStats;
        }

        public static bool statsShowing { get { return _mainWindow.DisplayStats; } set { _mainWindow.DisplayStats = value; } }

        public static CCSize windowSize
        {
            get
            {
                return _mainWindow.WindowSizeInPixels;
            }
        }

        public static CCScene activeScene {  get { return _activeScene; } }
    }
}
