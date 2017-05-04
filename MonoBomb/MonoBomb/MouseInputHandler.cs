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
                LeftButtonHandler.Push(mouseState.LeftButton);
                RightButtonHandler.Push(mouseState.RightButton);
                MiddleButtonHandler.Push(mouseState.MiddleButton);

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

        public void Push(ButtonState newState)
        {
            if (pastState != null)
            {
                if (newState == ButtonState.Pressed && pastState.Value == ButtonState.Released)
                {
                    IsInitialClick = true;
                    IsHeld = false;
                    IsInitialRelease = false;
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
