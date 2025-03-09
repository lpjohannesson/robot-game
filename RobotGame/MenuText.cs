using Microsoft.Xna.Framework;

namespace RobotGame
{
    public class MenuText
    {
        public string Text;
        public Vector2 Position = new(120.0f, 48.0f);
        public Color Color;

        public RobotGame Game;

        public MenuText(RobotGame game, string text, Color color)
        {
            Game = game;
            Text = text;
            Color = color;

            // Center text
            Position = Game.Renderer.TextRenderer.GetCenteredTextPosition(Text, Position);
        }

        public void Draw()
        {
            Renderer renderer = Game.Renderer;
            renderer.TextRenderer.DrawText(renderer, Text, Position, Color);
        }
    }
}
