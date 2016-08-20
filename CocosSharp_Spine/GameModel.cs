using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CocosSharp;

namespace CocosSharp_Spine
{
    public static class GameModel
    {
        static CCLayer _controls = null;
        static CCLayer _nodes = null;
        static CCLayer _back = null;

        static public void Init()
        {
            CCScene scene = Scenes.activeScene;

            _back = new CCLayer();
            _nodes = new CCLayer();
            _controls = new CCLayer();

            scene.AddChild(_back, 0);
            scene.AddChild(_nodes, 10);
            scene.AddChild(_controls, 20);

            CreateBackground();
            CreateAnimation();
            CreateControls();
        }

        static void CreateAnimation()
        {

        }

        static void CreateControls()
        {

        }

        static void CreateBackground()
        {
            var spr = new CCSprite("back_stars.png");
            spr.Position = Scenes.center;
            _back.AddChild(spr);
        }
    }
}
