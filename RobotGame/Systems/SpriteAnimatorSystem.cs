using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Components;
using System.Collections.Generic;

namespace RobotGame.Systems
{
    public class SpriteAnimatorSystem : ISystem
    {
        public QueryDescription Query;

        public SpriteAnimatorSystem()
        {
            Query = new QueryDescription().WithAll<SpriteComponent, SpriteAnimatorComponent>();
        }

        // Gets a list of frames for a texture
        public static List<Rectangle> GetFrames(Texture2D texture, int frameCount)
        {
            List<Rectangle> frames = new();

            // Get the width and height for a single frame
            int frameWidth = texture.Width / frameCount;
            int frameHeight = texture.Height;

            // Create every frame as a rectangle
            for (int i = 0; i < frameCount; i++)
            {
                frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
            }

            return frames;
        }

        // Plays an animation on an animator
        public static void PlayAnimation(ref SpriteAnimatorComponent animator, SpriteAnimation animation)
        {
            // Stop if animation already playing
            if (animator.Animation == animation)
            {
                return;
            }

            // Play new animation with resetted time
            animator.Animation = animation;
            animator.Time = 0.0f;
            animator.Finished = false;
        }

        public void Initialize()
        {

        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref SpriteComponent sprite,
                ref SpriteAnimatorComponent animator) =>
            {
                SpriteAnimation animation = animator.Animation;

                // Stop if no animation 
                if (animation == null)
                {
                    return;
                }

                // Stop if finished
                if (!animation.Loops && animator.Finished)
                {
                    return;
                }

                // Get frame index by flooring time
                int frameIndex = (int)animator.Time;

                // Load frame rect
                sprite.Frame = animation.Frames[frameIndex];

                // Advance animation by FPS
                animator.Time += delta * animation.FramesPerSecond;

                // Loop or finish
                float duration = animation.Frames.Count;

                if (animator.Time >= duration)
                {
                    if (animation.Loops)
                    {
                        animator.Time %= duration;
                    }
                    else
                    {
                        animator.Finished = true;
                    }
                }
            });
        }
    }
}
