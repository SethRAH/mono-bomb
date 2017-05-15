using System;
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
        Color BGColor { get; }

        IGameScreen Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Init();
    }

    public class ExitGameScreen : IGameScreen
    {
        Color IGameScreen.BGColor => Color.White;

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

        void IGameScreen.Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        void IGameScreen.Init()
        {
            throw new NotImplementedException();
        }

        IGameScreen IGameScreen.Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
