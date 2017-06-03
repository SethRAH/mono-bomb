using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoBomb
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoBomb : Game
    {
        GraphicsDeviceManager graphics;
        IGameScreen screen;

        //SpriteBatch spriteBatch;
        //MouseInputHandler mouseHandler;

        //Texture2D cursor;
        //SoundEffect zing;         

        public MonoBomb()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //IsMouseVisible = true;

            screen = new SplashScreen(this, new SweeperScreen(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            screen.Init();

            //mouseHandler = new MouseInputHandler();
            //mouseHandler.Init();

            //// Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            //Mouse.SetCursor(MouseCursor.FromTexture2D(Content.Load<Texture2D>(@"cursor"), 0, 0));
            //cursor = Content.Load<Texture2D>(@"cursor");

            //zing = Content.Load<SoundEffect>(@"Audio\SoundFX\NFF-zing");


            //var bgMusic = Content.Load<SoundEffect>(@"Audio\Music\1812-rev");

            //var bgMusicInstance = bgMusic.CreateInstance();
            //bgMusicInstance.Volume = 0.5f;
            //bgMusicInstance.IsLooped = true;
            //bgMusicInstance.Play();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var tempScreen = screen.Update(gameTime);

            if (tempScreen != null)// change of game screen taking place
            {

                if (tempScreen as ExitGameScreen != null)
                {
                    Exit();
                }
                else
                {
                    screen = tempScreen;
                }

            }

            //mouseHandler.Update(gameTime);

            //if(mouseHandler.LeftButtonHandler.IsInitialClick)
            //{
            //    var playZing = zing.CreateInstance();
            //    playZing.IsLooped = false;
            //    playZing.Play();
            //}

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(screen.BGColor);

            screen.Draw(gameTime);

            //spriteBatch.Begin();
            //spriteBatch.Draw(cursor, new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
