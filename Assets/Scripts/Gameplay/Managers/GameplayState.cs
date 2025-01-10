namespace Dremu.Gameplay.Manager
{
    public interface GameplayState
    {
        public void Stop();
        public void Continue();
        public void Restart();
        public void Pause();
    }
}