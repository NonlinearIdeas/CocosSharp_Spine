## CocosSharp Spine
This project provides a working example of Spine animation using CocosSharp.

The SkeletonAnimationController class is intended for use in a simple game where Actions are run on Entities to create motion, play an animation, etc.  An animation plays until it either finishes or until a new animation is started by a new action.  Because of this, stacking up animations is not necessary (at least at this point).

## Versions
Visual Studio 2015 Community Edition

CocosSharp v1.6.2 [(nuget)](https://www.nuget.org/packages/CocosSharp.PCL.Shared/) [(github)](https://github.com/mono/CocosSharp)

Spine C# Runtime v2.1.18.1  [(nuget)](https://www.nuget.org/packages/Spine/) [(github)](https://github.com/EsotericSoftware/spine-runtimes)

## Example Usage

```
        void CreateAnimationController()
        {
            String name = @"spineboy";
            animController = new SkeletonAnimationController(name);
            animController.Scale = 0.25f;
            animController.SetAnimation(AnimationType.MOVING, "run");
            animController.SetFrameRate(60);
            animController.PlayAnimation(AnimationType.MOVING, true);
            animController.Position = Scenes.center;

            AddChild(animController);
        }
```