using Microsoft.Xna.Framework;

namespace MonoBomb
{
    public interface IController
    {
        ICommand GetCommand(GameObjectContainer gameObjects);
    }

    public interface ICommand
    {
        void Execute(IActor actor);
    }

    public interface IActor
    {
        Vector2 Velocity { get; set; }

    }

    public interface IGameScreen
    {
        IGameScreen Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Init();
    }

    public class ExitGameScreen : IGameScreen
    {
        public void Draw(GameTime gameTime)
        {

        }

        public void Init()
        {

        }

        public IGameScreen Update(GameTime gameTime)
        {
            return null;
        }
    }
}
