using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGame
{
    public class SpriteAnimation
    {
        public List<Rectangle> Frames;
        public float FramesPerSecond;
        public bool Loops;

        public SpriteAnimation(List<Rectangle> frames, float framesPerSecond, bool loops = true)
        {
            Frames = frames;
            FramesPerSecond = framesPerSecond;

            Loops = loops;
        }
    }
}
