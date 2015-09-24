using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shooter;
using System;
using System.Collections.Generic;

namespace Win8Shooter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        #region Class Level Variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Represents The Player
        Player player;

        //Keyboard States used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        //Mouse stats used to track Mouse button Press
        MouseState currentMouseState;
        MouseState previousMouseState;

        //A movement speed for the player
        float playerMoveSpeed;

        //Image Used to display the static background
        Texture2D textureBackground;
        Rectangle rectangleBackgound;

        float scale = 1f;

        //variables for the backgroundLayer
        ParallaxingBackground backgroundLayer2;
        ParallaxingBackground backgroundLayer1;

        //Variables for the enemies
        Texture2D enemyTexture;
        List<Enemy> enemiesList;

        //The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;


        //Random Number Generator
        Random randomNumber;


        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Initialize the player class
            player = new Player();

            //Intilaize the background class
            backgroundLayer1 = new ParallaxingBackground();
            backgroundLayer2 = new ParallaxingBackground();

            //Intialize the rectangle background
            rectangleBackgound = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Set a constant player movement speed
            playerMoveSpeed = 8.0f;


            //INtialize the enemies list
            enemiesList = new List<Enemy>();

            //Set the time to zero
            previousSpawnTime = TimeSpan.Zero;

            //Used to determine how fast enemy spawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            //Initalize our random number gnerator
            randomNumber = new Random();



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);


            //Load the players resources
            Animation playerAnimation = new Animation();



            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");

            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerAnimationPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            player.Initialize(playerAnimation, playerAnimationPosition);

            //Load the background resource 
            backgroundLayer1.Initialize(Content, "Graphics\\bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            backgroundLayer2.Initialize(Content, "Graphics\\bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            enemyTexture = Content.Load<Texture2D>("Graphics\\mineAnimation");
            textureBackground = Content.Load<Texture2D>("Graphics\\mainbackground");


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();


            UpdatePlayer(gameTime);
            backgroundLayer1.Update(gameTime);
            backgroundLayer2.Update(gameTime);
            UpdateEnemies(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(textureBackground, rectangleBackgound, Color.White);

            //Draw the main background
            backgroundLayer1.Draw(spriteBatch);
            backgroundLayer2.Draw(spriteBatch);

            for (int i = 0; i < enemiesList.Count; i++)
            {
                enemiesList[i].Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Private Helper Methods

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            //Move Keyboard / DPad
            //Left
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.playerPosition.X -= playerMoveSpeed;
            }

            //Right
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.playerPosition.X += playerMoveSpeed;
            }

            //Up
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.playerPosition.Y -= playerMoveSpeed;
            }

            //Down 
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.playerPosition.Y += playerMoveSpeed;
            }

            //Mouse 
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 positionDelta = mousePosition - player.playerPosition;

                positionDelta.Normalize();

                positionDelta = positionDelta * playerMoveSpeed;

                player.playerPosition = player.playerPosition + positionDelta;
            }

            KeepInBounds();
        }

        private void AddEnemy()
        {
            //Create the animation object
            Animation enemyAnimation = new Animation();

            //Intialize the animation with the correct animation inforamtion
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            //Randomly generate the postion of the enemy
            Vector2 enemyPosition = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2,
                randomNumber.Next(100, GraphicsDevice.Viewport.Height - 100));

            //Create the enemy
            Enemy enemy = new Enemy();

            //Intilaize the enemy
            enemy.Initialze(enemyAnimation, enemyPosition);

            //Add the enemy to the active enemies list
            enemiesList.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            //Spawn a new neemy enemy every 2 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                //Create an Enemy
                AddEnemy();
            }


            //Update The Enemy
            for (int i = enemiesList.Count - 1; i >= 0; i--)
            {
                enemiesList[i].Update(gameTime);

                if (enemiesList[i].isAlive == false)
                {
                    enemiesList.RemoveAt(i);
                }
            }



        }

        private void KeepInBounds()
        {
            player.playerPosition.X = MathHelper.Clamp(player.playerPosition.X, 0,
                GraphicsDevice.Viewport.Width - player.getWidth);

            player.playerPosition.Y = MathHelper.Clamp(player.playerPosition.Y, 0,
                GraphicsDevice.Viewport.Height - player.getHeight);
        }

        #endregion 
    }
}
