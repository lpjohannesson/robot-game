using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGame.Components
{
    public struct SpriteComponent
    {
        public Texture2D Texture;
        public Rectangle Frame;
        public Vector2 Offset;
        public Color Color;
        public bool FlipX;

        public SpriteComponent()
        {
            Color = Color.White;
        }
    }
}
