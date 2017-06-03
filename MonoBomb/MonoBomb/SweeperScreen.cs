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
        MouseInputHandler mouseHandler;
        private Texture2D bombTexture, unclickedTileTexture, clickedTileTexture, cursor;
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

            gameboard.Draw(spriteBatch, bombTexture, unclickedTileTexture, clickedTileTexture, boardFont);
            spriteBatch.Draw(cursor, mouseHandler.CurrentPosition, Color.White);

            spriteBatch.End();
        }

        public void Init()
        {
            var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            mouseHandler = new MouseInputHandler();
            mouseHandler.Init();
            gameboard = new GameBoard(15,10,32,(gameBounds.Width / 2) - 240 , (gameBounds.Height / 2) - 160, 8);
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            bombTexture = game.Content.Load<Texture2D>(@"Skull_Bomb");
            unclickedTileTexture = game.Content.Load<Texture2D>(@"Unclicked_Tile");
            clickedTileTexture = game.Content.Load<Texture2D>(@"Clicked_Tile");
            boardFont = game.Content.Load<SpriteFont>(@"board-font");
            cursor = game.Content.Load<Texture2D>(@"cursor");
        }

        public IGameScreen Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                return new ExitGameScreen();

            mouseHandler.Update(gameTime);
            if (mouseHandler.LeftButtonHandler.IsInitialRelease)
            {
                gameboard.Click(mouseHandler.CurrentPosition);
            }

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

        public void Draw(SpriteBatch spriteBatch, Texture2D bombTexture, Texture2D UnclickedTileTexture, Texture2D ClickedTileTexture, SpriteFont font)
        {
            int size = height * width;
            for(int i = 0; i < size; i++)
            {
                var coord = coordinates(i);
                var location = new Vector2(left + (coord.X * tileSize), top + (coord.Y * tileSize));
                spriteBatch.Draw(ClickedTileTexture, location, Color.White * 1.0f);

                if (isUncovered[i])
                {
                    if (isBomb[i])
                    {
                        spriteBatch.Draw(bombTexture, location, Color.White * 1.0f);
                    }
                    else if (adjacency[i] > 0)
                    {
                        spriteBatch.DrawString(font, adjacency[i].ToString(), location + new Vector2(xPadding, yPadding), Color.White * 0.8f);
                    }
                }
                else
                {
                    spriteBatch.Draw(UnclickedTileTexture, location, Color.White * 1.0f);
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

            //After bombs are set, loop again for adjacency (can probably make this quicker by incrementing all surrounding tiles when dropping a bomb)
            for(int i = 0; i < size; i++)
            {
                int count = 0;
                var coord = coordinates(i);
                //NW
                if (coord.X - 1 > -1 && coord.Y - 1 > -1 && isBomb[index((int)coord.X - 1, (int)coord.Y - 1)]) count++;

                //N
                if (coord.X > -1 && coord.Y - 1 > -1 && isBomb[index((int)coord.X, (int)coord.Y - 1)]) count++;

                //NE
                if (coord.X + 1 < width && coord.Y - 1 > -1 && isBomb[index((int)coord.X + 1, (int)coord.Y - 1)]) count++;

                //E
                if (coord.X + 1 < width && coord.Y  > -1 && isBomb[index((int)coord.X + 1, (int)coord.Y)]) count++;

                //SE
                if (coord.X + 1 < width && coord.Y + 1 < height && isBomb[index((int)coord.X + 1, (int)coord.Y + 1)]) count++;

                //S
                if (coord.X > -1 && coord.Y + 1 < height && isBomb[index((int)coord.X, (int)coord.Y + 1)]) count++;

                //SW
                if (coord.X - 1 > -1 && coord.Y + 1 < height && isBomb[index((int)coord.X -1 , (int)coord.Y + 1)]) count++;

                //W
                if (coord.X - 1 > -1 && coord.Y > -1 && isBomb[index((int)coord.X - 1, (int)coord.Y)]) count++;

                adjacency[i] = count;
            }
        }

        public bool Click(Vector2 mousePos)
        {
            bool badClick = false;

            //use mouse position and gameboard dimemsion data to get the coordinates
            int coordX = (int)(mousePos.X - left) / tileSize;
            int coordY = (int)(mousePos.Y - top) / tileSize;

            var clickedIndex = index(coordX, coordY);

            if(clickedIndex > -1 && clickedIndex < (height * width))
            {
                uncoverTiles(clickedIndex);
                if (isBomb[clickedIndex])
                    badClick = true;
            }

            return badClick;
        }

        private void uncoverTiles(int clickedIndex)
        {
            List<int> alreadyExpanded = new List<int>();
            List<int> expandableIndexes = new List<int>();
            isUncovered[clickedIndex] = true;
            int gameSize = height * width;

            if(!isBomb[clickedIndex] && adjacency[clickedIndex] == 0)
            {
                expandableIndexes.Add(clickedIndex);
            }

            while (expandableIndexes.Any())
            {
                //alreadyExpanded.AddRange(expandableIndexes);
                var nextExpandableIndexes = new List<int>();

                foreach(int i in expandableIndexes)
                {
                    
                    //Uncover its adjacent tiles
                    int NW = index(coordinates(i) + new Vector2(-1, -1));
                    if(NW > -1 && NW < gameSize)
                    {
                        isUncovered[NW] = true;
                        if(!isBomb[NW] && adjacency[NW] == 0 && !alreadyExpanded.Contains(NW))
                        {
                            alreadyExpanded.Add(NW);
                            nextExpandableIndexes.Add(NW);
                        }
                    }
                    
                    int N = index(coordinates(i) + new Vector2(0, -1));
                    if (N > -1 && N < gameSize)
                    {
                        isUncovered[N] = true;
                        if (!isBomb[N] && adjacency[N] == 0 && !alreadyExpanded.Contains(N))
                        {
                            alreadyExpanded.Add(N);
                            nextExpandableIndexes.Add(N);
                        }
                    }

                    int NE = index(coordinates(i) + new Vector2(1, -1));
                    if (NE > -1 && NE < gameSize)
                    {
                        isUncovered[NE] = true;
                        if (!isBomb[NE] && adjacency[NE] == 0 && !alreadyExpanded.Contains(NE))
                        {
                            alreadyExpanded.Add(NE);
                            nextExpandableIndexes.Add(NE);
                        }
                    }

                    int E = index(coordinates(i) + new Vector2(1, 0));
                    if (E > -1 && E < gameSize)
                    {
                        isUncovered[E] = true;
                        if (!isBomb[E] && adjacency[E] == 0 && !alreadyExpanded.Contains(E))
                        {
                            alreadyExpanded.Add(E);
                            nextExpandableIndexes.Add(E);
                        }
                    }

                    int SE = index(coordinates(i) + new Vector2(1, 1));
                    if (SE > -1 && SE < gameSize)
                    {
                        isUncovered[SE] = true;
                        if (!isBomb[SE] && adjacency[SE] == 0 && !alreadyExpanded.Contains(SE))
                        {
                            alreadyExpanded.Add(SE);
                            nextExpandableIndexes.Add(SE);
                        }
                    }

                    int S = index(coordinates(i) + new Vector2(0, 1));
                    if (S > -1 && S < gameSize)
                    {
                        isUncovered[S] = true;
                        if (!isBomb[S] && adjacency[S] == 0 && !alreadyExpanded.Contains(S))
                        {
                            alreadyExpanded.Add(S);
                            nextExpandableIndexes.Add(S);
                        }
                    }

                    int SW = index(coordinates(i) + new Vector2(-1, 1));
                    if (SW > -1 && SW < gameSize)
                    {
                        isUncovered[SW] = true;
                        if (!isBomb[SW] && adjacency[SW] == 0 && !alreadyExpanded.Contains(SW))
                        {
                            alreadyExpanded.Add(SW);
                            nextExpandableIndexes.Add(SW);
                        }
                    }

                    int W = index(coordinates(i) + new Vector2(-1, 0));
                    if (W > -1 && W < gameSize)
                    {
                        isUncovered[W] = true;
                        if (!isBomb[W] && adjacency[W] == 0 && !alreadyExpanded.Contains(W))
                        {
                            alreadyExpanded.Add(W);
                            nextExpandableIndexes.Add(W);
                        }
                    }
                }
                
                expandableIndexes = nextExpandableIndexes;
            }
        }

        private Vector2 coordinates(int index)
        {
            int x = 0, y = 0;

            y = index / this.width;
            x = index % width;

            return new Vector2(x, y);
        }

        private int index(int x, int y)
        {
            return y * width + x;
        }

        private int index(Vector2 vector)
        {
            if (vector.X < 0 || vector.X > width - 1)
                return -1; // Can't safely wrap left or right

            return (int)Math.Floor(vector.Y * width + vector.X);
        }
    }
}
