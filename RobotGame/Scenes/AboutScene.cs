using Microsoft.Xna.Framework;

namespace RobotGame.Scenes
{
    public class AboutScene : IGameScene
    {
        public MenuText MenuText;

        public TextButton OkayButton;
        public ButtonList ButtonList;

        public RobotGame Game;

        public AboutScene(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            MenuText = new MenuText(Game,
                "Game created by Leif Johannesson",
                Color.Cyan);

            // Create buttons
            ButtonList = new ButtonList(Game);

            OkayButton = ButtonList.CreateButton("Okay");
        }

        public void Update(float delta)
        {
            ButtonList.Update();

            if (OkayButton.Clicked)
            {
                // Return to meun
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
