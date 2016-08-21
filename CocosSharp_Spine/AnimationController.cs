using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Spine;
using CocosSharp;

namespace CocosSharp_Spine
{
    public enum AnimationType
    {
        IDLE,
        MOVE_START,
        MOVE_STOP,
        JUMPING,
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
        public abstract void SetFrameRate(UInt32 fps);
    }

    /// <summary>
    /// Load animations and play them on demand.
    /// </summary>
    public class SkeletonAnimationController : AnimationController
    {
        AnimationState _state;
        bool _isLooping = false;
        bool _isPlaying = false;
        float _timeScale = 1.0f;
        CCSkeleton _skeleton;
        Dictionary<AnimationType, String> _animDict = null;

        SkeletonAnimationController() { }

        void UpdateAnimation(float dt)
        {
            if (_isPlaying)
            {
                _state.Update(dt * _timeScale);
                _state.Apply(_skeleton.Skeleton);
                _skeleton.UpdateWorldTransform();               
            }
            else
            {
                UnscheduleAll();
            }
        }

        public CCSkeleton skeleton {  get { return _skeleton; } }

        /// <summary>
        /// Initialize an instance of this controller with the root file
        /// name.  The name of the atlas, json, and texture file must all
        /// have the same root.  For example, "spineboy" to find the 
        /// files named spineboy.json, spineboy.atlas, and spineboy.png.
        /// </summary>
        /// <param name="rootFileName"></param>
        public SkeletonAnimationController(String rootFileName)
        {
            _skeleton = new CCSkeleton(rootFileName + ".json", rootFileName + ".atlas");
            _state = new AnimationState(new AnimationStateData(_skeleton.Skeleton.Data));
            _animDict = new Dictionary<AnimationType, string>();
            _state.Complete += AnimationCompleted;

            AddChild(_skeleton);
        }

        private void AnimationCompleted(AnimationState state, int trackIndex, int loopCount)
        {
            if(!_isLooping)
            {
                _isPlaying = false;
            }
        }

        /// <summary>
        /// Is the animation still playing?  If looping, this will always be true.
        /// </summary>
        /// <returns></returns>
        public override bool IsPlaying()
        {
            return _isPlaying;
        }

        /// <summary>
        /// Is the animation set to loop?
        /// </summary>
        /// <returns></returns>
        public override bool IsLooping()
        {
            return _isLooping;
        }

        /// <summary>
        /// Set the name of the animation to use for a given animation type.
        /// </summary>
        /// <param name="animType"></param>
        /// <param name="name"></param>
        public override void SetAnimation(AnimationType animType, string name)
        {
            _animDict[animType] = name;
        }

        /// <summary>
        /// Start playing an animation.
        /// </summary>
        /// <param name="animType"></param>
        /// <param name="loop"></param>
        public override void PlayAnimation(AnimationType animType, bool loop)
        {
            Debug.Assert(_animDict.ContainsKey(animType));
            Animation animation = _skeleton.Skeleton.Data.FindAnimation(_animDict[animType]);
            Debug.Assert(animation != null);

            UnscheduleAll();
            _state.SetAnimation(0, animation, loop);
            _isLooping = loop;
            _isPlaying = true;
            Schedule(UpdateAnimation);
        }

        /// <summary>
        /// Pause the active animation.  This is cleaner than
        /// setting the time rate to 0.
        /// </summary>
        public override void PauseAnimation()
        {
            UnscheduleAll();
        }

        /// <summary>
        /// Resume the animation.
        /// </summary>
        public override void ResumeAnimation()
        {
            Schedule(UpdateAnimation);
        }

        /// <summary>
        /// Set the frame rate for animation.  This can be between 1 and 60 fps,
        /// inclusive.
        /// </summary>
        /// <param name="fps"></param>
        public override void SetFrameRate(uint fps)
        {
            Debug.Assert(fps > 0);
            Debug.Assert(fps <= 60);
            _timeScale = fps / 60.0f;
        }

        /// <summary>
        /// Set the mix time between animations.
        /// </summary>
        /// <param name="animFrom"></param>
        /// <param name="animTo"></param>
        /// <param name="duration"></param>
        public void SetMix(AnimationType animFrom, AnimationType animTo, float duration)
        {
            Debug.Assert(_animDict.ContainsKey(animFrom));
            Debug.Assert(_animDict.ContainsKey(animTo));
            Debug.Assert(duration > 0);
     
            _state.Data.SetMix(_animDict[animFrom], _animDict[animTo], duration);
        }
    }

    public class ImageAnimationController : AnimationController
    {
        Dictionary<AnimationType, List<CCSpriteFrame>> _frames;
        String _animSheet = null;
        CCSprite _sprite = null;

        bool _isLooping = false;
        bool _isPlaying = false;
        float _frameIntervalSeconds = 1.0f / 60;

        List<CCSpriteFrame> _activeFrames = null;
        Int32 _activeFramesIdx = 0;
        AnimationType _activeAnimation = AnimationType.IDLE;

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

        public override void SetFrameRate(uint fps)
        {
            Debug.Assert(fps > 0);
            Debug.Assert(fps <= 60);
            UnscheduleAll();
            _frameIntervalSeconds = 1.0f / fps;
            Schedule(UpdateAnimation, _frameIntervalSeconds);
        }

        public override bool IsPlaying() { return _isPlaying; }

        public override bool IsLooping()
        {
            return _isLooping;
        }

        public ImageAnimationController(String animSheet)
        {

            _animSheet = animSheet;
            _frames = new Dictionary<AnimationType, List<CCSpriteFrame>>();
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
