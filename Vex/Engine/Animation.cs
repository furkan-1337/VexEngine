using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;
using Vex.Engine.Assets;

namespace Vex.Engine
{
    public class Animation
    {
        public List<SubTexture> Frames { get; private set; }
        public float FrameDuration { get; set; } = 0.1f;
        public bool IsLooping { get; set; } = true;
        public bool IsPlaying { get; private set; } = true;
        public int CurrentFrameIndex { get; private set; } = 0;
        public int NextFrameIndex { get; private set; } = 0;
        private float _timer = 0.0f;
        private bool _hasStarted = false;
        public SubTexture CurrentFrame => Frames[CurrentFrameIndex];
        public bool IsFinished => !IsLooping && CurrentFrameIndex == Frames.Count - 1;


        public delegate void AnimationHandler(Animation animation, int previousFrameIndex, int currentFrameIndex);
        public event AnimationHandler? AnimationStarted;
        public event AnimationHandler? AnimationUpdated;
        public event AnimationHandler? AnimationFinished;
        public event AnimationHandler? AnimationLooped;

        public Animation(List<SubTexture> frames, float frameDuration = 0.1f, bool isLooping = true)
        {
            if (frames == null || frames.Count == 0)
                throw new VexException("Animation frames cannot be null or empty.");
            Frames = frames;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            CurrentFrameIndex = 0;
            NextFrameIndex = 0;
        }

        public Animation(SpriteSheet sheet, int startFrame, int frameCount, float frameDuration = 0.1f, bool isLooping = true)
        {
            if (frameCount <= 0)
                throw new VexException("Frame count must be greater than zero.");

            if (sheet == null)
                throw new VexArgumentNullException($"SpriteSheet ({nameof(sheet)}) was null.");

            Frames = new List<SubTexture>();
            for (int i = 0; i < frameCount; i++)
            {
                Frames.Add(sheet.GetSprite(startFrame + i));
            }
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            CurrentFrameIndex = 0;
            NextFrameIndex = 0;
        }
        public void Update(double deltaTime)
        {
            if (!IsPlaying || Frames.Count <= 1) return;

            if (!_hasStarted)
            {
                _hasStarted = true;
                AnimationStarted?.Invoke(this, CurrentFrameIndex, NextFrameIndex);
            }

            _timer += (float)deltaTime;

            if (_timer >= FrameDuration)
            {
                _timer -= FrameDuration;

                bool hasLooped = false;
                bool hasFinished = false;

                if (CurrentFrameIndex < Frames.Count - 1)
                {
                    NextFrameIndex = CurrentFrameIndex + 1;
                }
                else if (IsLooping)
                {
                    NextFrameIndex = 0;
                    hasLooped = true;
                }
                else
                {
                    NextFrameIndex = CurrentFrameIndex;
                    IsPlaying = false;
                    hasFinished = true;
                }

                int previousFrame = CurrentFrameIndex;
                CurrentFrameIndex = NextFrameIndex;

                if (hasFinished)
                {
                    AnimationFinished?.Invoke(this, previousFrame, CurrentFrameIndex);
                }
                else if (hasLooped)
                {
                    AnimationLooped?.Invoke(this, previousFrame, CurrentFrameIndex);
                }
                else
                {
                    AnimationUpdated?.Invoke(this, previousFrame, CurrentFrameIndex);
                }
            }
        }
        public void Play()
        {
            if (IsFinished)
                Reset(); 

            IsPlaying = true;
        }
        public void Pause()
        {
            IsPlaying = false;
        }

        public void Reset()
        {
            CurrentFrameIndex = 0;
            NextFrameIndex = 0;
            _timer = 0.0f;
            _hasStarted = false;
        }
        public void Stop()
        {
            Pause();
            Reset();
        }
    }
}
