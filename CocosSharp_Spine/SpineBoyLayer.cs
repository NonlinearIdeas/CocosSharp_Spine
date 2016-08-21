using System;
using CocosSharp;
using Microsoft.Xna.Framework;
using Spine;

namespace CocosSharp_Spine
{
    class SpineBoyLayer : CCLayer
    {
        CCSkeletonAnimation skeletonNode;
        SkeletonAnimationController animController;

        CCMenuItemFont labelBones, labelSlots, labelTimeScaleUp, labelTimeScaleDown, labelJump;
        CCMenu menu;

        void CreateSkeletonAnimation()
        {
            String name = @"spineboy";
            skeletonNode = new CCSkeletonAnimation(name + ".json", name + ".atlas");
            skeletonNode.Scale = 0.5f;

            skeletonNode.SetMix("walk", "jump", 0.2f);
            skeletonNode.SetMix("jump", "run", 0.2f);
            skeletonNode.SetAnimation(0, "walk", true);
            skeletonNode.AddAnimation(0, "jump", false, 3);
            skeletonNode.AddAnimation(0, "run", true);

            skeletonNode.Start += Start;
            skeletonNode.End += End;
            skeletonNode.Complete += Complete;
            skeletonNode.Event += Event;

            AddChild(skeletonNode);
        }

        void CreateAnimationController()
        {
            String name = @"spineboy";
            animController = new SkeletonAnimationController(name);
            animController.Scale = 0.25f;
            animController.SetAnimation(AnimationType.MOVING, "run");
            animController.SetFrameRate(30);
            animController.PlayAnimation(AnimationType.MOVING, true);
            animController.Position = Scenes.center;

            AddChild(animController);
        }


        public SpineBoyLayer()
        {
            CCMenuItemFont.FontName = "arial";
            CCMenuItemFont.FontSize = 12;

            labelBones = new CCMenuItemFont("B = Toggle Debug Bones", (obj) =>
                {
                    skeletonNode.DebugBones = !skeletonNode.DebugBones;
                    animController.skeleton.DebugBones = !animController.skeleton.DebugBones;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelSlots = new CCMenuItemFont("M = Toggle Debug Slots", (obj) =>
                {
                    skeletonNode.DebugSlots = !skeletonNode.DebugSlots;
                    animController.skeleton.DebugSlots = !animController.skeleton.DebugSlots;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelTimeScaleUp = new CCMenuItemFont("Up - TimeScale +", (obj) =>
                {
                    skeletonNode.TimeScale += 0.1f;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelTimeScaleDown = new CCMenuItemFont("Down - TimeScale -", (obj) =>
                {
                    skeletonNode.TimeScale -= 0.1f;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelJump = new CCMenuItemFont("J = Jump", (obj) =>
                {
                    // I truthfully do not know if this is how it is done or not
                    skeletonNode.SetAnimation(0, "jump", false);
                    skeletonNode.AddAnimation(0, "run", true);
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            menu = new CCMenu(labelBones, labelSlots, labelTimeScaleUp, labelTimeScaleDown, labelJump);
            menu.AlignItemsVertically();
            AddChild(menu);

            CreateSkeletonAnimation();
            CreateAnimationController();

            var listener = new CCEventListenerTouchOneByOne();
            listener.OnTouchBegan = (touch, touchEvent) =>
                {
                    if (!skeletonNode.DebugBones)
                    {
                        skeletonNode.DebugBones = true; 
                    }
                    else if (skeletonNode.TimeScale == 1)
                        skeletonNode.TimeScale = 0.3f;
                    return true;
                };
			AddEventListener(listener, this);

            var keyListener = new CCEventListenerKeyboard();
            keyListener.OnKeyPressed = (keyEvent) =>
                {
                    switch (keyEvent.Keys)
                    {
                        case CCKeys.B:
                            skeletonNode.DebugBones = !skeletonNode.DebugBones;
                            animController.skeleton.DebugBones = !animController.skeleton.DebugBones;
                            break;
                        case CCKeys.M:
                            skeletonNode.DebugSlots = !skeletonNode.DebugSlots;
                            animController.skeleton.DebugSlots = !animController.skeleton.DebugSlots;
                            break;
                        case CCKeys.Up:
                            skeletonNode.TimeScale += 0.1f;
                            break;
                        case CCKeys.Down:
                            skeletonNode.TimeScale -= 0.1f;
                            break;
                        case CCKeys.G:
                            break;
						case CCKeys.J:
							// I truthfully do not know if this is how it is done or not
							skeletonNode.SetAnimation(0, "jump", false);
							skeletonNode.AddAnimation(0, "run", true);
							break;
                    }

                };
			AddEventListener(keyListener, this);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            var windowSize = VisibleBoundsWorldspace.Size;

            menu.Position = new CCPoint(15, windowSize.Height - 70);

			skeletonNode.Position = new CCPoint(windowSize.Center.X, 10);

		}

        public void Start(AnimationState state, int trackIndex)
        {
            var entry = state.GetCurrent(trackIndex);
            var animationName = (entry != null && entry.Animation != null) ? entry.Animation.Name : string.Empty;

            CCLog.Log(trackIndex + ":start " + animationName);
        }

        public void End(AnimationState state, int trackIndex)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": end");
        }

        public void Complete(AnimationState state, int trackIndex, int loopCount)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": complete " + loopCount);
        }

        public void Event(AnimationState state, int trackIndex, Event e)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": event " + e);
        }
    }
}

