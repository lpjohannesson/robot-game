using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGame
{
    public class RectRenderer
    {
        public Texture2D Texture;

        public RectRenderer(Renderer renderer, Color color)
        {
            // Create 1x1 texture to represent hitboxes
            Texture = new Texture2D(renderer.GraphicsDevice, 1, 1);
            Texture.SetData(new[] { new Color(color, 0.1f) });
        }

        // Draws a rect
        public void DrawRect(Renderer renderer, GameRect gameRect)
        {
            Rectangle rect = new(
                (int)gameRect.Position.X,
                (int)gameRect.Position.Y,
                (int)gameRect.Size.X,
                (int)gameRect.Size.Y);

            renderer.SpriteBatch.Draw(Texture, rect, Color.White);
        }
    }
}
