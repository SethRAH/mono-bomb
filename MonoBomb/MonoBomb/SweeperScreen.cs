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
        private SpriteFont boardFont;

        public Color BGColor { get { return this.bgColor; } }

        public SweeperScreen(Game game)
        {
            this.game = game;
            this.bgColor = new Color(138, 148, 136);
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            gameboard.Draw(spriteBatch, bombTexture, unclickedTileTexture, boardFont);

            spriteBatch.End();
        }

        public void Init()
        {
            var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            gameboard = new GameBoard(15,10,32,(gameBounds.Width / 2) - 240 , (gameBounds.Height / 2) - 160, 8);
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            bombTexture = game.Content.Load<Texture2D>(@"Skull_Bomb");
            unclickedTileTexture = game.Content.Load<Texture2D>(@"Unclicked_Tile");
            boardFont = game.Content.Load<SpriteFont>(@"board-font");
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
        private readonly int xPadding = 8, yPadding = 8;
        int height, width, bombRatio, tileSize, top, left;
        bool[] isUncovered;
        bool[] isBomb;
        int[] adjacency;

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

        public void Draw(SpriteBatch spriteBatch, Texture2D bombTexture, Texture2D UnclickedTileTexture, SpriteFont font)
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
                else if(adjacency[i] > 0)
                {
                    spriteBatch.DrawString(font, adjacency[i].ToString(), location + new Vector2(xPadding, yPadding), Color.White * 0.8f);
                }
            }
        }

        private void initBoard()
        {
            var random = new Random();
            int size = height * width;
            isUncovered = new bool[size];
            isBomb = new bool[size];
            adjacency = new int[size];
            for(int i = 0; i < size; i++)
            {
                isUncovered[i] = false;
                isBomb[i] = random.Next(bombRatio - 1) == 0;
            }

            //After bombs are set, loop again for adjacency
            for(int i = 0; i < size; i++)
            {
                int count = 0;
                var coord = coordinates(i);
                //NW
                if (coord.Item1 - 1 > -1 && coord.Item2 - 1 > -1 && isBomb[index(coord.Item1 - 1, coord.Item2 - 1)]) count++;

                //N
                if (coord.Item1 > -1 && coord.Item2 - 1 > -1 && isBomb[index(coord.Item1, coord.Item2 - 1)]) count++;

                //NE
                if (coord.Item1 + 1 < width - 1 && coord.Item2 - 1 > -1 && isBomb[index(coord.Item1 + 1, coord.Item2 - 1)]) count++;

                //E
                if (coord.Item1 + 1 < width - 1 && coord.Item2  > -1 && isBomb[index(coord.Item1 + 1, coord.Item2)]) count++;

                //SE
                if (coord.Item1 + 1 < width && coord.Item2 + 1 < height && isBomb[index(coord.Item1 + 1, coord.Item2 + 1)]) count++;

                //S
                if (coord.Item1 > -1 && coord.Item2 + 1 < height && isBomb[index(coord.Item1, coord.Item2 + 1)]) count++;

                //SW
                if (coord.Item1 - 1 > -1 && coord.Item2 + 1 < height && isBomb[index(coord.Item1 -1 , coord.Item2 + 1)]) count++;

                //W
                if (coord.Item1 - 1 > -1 && coord.Item2 > -1 && isBomb[index(coord.Item1 - 1, coord.Item2)]) count++;

                adjacency[i] = count;
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
