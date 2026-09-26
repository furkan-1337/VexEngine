using System;
using System.Collections.Generic;
using Vex.Debugging;
using Vex.Engine.Assets;

namespace Vex.Engine
{
    public class AnimationPlayer
    {
        public Dictionary<string, Animation> Animations { get; private set; } = new Dictionary<string, Animation>();

        public string? DefaultAnimationName { get; private set; }
        public string? CurrentAnimationName { get; private set; }
        public string? NextAnimationName { get; private set; }

        public Animation? CurrentAnimation { get; private set; }

        public SubTexture? CurrentFrame => CurrentAnimation?.CurrentFrame;
        public bool IsPlaying => CurrentAnimation?.IsPlaying ?? false;

        public void Register(string name, Animation animation)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new VexException("Animation name cannot be null or empty.");

            if (animation == null)
                throw new VexArgumentNullException(nameof(animation));

            if (!Animations.ContainsKey(name))
            {
                Animations.Add(name, animation);

                if (DefaultAnimationName == null)
                {
                    DefaultAnimationName = name;
                }
            }
            else
            {
                Debug.Log(Debug.LogLevel.Warning, $"Animation with name '{name}' already exists in the animation player.");
            }
        }

        public void SetDefault(string name)
        {
            if (Animations.ContainsKey(name))
            {
                DefaultAnimationName = name;
            }
            else
            {
                Debug.Log(Debug.LogLevel.Warning, $"Cannot set default. Animation '{name}' does not exist.");
            }
        }

        public void Queue(string name)
        {
            if (Animations.ContainsKey(name))
            {
                NextAnimationName = name;
            }
            else
            {
                Debug.Log(Debug.LogLevel.Warning, $"Cannot queue animation '{name}' because it does not exist.");
            }
        }

        public void Play(string name, bool force = false)
        {
            if (CurrentAnimationName == name && !force)
                return;

            if (!Animations.TryGetValue(name, out var nextAnim))
            {
                Debug.Log(Debug.LogLevel.Warning, $"Animation '{name}' not found in AnimationPlayer.");
                return;
            }

            CurrentAnimation?.Stop();

            CurrentAnimationName = name;
            CurrentAnimation = nextAnim;
            CurrentAnimation.Reset();
            CurrentAnimation.Play();
        }

        public void Update(double deltaTime)
        {
            if (CurrentAnimation == null) return;
            CurrentAnimation.Update(deltaTime);
            if (CurrentAnimation.IsFinished)
            {

                if (!string.IsNullOrEmpty(NextAnimationName))
                {
                    string queued = NextAnimationName;
                    NextAnimationName = null;
                    Play(queued);
                }

                else if (!string.IsNullOrEmpty(DefaultAnimationName) && CurrentAnimationName != DefaultAnimationName)
                {
                    Play(DefaultAnimationName);
                }
            }
        }

        public void Pause() => CurrentAnimation?.Pause();
        public void Resume() => CurrentAnimation?.Play();
        public void Stop() => CurrentAnimation?.Stop();
        public void Reset() => CurrentAnimation?.Reset();
    }
}