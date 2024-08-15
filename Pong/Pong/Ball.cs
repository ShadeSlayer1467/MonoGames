using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Pong
{
    public class Ball
    {
        public Rectangle BoundingBox => new Rectangle((int)Position.X - Texture.Width / 2,
                                              (int)Position.Y - Texture.Height / 2,
                                              Texture.Width,
                                              Texture.Height);

        public Vector2 Position;
        public Vector2 Velocity;
        public float Speed;
        public Texture2D Texture;
        private static Random Random = RandomSingleton.Instance;

        public Ball(float x, float y, float speed)
        {
            Position = new Vector2(x, y);
            Speed = speed;
            Velocity = new Vector2(0,0);
        }

        public void LoadContent(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {

            // Reverse direction if hitting screen bounds
            if (Position.X < 0)
            {
                Position.X = 0; // Reset position to the edge
                Velocity.X = -Velocity.X;
            }
            else if (Position.X > graphicsDevice.Viewport.Width)
            {
                Position.X = graphicsDevice.Viewport.Width; // Reset position to the edge
                Velocity.X = -Velocity.X;
            }

            if (Position.Y < 0)
            {
                Position.Y = 0; // Reset position to the edge
                Velocity.Y = -Velocity.Y;
            }
            else if (Position.Y > graphicsDevice.Viewport.Height)
            {
                Position.Y = graphicsDevice.Viewport.Height; // Reset position to the edge
                Velocity.Y = -Velocity.Y;
            }

            // Update the position based on velocity and elapsed time
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
        public void StartMoving(int viewportWidth, int viewportHeight)
        {
            Position = new Vector2(viewportWidth / 2, viewportHeight / 2);

            double angle = (Random.NextDouble() * Math.PI / 3) - Math.PI / 6;
            if (Random.Next(2) == 1)
            {
                angle += Math.PI;
            }

            Velocity = new Vector2(
                (float)(Speed * Math.Cos(angle)),
                (float)(Speed * Math.Sin(angle))
            );
        }
    }
}

