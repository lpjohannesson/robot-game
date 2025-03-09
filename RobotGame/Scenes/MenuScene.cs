using Microsoft.Xna.Framework;

namespace RobotGame.Scenes
{
    public class MenuScene : IGameScene
    {
        public MenuText MenuText;

        public TextButton PlayButton, HelpButton, AboutButton;
        public ButtonList ButtonList;

        public RobotGame Game;

        public MenuScene(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            MenuText = new MenuText(Game, "Robot game", Color.LightGreen);

            // Create buttons
            ButtonList = new ButtonList(Game);

            PlayButton = ButtonList.CreateButton("Play game");
            HelpButton = ButtonList.CreateButton("Help");
            AboutButton = ButtonList.CreateButton("About");
        }

        public void Update(float delta)
        {
            ButtonList.Update();

            if (PlayButton.Clicked)
            {
                // Start game
                Game.ChangeScene(new MainGameScene(Game));
            }

            if (HelpButton.Clicked)
            {
                // Show help
                Game.ChangeScene(new HelpScene(Game));
            }

            if (AboutButton.Clicked)
            {
                // Show help
                Game.ChangeScene(new AboutScene(Game));
            }
        }

        public void Draw()
        {
            MenuText.Draw();
            ButtonList.Draw();
        }
    }
}
