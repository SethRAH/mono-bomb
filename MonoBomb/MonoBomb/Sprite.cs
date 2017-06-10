using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoBomb
{
    public class Sprite : IActor
    {
        protected readonly Rectangle screenBounds;
        protected readonly Texture2D texture;
        public Vector2 Location;
        public Vector2 Velocity { get; set; }

        public float transparency = 1.0f;

        public int Width
        {
            get
            {
                return texture.Width;
            }
        }

        public int Height
        {
            get
            {
                return texture.Height;
            }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Location.X, (int)Location.Y, Width, Height);
            }
        }

        public Sprite(Texture2D texture, Vector2 location, Rectangle screenBounds)
        {
            this.texture = texture;
            this.Location = location;
            this.Velocity = Vector2.Zero;
            this.screenBounds = screenBounds;
        }

        public virtual void Update(GameTime gameTime, GameObjectContainer gameObjects)
        {
            Location += Velocity;

            CheckBounds();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.Location, Color.White * transparency);
        }

        protected virtual void CheckBounds() { }
    }

    
}
