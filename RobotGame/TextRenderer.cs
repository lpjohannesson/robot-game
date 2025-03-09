using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RobotGame
{
    public class TextRenderer
    {
        public int CharHeight = 10;

        public Texture2D Texture;
        public Point CharSize;
        public int CharsPerRow;
        public int[] CharSpacings;

        public TextRenderer(Texture2D texture, Point charSize)
        {
            Texture = texture;
            CharSize = charSize;

            CharsPerRow = Texture.Width / CharSize.X;

            // Load font spacing from json
            string spacingsJson = Encoding.UTF8.GetString(Properties.Resources.FontSpacings);
            CharSpacings = JsonSerializer.Deserialize<int[]>(spacingsJson);
        }

        // Returns the spacing for a character
        public int GetCharSpacing(char c)
        {
            int charIndex = c - 32;

            // Check range
            if (charIndex < 0 || charIndex >= CharSpacings.Length)
            {
                return 0;
            }

            return CharSpacings[charIndex];
        }

        // Gets the size of a string in the font
        public Vector2 GetTextSize(string text)
        {
            // Find sum of every letter spacing
            int lineWidth = 0, width = 0, height = CharHeight;

            foreach (char c in text)
            {
                // Check for newline
                if (c == '\n')
                {
                    lineWidth = 0;
                    height += CharHeight;
                    continue;
                }

                lineWidth += GetCharSpacing(c);
                width = Math.Max(width, lineWidth);
            }

            return new Vector2(width, height);
        }

        // Centers a text position
        public Vector2 GetCenteredTextPosition(string text, Vector2 position)
        {
            return Vector2.Floor(position - GetTextSize(text) * 0.5f);
        }

        public void DrawText(Renderer renderer, string text, Vector2 position, Color color)
        {
            Vector2 charPosition = position;

            foreach (char c in text)
            {
                // Check for newline
                if (c == '\n')
                {
                    charPosition.X = position.X;
                    charPosition.Y += CharHeight;
                    continue;
                }

                // Get relative ASCII index
                int charIndex = c - 32;

                // Create source rectangle
                Rectangle srcRect = new(new Point(charIndex % CharsPerRow, charIndex / CharsPerRow) * CharSize, CharSize);

                renderer.SpriteBatch.Draw(Texture, charPosition, srcRect, color);

                charPosition.X += GetCharSpacing(c);
            }
        }
    }
}
