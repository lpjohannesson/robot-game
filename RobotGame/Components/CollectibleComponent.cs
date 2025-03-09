namespace RobotGame.Components
{
    public struct CollectibleComponent
    {
        public bool Collected;
        public float DespawnTimer;

        public CollectibleComponent()
        {
            DespawnTimer = 5.0f;
        }
    }
}
