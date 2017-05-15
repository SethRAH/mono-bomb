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
    public class SweeperScreen : IGameScreen
    {
        private Game game;
        private GameBoard gameboard;
        SpriteBatch spriteBatch;
        private Texture2D bombTexture, unclickedTileTexture;
        private Color bgColor;

        public Color BGColor { get { return this.bgColor; } }

        public SweeperScreen(Game game)
        {
            this.game = game;
            this.bgColor = new Color(138, 148, 136);
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            gameboard.Draw(spriteBatch, bombTexture, unclickedTileTexture);

            spriteBatch.End();
        }

        public void Init()
        {
            var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            gameboard = new GameBoard(15,10,32,(gameBounds.Width / 2) - 240 , (gameBounds.Height / 2) - 160, 10);
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            bombTexture = game.Content.Load<Texture2D>(@"Skull_Bomb");
            unclickedTileTexture = game.Content.Load<Texture2D>(@"Unclicked_Tile");
        }

        public IGameScreen Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                return new ExitGameScreen();

            return null;
        }
    }

    public class GameBoard
    {
        int height, width, bombRatio, tileSize, top, left;
        bool[] isUncovered;
        bool[] isBomb;

        public int pixHeight { get { return height * tileSize; } }
        public int pixWidth { get { return width * tileSize; } }

        public GameBoard()
        {
            height = 10;
            width = 10;
            tileSize = 32;
            top = 100;
            left = 100;
            bombRatio = 10;
            initBoard();
        }

        public GameBoard(int width, int height, int tileSize, int left, int top, int bombRatio)
        {
            this.height = height;
            this.width = width;
            this.tileSize = tileSize;
            this.top = top;
            this.left = left;
            this.bombRatio = bombRatio;
            initBoard();
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D bombTexture, Texture2D UnclickedTileTexture)
        {
            int size = height * width;
            for(int i = 0; i < size; i++)
            {
                var coord = coordinates(i);
                var location = new Vector2(left + (coord.Item1 * tileSize), top + (coord.Item2 * tileSize));
                spriteBatch.Draw(UnclickedTileTexture, location, Color.White * 1.0f);

                if (isBomb[i])
                {
                    spriteBatch.Draw(bombTexture, location, Color.White * 1.0f);
                }
            }
        }

        private void initBoard()
        {
            var random = new Random();
            int size = height * width;
            isUncovered = new bool[size];
            isBomb = new bool[size];
            for(int i = 0; i < size; i++)
            {
                isUncovered[i] = false;
                isBomb[i] = random.Next(bombRatio - 1) == 0;
            }
        }

        private Tuple<int, int> coordinates(int index)
        {
            int x = 0, y = 0;

            y = index / this.width;
            x = index % width;

            return new Tuple<int, int>(x, y);
        }

        private int index(int x, int y)
        {
            return y * width + x;
        }
    }
}
