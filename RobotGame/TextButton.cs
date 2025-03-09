using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RobotGame
{
    public class TextButton
    {
        public Color HoverColor = Color.Cyan;

        public bool Hovered;
        public bool Clicked;

        public string Text;
        public GameRect Rect;

        public RobotGame Game;

        public TextButton(RobotGame game, string text, Vector2 position)
        {
            Game = game;
            Text = text;

            // Find centered rect from text
            Vector2 textSize = Game.Renderer.TextRenderer.GetTextSize(text);

            Rect = new GameRect(Vector2.Floor(position - textSize * 0.5f), textSize);
        }

        // Updates based on the mouse state
        public void UpdateMouse(Vector2 mousePosition, bool mouseClicked)
        {
            Hovered = GameRect.Contains(Rect, mousePosition);

            if (Hovered)
            {
                Clicked = mouseClicked;
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.TextRenderer.DrawText(renderer, Text, Rect.Position, Hovered ? HoverColor : Color.White);
        }
    }
}
