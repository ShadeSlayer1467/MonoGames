using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Content;

namespace Pong
{
    public class Paddle
    {
        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Texture.Width / 2,
            (int)Position.Y - Texture.Height / 2,
            Texture.Width,
            Texture.Height);
        public Vector2 Position;
        public float Speed;
        public Texture2D Texture;

        // Constructor to initialize the paddle
        public Paddle(float x, float y, float speed)
        {
            Position = new Vector2(x, y);
            Speed = speed;
        }

        // Load the texture for the paddle
        public void LoadContent(ContentManager content, string textureName)
        {
            Texture = content.Load<Texture2D>(textureName);
        }

        // Update the paddle's position based on user input
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Keys upKey, Keys downKey)
        {
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(upKey))
                Position.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(downKey))
                Position.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Prevent the paddle from moving off the screen
            Position.Y = Math.Clamp(Position.Y, Texture.Height / 2, graphicsDevice.Viewport.Height - Texture.Height / 2);
        }

        // Draw the paddle on the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
    }

}
