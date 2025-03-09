using Arch.Core;

namespace RobotGame.Systems
{
    public interface ISystem
    {
        // Starts the system
        void Initialize();

        // Updates the system
        void Update(World entities, float delta);
    }
}
