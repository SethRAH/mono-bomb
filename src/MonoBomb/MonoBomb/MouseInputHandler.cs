using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoBomb
{
    public class MouseInputHandler
    {
        public ButtonHandler LeftButtonHandler { get; private set; }
        public ButtonHandler RightButtonHandler { get; private set; }
        public ButtonHandler MiddleButtonHandler { get; private set; }
        public Vector2 CurrentPosition
        {
            get
            {
                var mouseState = Mouse.GetState();
                return new Vector2(mouseState.X, mouseState.Y);
            }
        }
        TimeSpan lastUpdate;

        public void Init()
        {
            LeftButtonHandler = new ButtonHandler();
            RightButtonHandler = new ButtonHandler();
            MiddleButtonHandler = new ButtonHandler();
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime != lastUpdate)
            {
                var mouseState = Mouse.GetState();
                LeftButtonHandler.Push(mouseState.LeftButton, mouseState.X, mouseState.Y);
                RightButtonHandler.Push(mouseState.RightButton, mouseState.X, mouseState.Y);
                MiddleButtonHandler.Push(mouseState.MiddleButton, mouseState.X, mouseState.Y);

                lastUpdate = gameTime.TotalGameTime;
            }

        }
    }

    public class ButtonHandler
    {
        private ButtonState? pastState;

        public bool IsInitialClick { get; private set; }
        public bool IsHeld { get; private set; }
        public bool IsInitialRelease { get; private set; }
        public Vector2 LastClickPosition { get; private set; }
        public Vector2 LastReleasePosition { get; private set; }
        

        public void Push(ButtonState newState, int xPos, int yPos)
        {
            if (pastState != null)
            {
                if (newState == ButtonState.Pressed && pastState.Value == ButtonState.Released)
                {
                    IsInitialClick = true;
                    IsHeld = false;
                    IsInitialRelease = false;
                    LastClickPosition = new Vector2(xPos, yPos);
                }
                else if (newState == ButtonState.Pressed && pastState.Value == ButtonState.Pressed)
                {
                    IsInitialClick = false;
                    IsHeld = true;
                    IsInitialRelease = false;
                }
                else if (newState == ButtonState.Released && pastState.Value == ButtonState.Pressed)
                {
                    IsInitialClick = false;
                    IsHeld = false;
                    IsInitialRelease = true;
                    LastReleasePosition = new Vector2(xPos, yPos);
                }
                else
                {
                    IsInitialClick = false;
                    IsHeld = false;
                    IsInitialRelease = false;
                }
            }

            pastState = newState;
        }

    }
}
