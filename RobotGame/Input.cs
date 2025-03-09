using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RobotGame
{
    public class Input
    {
        public bool LastClicked = false;

        public bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        // Returns a vector based on four inputs
        public Vector2 GetVector(Keys up, Keys down, Keys left, Keys right)
        {
            // Create axes by weighting inputs
            return new Vector2(
                (IsKeyPressed(right) ? 1.0f : 0.0f) -
                (IsKeyPressed(left) ? 1.0f : 0.0f),
                (IsKeyPressed(down) ? 1.0f : 0.0f) -
                (IsKeyPressed(up) ? 1.0f : 0.0f));
        }

        // Returns the mouse position
        public Vector2 GetMousePosition()
        {
            MouseState mouseState = Mouse.GetState();

            return new Vector2(mouseState.X, mouseState.Y);
        }

        // Returns whether the mouse was just clicked
        public bool IsMouseJustClicked()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed && !LastClicked;
        }

        // Updates the input states
        public void Update()
        {
            LastClicked = Mouse.GetState().LeftButton == ButtonState.Pressed;
        }
    }
}
