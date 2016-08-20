using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CocosSharp;

namespace CocosSharp_Spine
{
    public enum AnimationType
    {
        IDLE,
        MOVE_START,
        MOVE_STOP,
        MOVING,
    }

    public abstract class AnimationController : CCNode
    {
        public abstract bool IsPlaying();
        public abstract bool IsLooping();
        public abstract void SetAnimation(AnimationType animType, String filter);

        public abstract void PlayAnimation(AnimationType animType, bool loop);
        public abstract void PauseAnimation();
        public abstract void ResumeAnimation();
    }

    public class ImageAnimationController : AnimationController
    {
        Dictionary<AnimationType, List<CCSpriteFrame>> _frames;
        String _animSheet = null;
        String _animSheetFilter = null;
        CCSprite _sprite = null;
        bool _isLooping = false;
        List<CCSpriteFrame> _activeFrames = null;
        Int32 _activeFramesIdx = 0;
        AnimationType _activeAnimation = AnimationType.IDLE;
        bool _isPlaying = false;
        float _frameIntervalSeconds = 1.0f;

        ImageAnimationController() { }

        List<CCSpriteFrame> CreateAnimationFrames(String filter)
        {

            var spriteSheet = new CCSpriteSheet(_animSheet);
            var result = spriteSheet.Frames.FindAll(x => x.TextureFilename.Contains(filter));

            Debug.Assert(result.Count > 0);

            return result;
        }

        void SetAnimationFrames(AnimationType animType)
        {
            Debug.Assert(_frames.ContainsKey(animType));

            _activeFrames = _frames[animType];
            _activeFramesIdx = 0;

            Debug.Assert(_activeFrames.Count > 0);
        }

        public override bool IsPlaying() { return _isPlaying; }

        public override bool IsLooping()
        {
            return _isLooping;
        }

        public ImageAnimationController(String animSheet, UInt32 framesPerSecond)
        {
            Debug.Assert(framesPerSecond <= 60);

            _animSheet = animSheet;
            _frames = new Dictionary<AnimationType, List<CCSpriteFrame>>();
            _frameIntervalSeconds = 1.0f / framesPerSecond;
        }

        public override void SetAnimation(AnimationType animType, String animFilter)
        {
            _frames[animType] = CreateAnimationFrames(animFilter);
        }

        public override void PlayAnimation(AnimationType animType, bool loop)
        {
            UnscheduleAll();
            // Store off the active animation.
            _activeAnimation = animType;
            // Load the frames into the active frames.
            SetAnimationFrames(_activeAnimation);
            // The very first animation frame?
            if(_sprite == null)
            {
                _sprite = new CCSprite(_activeFrames[0]);
                // Add the sprite to the display.
                AddChild(_sprite);
            }
            _isLooping = loop;
            _isPlaying = true;

            Schedule(UpdateAnimation, _frameIntervalSeconds);
        }

        public override void PauseAnimation()
        {
            UnscheduleAll();
        }

        public override void ResumeAnimation()
        {
            Schedule(UpdateAnimation, _frameIntervalSeconds);
        }

        void UpdateAnimation(float dt)
        {
            if(_isPlaying)
            {
                ++_activeFramesIdx;
                if(_activeFramesIdx >= _activeFrames.Count)
                {
                    if(_isLooping)
                    {   // Loop the animation.
                        _activeFramesIdx = 0;
                        _sprite.SpriteFrame = _activeFrames[_activeFramesIdx];
                    }
                    else
                    {   // Animation is finished.
                        _isPlaying = false;
                        _sprite.SpriteFrame = _activeFrames[_activeFrames.Count - 1];
                        UnscheduleAll();
                    }
                }
                else
                {
                    _sprite.SpriteFrame = _activeFrames[_activeFramesIdx];
                    //Utilities.Log("Frame {0} / {1} for animation [{2}].", _activeFramesIdx, _activeFrames.Count, _activeAnimation);
                }
            }
        }
    }
}
