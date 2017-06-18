using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MonoBomb
{
    class SplashScreen : IGameScreen
    {
        private Game game;
        SpriteBatch spriteBatch;
        IGameScreen successor;
        private Sprite logo1;
        private Sprite logo2;
        private TimeSpan firstTime;
        private Color bgColor;

        public Color BGColor { get { return bgColor; } }

        public SplashScreen(Game game, IGameScreen successor)
        {
            this.game = game;
            this.successor = successor;
            this.bgColor = Color.White;
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            logo1.Draw(spriteBatch);
            logo2.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void Init()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            var logo1Texture = game.Content.Load<Texture2D>(@"dev-logo-1");
            var logo2Texture = game.Content.Load<Texture2D>(@"dev-logo-2");


            var logo1PosX = (gameBounds.Width / 2) - (logo1Texture.Width / 2);
            var logo1PosY = (gameBounds.Height / 2) - ((logo1Texture.Height + logo2Texture.Height) / 2);

            var logo2PosX = (gameBounds.Width / 2) - (logo2Texture.Width / 2);
            var logo2PosY = (gameBounds.Height / 2) - ((logo1Texture.Height + logo2Texture.Height) / 2) + logo1Texture.Height - 12;

            logo1 = new Sprite(logo1Texture, new Vector2(logo1PosX, logo1PosY), gameBounds);
            logo1.transparency = 0.0f;
            logo2 = new Sprite(logo2Texture, new Vector2(logo2PosX, logo2PosY), gameBounds);
            logo2.transparency = 0.0f;

            var devTheme = game.Content.Load<SoundEffect>(@"Audio\SoundFX\dev-theme");

            var devThemeInstance = devTheme.CreateInstance();
            devThemeInstance.Volume = 0.5f;
            devThemeInstance.IsLooped = false;
            devThemeInstance.Play();
        }

        public IGameScreen Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                return new ExitGameScreen();

            if(firstTime != null)
            {
                logo1.transparency = (float)(MathHelper.Clamp((float)(gameTime.TotalGameTime - firstTime).TotalMilliseconds, 0, 1000)/ 1000.0f);
                if ((gameTime.TotalGameTime - firstTime).TotalMilliseconds > 1000)
                    logo2.transparency = 1.0f;

                if ((gameTime.TotalGameTime - firstTime).TotalMilliseconds > 3000)
                {
                    successor.Init();
                    return successor;
                }
            }
            else
            {
                firstTime = gameTime.TotalGameTime;
            }

            return null;
        }
    }
}
