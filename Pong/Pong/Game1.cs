using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Pong
{
    public class Game1 : Game
    {
        private enum GameState
        {
            WaitingToStart,
            Playing,
            Scored
        }
        private GameState currentState = GameState.WaitingToStart;
        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont gameFont;

        private Ball ball;
        private const float MaxBounceAngle = 2f * MathHelper.Pi / 12f; // 75 degrees
        private const float BallSpeed = 200f; 

        private Paddle paddle1;
        private Paddle paddle2;

        int joystickDeadZone;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialize ball
            ball = new Ball(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2, BallSpeed);

            // Initialize paddles for two players
            paddle1 = new Paddle(30, _graphics.PreferredBackBufferHeight / 2, 300f);  // Left paddle
            paddle2 = new Paddle(_graphics.PreferredBackBufferWidth - 30, _graphics.PreferredBackBufferHeight / 2, 300f);  // Right paddle

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameFont = Content.Load<SpriteFont>("GameFont");

            // TODO: use this.Content to load your game content here
            Texture2D ballTexture = Content.Load<Texture2D>("ball");
            ball.LoadContent(ballTexture);

            // Load content for paddles, assuming a texture named "paddle"
            paddle1.LoadContent(Content, "paddle");
            paddle2.LoadContent(Content, "paddle");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // Handle input to start the game
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && currentState == GameState.WaitingToStart)
            {
                currentState = GameState.Playing;
                // Initialize or reset the ball's direction and speed when starting
                ball.StartMoving(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);
            }
            if (currentState == GameState.Playing)
            {
                ball.Update(gameTime, _graphics.GraphicsDevice);
                CheckPaddleCollisions(gameTime);

                // Additional logic to handle the ball going off the screen sides
                HandleBallOffScreen();
            }

            // Update ball and paddles
            paddle1.Update(gameTime, _graphics.GraphicsDevice, Keys.W, Keys.S);  // Controls for paddle 1
            paddle2.Update(gameTime, _graphics.GraphicsDevice, Keys.Up, Keys.Down);  // Controls for paddle 2

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Vector2 messageSize;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            ball.Draw(_spriteBatch);
            paddle1.Draw(_spriteBatch);
            paddle2.Draw(_spriteBatch);


            // Display scores
            string scoreText = $"P1:{scorePlayer1}";
            _spriteBatch.DrawString(gameFont, scoreText, new Vector2(10, 10), Color.Black);


            string scoreValues = $"P2:{scorePlayer2}";
            messageSize = gameFont.MeasureString(scoreValues);
            _spriteBatch.DrawString(gameFont, scoreValues, new Vector2(_graphics.PreferredBackBufferWidth - messageSize.X - 10, 10), Color.Black);

            // Conditionally display the start message
            if (currentState == GameState.WaitingToStart)
            {
                string startMessage = "Press Space to Start";
                messageSize = gameFont.MeasureString(startMessage);
                _spriteBatch.DrawString(gameFont, startMessage, new Vector2((_graphics.PreferredBackBufferWidth - messageSize.X) / 2, _graphics.PreferredBackBufferHeight / 2), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckPaddleCollisions(GameTime gameTime)
        {
            Vector2 futurePosition = ball.Position + ball.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rectangle futureBoundingBox = new Rectangle(
                (int)futurePosition.X - ball.Texture.Width / 2,
                (int)futurePosition.Y - ball.Texture.Height / 2,
                ball.Texture.Width,
                ball.Texture.Height);

            if (futureBoundingBox.Intersects(paddle1.BoundingBox))
            {
                HandlePaddleCollision(paddle1, ball, gameTime);
            }
            if (futureBoundingBox.Intersects(paddle2.BoundingBox))
            {
                HandlePaddleCollision(paddle2, ball, gameTime);
            }
        }


        private void HandlePaddleCollision(Paddle paddle, Ball ball, GameTime gameTime)
        {
            Debug.WriteLine($"Ball position: {ball.Position}, Paddle position: {paddle.Position}");

            ball.Velocity.X = -ball.Velocity.X;

            var relativeIntersectY = paddle.Position.Y - ball.Position.Y;
            var normalizedRelativeIntersectionY = relativeIntersectY / (paddle.Texture.Height / 2);
            var bounceAngle = normalizedRelativeIntersectionY * MaxBounceAngle;

            bool ballOnLeftSide = ball.Position.X < paddle.Position.X + paddle.Texture.Width / 2;

            if (ballOnLeftSide)
            {
                ball.Velocity.X = -(float)(BallSpeed * Math.Cos(bounceAngle));
            }
            else
            {
                ball.Velocity.X = (float)(BallSpeed * Math.Cos(bounceAngle));
            }
            ball.Velocity.Y = (float)(BallSpeed * -Math.Sin(bounceAngle));


            // Move the ball to the edge of the paddle to avoid multiple collisions
            if (ball.Velocity.X > 0)
            {
                ball.Position.X = paddle.Position.X + (paddle.Texture.Width / 2) + (ball.Texture.Width / 2);
            }
            else
            {
                ball.Position.X = paddle.Position.X - (ball.Texture.Width / 2);
            }
            Debug.WriteLine($"Bounce angle: {bounceAngle}, Ball velocity: {ball.Velocity}");
        }
        private float map(float x, float a, float b, float p, float q)
        {
            return p + (x - a) * (q - p) / (b - a);
        }

        private void HandleBallOffScreen()
        {
            if (ball.Position.X < 0) // Left side
            {
                scorePlayer2++;
                currentState = GameState.Scored;
                ResetBall();
            }
            else if (ball.Position.X > _graphics.PreferredBackBufferWidth) // Right side
            {
                scorePlayer1++;
                currentState = GameState.Scored;
                ResetBall();
            }
        }

        private void ResetBall()
        {
            // Position the ball back at the center
            ball.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            ball.Velocity = Vector2.Zero;  // Stop the ball
            currentState = GameState.WaitingToStart;
        }
    }
}