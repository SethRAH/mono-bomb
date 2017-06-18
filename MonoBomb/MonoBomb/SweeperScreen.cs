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
        private GamePanel gamepanel;
        SpriteBatch spriteBatch;
        MouseInputHandler mouseHandler;
        private Texture2D bombTexture, unclickedTileTexture, unclickedTileTextureLong, clickedTileTexture, clickedTileTextureLong, cursor, flag, question;
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

            gamepanel.Draw(spriteBatch, unclickedTileTexture, unclickedTileTextureLong, clickedTileTexture, boardFont);
            gameboard.Draw(spriteBatch, bombTexture, unclickedTileTexture, clickedTileTexture, flag, question, boardFont);
            spriteBatch.Draw(cursor, mouseHandler.CurrentPosition, Color.White);

            spriteBatch.End();
        }

        public void Init()
        {
            int gamepanelWidth = 148;
            int gameboardWidth = 480;
            int spacerWidth = 20;

            var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            mouseHandler = new MouseInputHandler();
            mouseHandler.Init();
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            bombTexture = game.Content.Load<Texture2D>(@"Skull_Bomb");
            unclickedTileTexture = game.Content.Load<Texture2D>(@"Unclicked_Tile");
            unclickedTileTextureLong = game.Content.Load<Texture2D>(@"Unclicked_Tile_Long");
            clickedTileTexture = game.Content.Load<Texture2D>(@"Clicked_Tile");
            clickedTileTextureLong = game.Content.Load<Texture2D>(@"Clicked_Tile_Long");
            boardFont = game.Content.Load<SpriteFont>(@"board-font");
            cursor = game.Content.Load<Texture2D>(@"cursor");
            flag = game.Content.Load<Texture2D>(@"Flag");
            question = game.Content.Load<Texture2D>(@"Question");
            gameboard = new GameBoard(15,10,32, ((gameBounds.Width - gamepanelWidth)/2) - (gameboardWidth/2) + gamepanelWidth + (spacerWidth*2), (gameBounds.Height / 2) - 160, 8);
            gamepanel = new GamePanel(spacerWidth,25,gamepanelWidth,148);
            gamepanel.Init(unclickedTileTextureLong, clickedTileTextureLong, unclickedTileTexture, clickedTileTexture, boardFont, this);
        }

        public IGameScreen Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                return new ExitGameScreen();

            mouseHandler.Update(gameTime);
            if (mouseHandler.LeftButtonHandler.IsInitialRelease)
            {
                bool gameOver = gameboard.Click(mouseHandler.CurrentPosition);
                gamepanel.Click(mouseHandler.CurrentPosition);
            }
            if (mouseHandler.RightButtonHandler.IsInitialRelease)
            {
                gameboard.Flag(mouseHandler.CurrentPosition);
            }

            gamepanel.NumFlags = gameboard.NumBombs() - gameboard.NumFlags();

            gameboard.IsWin();

            gamepanel.Update(mouseHandler.CurrentPosition, mouseHandler.LeftButtonHandler.IsInitialClick, mouseHandler.LeftButtonHandler.IsInitialRelease, mouseHandler.LeftButtonHandler.IsHeld);

            return null;
        }

        internal void AcceptPanelCommand(PanelCommand panelCommand)
        {
            switch (panelCommand.Action)
            {
                case "Reset":
                    gameboard = new GameBoard(gameboard.width, gameboard.height, gameboard.tileSize, gameboard.left, gameboard.top, gameboard.bombRatio);
                    break;
                case "Change":
                    int newWidth = panelCommand.Width != null ? panelCommand.Width.Value : gameboard.width;
                    int newHeight = panelCommand.Height != null ? panelCommand.Height.Value : gameboard.height;
                    int newBombRation = panelCommand.BombRatio != null ? panelCommand.BombRatio.Value : gameboard.bombRatio;
                    var gameBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
                    int gamepanelWidth = 148;
                    int newLeft = ((gameBounds.Width - gamepanelWidth) / 2) - (newWidth * 16) + gamepanelWidth + 40;
                    int newTop = (gameBounds.Height / 2) - (16 * newHeight);


                    gameboard = new GameBoard(newWidth, newHeight, gameboard.tileSize, newLeft, newTop, newBombRation);
                    break;
            }
        }
    }
    
    public class PanelCommand
    {
        public String Action { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? BombRatio { get; set; }
    }

    public class GamePanel
    {
        //state variables

        private int height, width, bombRatio, pxTop, pxLeft, pxHeight, pxWidth;

        private List<Vector2> sizeOptions = new List<Vector2>() { new Vector2(10, 10), new Vector2(15,10), new Vector2(10,15), new Vector2(15,15) };
        private List<int> ratioOptions = new List<int> { 5, 8, 10, 15 };
        private List<IButton> buttons = new List<IButton>();

        private Texture2D _clickedButtonTextureShort, _clickedButtonTextureLong, _unclickedButtonTextureShort, _unclickedButtonTextureLong;

        public int NumFlags { get; set; }

        public GamePanel()
        {
            pxTop = 25;
            pxLeft = 25;
            pxHeight = 148;
            pxWidth = 148;
            bombRatio = 8;
            height = 10;
            width = 15;
        }

        public GamePanel(int left, int top, int width, int height)
        {
            pxTop = top;
            pxLeft = left;
            pxHeight = height;
            pxWidth = width;
            bombRatio = 8;
            this.height = 10;
            this.width = 15;
        }

        public void Init(Texture2D unclickedButtonTextureLong, Texture2D clickedButtonTextureLong, Texture2D unclickedButtonTextureShort, Texture2D clickedButtonTextureShort, SpriteFont font, SweeperScreen screen)
        {
            _unclickedButtonTextureLong = unclickedButtonTextureLong;
            _clickedButtonTextureLong = clickedButtonTextureLong;
            _unclickedButtonTextureShort = unclickedButtonTextureShort;
            _clickedButtonTextureShort = clickedButtonTextureShort;

            buttons = new List<IButton>() {
                new PanelButton(_unclickedButtonTextureLong, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 47), "Reset", new ResetCommand(ref screen)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 111), "10x10", new ChangeBoardCommand(ref screen, 10, 10, null)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 143), "15x10", new ChangeBoardCommand(ref screen, 15, 10, null)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 175), "15x15", new ChangeBoardCommand(ref screen, 15, 15, null)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 239), "1:5", new ChangeBoardCommand(ref screen, null, null, 5)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 271), "1:8", new ChangeBoardCommand(ref screen, null, null, 8)),
                new PanelButton(_unclickedButtonTextureShort, font, new Vector2((pxWidth / 2) - (_unclickedButtonTextureLong.Width / 2) + pxLeft, pxTop + 303), "1:10", new ChangeBoardCommand(ref screen, null, null, 10))
            };
        }

        public PanelCommand Update(Vector2 mousePos, bool isInitialClick, bool isInitialRelease, bool isDown)
        {
            PanelCommand result = new PanelCommand() { Height = height, Width = width, BombRatio = bombRatio };

            foreach(var button in buttons)
            {
                if(isDown && button.Intersect(mousePos))
                {
                    button.SetTexture(_clickedButtonTextureLong);
                }
                else
                {
                    button.SetTexture(_unclickedButtonTextureLong);
                }
            }

            return result;
        }

        public void Click(Vector2 mousePos)
        {
            foreach(var button in buttons)
            {
                if (button.Intersect(mousePos))
                {
                    button.Click();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D UnclickedTileTexture, Texture2D UnclickedTileTextureLong, Texture2D ClickedTileTexture, SpriteFont font)
        {
            var vertOffset = pxTop + 10;
            //First row is the panel label
            spriteBatch.DrawString(font, "Panel", new Vector2(pxLeft + 10, vertOffset), Color.White * 0.8f);

            spriteBatch.DrawString(font, "Dim.", new Vector2(pxLeft + 10, pxTop + 89), Color.White * 0.8f);

            spriteBatch.DrawString(font, "Bomb Ratio", new Vector2(pxLeft + 10, pxTop + 217), Color.White * 0.8f);

            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }

            var flagCountString = NumFlags.ToString().PadLeft(2, '0');
            var flagCountMeasure = font.MeasureString(flagCountString);
            spriteBatch.DrawString(font, flagCountString, new Vector2((pxWidth / 2) - (flagCountMeasure.X/2) + pxLeft, pxTop + 345), Color.Red * 0.8f);
        }
    }

    public class GameBoard
    {
        private readonly int xPadding = 8, yPadding = 8;
        
        public int height { get; private set; }
        public int width { get; private set; }
        public int bombRatio { get; private set; }
        public int tileSize { get; private set; }
        public int top { get; private set; }
        public int left { get; private set; }

        private bool gameOver, win;

        bool[] isUncovered;
        bool[] isBomb;
        int[] adjacency;

        int[] isFlagged;//0 no , 1 flag, 2 question

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

        public int NumFlags()
        {
            return isFlagged.Where(x => x == 1).Count();
        }

        public int NumBombs()
        {
            return isBomb.Where(x => x).Count();
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D bombTexture, Texture2D UnclickedTileTexture, Texture2D ClickedTileTexture, Texture2D Flag, Texture2D question, SpriteFont font)
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
                    if (isFlagged[i] == 1)
                    {
                        spriteBatch.Draw(Flag, location, Color.White * 1.0f);
                    }
                    else if(isFlagged[i] == 2)
                    {
                        spriteBatch.Draw(question, location, Color.White * 1.0f);
                    }
                }
            }

            if(gameOver && win)
            {
                string winningText = "#Winning";
                Vector2 winningTextMeasure = font.MeasureString(winningText);
                var location = new Vector2((left + (pixWidth / 2))- (winningTextMeasure.X/2), (top + (pixHeight / 2)) - (winningTextMeasure.Y/2));

                //Draw Outline First
                spriteBatch.DrawString(font, winningText, location + new Vector2(-3, -3), Color.Black * 1.0f);
                spriteBatch.DrawString(font, winningText, location + new Vector2(-3, 3), Color.Black * 1.0f);
                spriteBatch.DrawString(font, winningText, location + new Vector2(3, -3), Color.Black * 1.0f);
                spriteBatch.DrawString(font, winningText, location + new Vector2(3, 3), Color.Black * 1.0f);
                //Draw Text
                spriteBatch.DrawString(font, winningText, location, Color.White * 1.0f);
            }
        }

        private void initBoard()
        {
            var random = new Random();
            int size = height * width;
            isUncovered = new bool[size];
            isFlagged = new int[size];
            isBomb = new bool[size];
            adjacency = new int[size];
            for(int i = 0; i < size; i++)
            {
                isUncovered[i] = false;
                isFlagged[i] = 0;
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

            if (!gameOver)
            {
                //use mouse position and gameboard dimemsion data to get the coordinates
                int coordX = (int)(mousePos.X - left) / tileSize;
                int coordY = (int)(mousePos.Y - top) / tileSize;

                var clickedIndex = index(coordX, coordY);

                if (clickedIndex > -1 && clickedIndex < (height * width))
                {
                    uncoverTiles(clickedIndex);
                    if (isBomb[clickedIndex])
                        badClick = true;
                }

                if (badClick)
                {
                    gameOver = true;
                    revealBombs();
                }
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

        private void revealBombs()
        {
            int size = height * width;
            for(int i = 0; i< size; i++)
            {
                if (isBomb[i])
                    isUncovered[i] = true;
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
            if (x < 0 || x > width - 1)
                return -1; // Can't safely wrap left or right
            return y * width + x;
        }

        private int index(Vector2 vector)
        {
            if (vector.X < 0 || vector.X > width - 1)
                return -1; // Can't safely wrap left or right

            return (int)Math.Floor(vector.Y * width + vector.X);
        }

        public void Flag(Vector2 mousePos)
        {
            if (!gameOver)
            {
                //use mouse position and gameboard dimemsion data to get the coordinates
                int coordX = (int)(mousePos.X - left) / tileSize;
                int coordY = (int)(mousePos.Y - top) / tileSize;

                var clickedIndex = index(coordX, coordY);

                if (clickedIndex > -1 && clickedIndex < (height * width))
                {
                    isFlagged[clickedIndex]++;
                    isFlagged[clickedIndex] = isFlagged[clickedIndex] % 3;
                }
            }
        }

        public bool IsWin()
        {
            //Is Win if all spaces that are non-bomb are unclicked (and no bomb spaces are clicked)
            bool winning = true;
            int size = width * height;
            for(int i = 0; i < size && winning; i++)
            {
                winning = isUncovered[i] ^ isBomb[i];
            }

            if (winning)
            {
                gameOver = true;
                win = true;
            }
            

            return winning;
        }
    }
    public interface IButton
    {
        void SetText(string text);
        void SetTexture(Texture2D texture);
        void SetFont(SpriteFont font);
        void SetCommand(IButtonCommand command);
        bool Intersect(Vector2 coords);
        void Draw(SpriteBatch spriteBatch);
        void Click();
    }

    public interface IButtonCommand
    {
        void Execute();
    }

    public class NullCommand : IButtonCommand
    {
        public void Execute() { }
    }

    public class ResetCommand : IButtonCommand
    {
        private SweeperScreen _sweeperScreen;

        public ResetCommand(ref SweeperScreen sweeperScreen)
        {
            _sweeperScreen = sweeperScreen;
        }

        public void Execute()
        {
            _sweeperScreen.AcceptPanelCommand(new PanelCommand() { Action = "Reset" });
        }
    }

    public class ChangeBoardCommand : IButtonCommand
    {
        private SweeperScreen _sweeperScreen;
        private int? _height, _width, _bombRatio;

        public ChangeBoardCommand(ref SweeperScreen sweeperScreen, int? newWidth, int? newHeight, int? newBombRatio)
        {
            _sweeperScreen = sweeperScreen;
            _height = newHeight;
            _width = newWidth;
            _bombRatio = newBombRatio;
        }

        public void Execute()
        {
            PanelCommand command = new PanelCommand() { Action = "Change" };
            if (_height != null) command.Height = _height.Value;
            if (_width != null) command.Width = _width.Value;
            if (_bombRatio != null) command.BombRatio = _bombRatio.Value;

            _sweeperScreen.AcceptPanelCommand(command);
        }
    }

    public class PanelButton : IButton
    {
        protected SpriteFont _font;
        protected string _text;
        protected Texture2D _texture;
        protected Vector2 _location;
        protected IButtonCommand _command;


        public PanelButton(Texture2D texture, SpriteFont font, Vector2 location, string text, IButtonCommand command)
        {
            _font = font;
            _text = text;
            _texture = texture;
            _location = location;
            _command = command;
        }

        public void Click()
        {
            _command.Execute();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _location, Color.White * 1.0f);
            var fontDim = _font.MeasureString(_text);
            var textOffset = new Vector2((_texture.Width / 2) - (fontDim.X / 2), (_texture.Height / 2) - (fontDim.Y / 2));
            spriteBatch.DrawString(_font, _text, _location + textOffset, Color.White * 0.8f);
        }

        public bool Intersect(Vector2 coords)
        {
            if (coords.X >= _location.X && coords.X <= _location.X + _texture.Width
                && coords.Y >= _location.Y && coords.Y <= _location.Y + _texture.Height)
                return true;
            return false;
        }

        public void SetCommand(IButtonCommand command)
        {
            _command = command;
        }

        public void SetFont(SpriteFont font)
        {
            _font = font;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
    }
}
