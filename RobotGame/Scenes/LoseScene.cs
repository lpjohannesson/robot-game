using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace RobotGame.Scenes
{
    public class LoseScene : IGameScene
    {
        public MenuText MenuText;

        public TextButton PlayAgainButton, MenuButton;
        public ButtonList ButtonList;

        public RobotGame Game;

        public LoseScene(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            MenuText = new MenuText(Game, "You lose!", Color.Red);

            // Create buttons
            ButtonList = new ButtonList(Game);

            PlayAgainButton = ButtonList.CreateButton("Play again");
            MenuButton = ButtonList.CreateButton("Return to menu");

            // Stop music
            MediaPlayer.Stop();
        }

        public void Update(float delta)
        {
            ButtonList.Update();

            if (PlayAgainButton.Clicked)
            {
                // Restart game
                Game.ChangeScene(new MainGameScene(Game));
            }

            if (MenuButton.Clicked)
            {
                // Return to menu
                Game.ChangeScene(new MenuScene(Game));
            }
        }

        public void Draw()
        {
            MenuText.Draw();
            ButtonList.Draw();
        }
    }
}
