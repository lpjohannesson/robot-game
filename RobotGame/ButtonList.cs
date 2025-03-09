using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RobotGame
{
    public class ButtonList
    {
        public const float ButtonSeparation = 16.0f;

        public Vector2 ButtonsStart = new(120.0f, 90.0f);

        public List<TextButton> Buttons = new();

        public RobotGame Game;

        public ButtonList(RobotGame game)
        {
            Game = game;
        }

        // Gets the position for a button by index
        public Vector2 GetButtonPosition(int index)
        {
            return new Vector2(ButtonsStart.X, ButtonsStart.Y + index * ButtonSeparation);
        }

        // Creates a button
        public TextButton CreateButton(string text)
        {
            TextButton button = new(Game, text, GetButtonPosition(Buttons.Count));
            Buttons.Add(button);

            return button;
        }

        public void Update()
        {
            // Scale mouse by window scale
            Vector2 mousePosition = Game.Input.GetMousePosition() / Game.Renderer.WindowScale;
            bool mouseClicked = Game.Input.IsMouseJustClicked();

            foreach (TextButton button in Buttons)
            {
                button.UpdateMouse(mousePosition, mouseClicked);
            }
        }

        public void Draw()
        {
            foreach (TextButton button in Buttons)
            {
                button.Draw(Game.Renderer);
            }
        }
    }
}
