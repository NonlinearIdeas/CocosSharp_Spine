using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CocosSharp;

namespace CocosSharp_Spine
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CCApplication(isFullScreen: false, mainWindowSizeInPixels: new CCSize(1024, 768))
            {
                ApplicationDelegate = new AppDelegate()
            };
            app.StartGame();
        }
    }
}
