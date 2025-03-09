using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace RobotGame.Scenes
{
    public class HelpScene : IGameScene
    {
        public MenuText MenuText;

        public TextButton OkayButton;
        public ButtonList ButtonList;

        public RobotGame Game;

        public HelpScene(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            MenuText = new MenuText(Game,
                "Collect as many gears as you can\n" +
                "while avoiding enemy fire.\n" +
                "Batteries will refill your health.\n" +
                "WASD: Move, Arrow keys: Fire",
                Color.Yellow);

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
