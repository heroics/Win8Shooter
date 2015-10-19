using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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

        //Texture for the projectile 
        Texture2D projectileTexture;

        //List of projectiles
        List<Projectile> projectileList;

        //The Rate of fire for the player
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        //Animation For The Explosions
        Texture2D explosionTexture;
        List<Animation> explosionList;

        //The sound that is played when a laser is fried
        SoundEffect laserSound;

        //THe sound used when the player or an enemy die
        SoundEffect explosionSound;

        //The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        int score;
        


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

            //Intialize the projectiles
            projectileList = new List<Projectile>();

            //Set the laster to fire every quearter second
            fireTime = TimeSpan.FromSeconds(.15f);

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

            //Initalize the explisions List
            explosionList = new List<Animation>();

            //Set player's score to zero
            score = 0;

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

            //Load the The Textures
            backgroundLayer1.Initialize(Content, "Graphics\\bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            backgroundLayer2.Initialize(Content, "Graphics\\bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            enemyTexture = Content.Load<Texture2D>("Graphics\\mineAnimation");
            projectileTexture = Content.Load<Texture2D>("Graphics\\laser");
            textureBackground = Content.Load<Texture2D>("Graphics\\mainbackground");
            explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");

            //Load the music and sounds
            laserSound = Content.Load<SoundEffect>("Sounds\\laserFire");
            explosionSound = Content.Load<SoundEffect>("Sounds\\explosion");
            gameplayMusic = Content.Load<Song>("Sounds\\gameMusic");

            //Load The Score Font
            //font = Content.Load<SpriteFont>("Gothic Bozo");

            //Start the music when the game loads
            PlayMusic(gameplayMusic);

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


            //Update Textures, Classes, and Other Things
            UpdatePlayer(gameTime);
            backgroundLayer1.Update(gameTime);
            backgroundLayer2.Update(gameTime);
            UpdateEnemies(gameTime);
            UpdateProjectiles();
            UpdateCollision();
            UpdateExplosions(gameTime);

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

            //Draw the Projectiles
            for (int i = 0; i < projectileList.Count; i++)
            {
                projectileList[i].Draw(spriteBatch);
            }

            //Draw the Explosions
            for (int i = 0; i < explosionList.Count; i++)
            {
                explosionList[i].Draw(spriteBatch);
            }

            // Draw the score
            // spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            // spriteBatch.DrawString(font, "health: " + player.playerHitpoints, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

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

            //Fire only every interval set as the fireTime
            if (gameTime.TotalGameTime - previousFireTime > fireTime)
            {
                // Reset our current time
                previousFireTime = gameTime.TotalGameTime;

                //Add the projecticle, but add it to the front and center of the player
                AddProjectile(player.playerPosition + new Vector2(player.getWidth / 2, 0));

                //Play the sound of the laser
                laserSound.Play();
            }

            //Reset score if player health goes to zero
            if(player.playerHitpoints <= 0)
            {
                player.playerHitpoints = 100;
                score = 0;
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
                    if (enemiesList[i].enemyHealth <= 0)
                    {
                        //Add an explosion
                        AddExplosion(enemiesList[i].enemyPosition);

                        //PLay the exploision sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemiesList[i].enemyValue; ;
                    }
                    enemiesList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// This method will handle the Collisions for everything.
        /// Parameters: None
        /// Returns: None
        /// </summary>
        private void UpdateCollision()
        {
            //Use the rectangle's built-in intersection function to 
            // determine if tow object are overlapping

            Rectangle collisionRectangle1;
            Rectangle collisionRectangle2;

            //Only creates the rectangle once for the player
            collisionRectangle1 = new Rectangle((int)player.playerPosition.X,
                (int)player.playerPosition.Y, player.getWidth, player.getHeight);

            for (int i = 0; i < enemiesList.Count; i++)
            {
                collisionRectangle2 = new Rectangle((int)enemiesList[i].enemyPosition.X,
                    (int)enemiesList[i].enemyPosition.Y, enemiesList[i].getWidth, enemiesList[i].getHeight);

                //Determine if two objects collied with each other
                if (collisionRectangle1.Intersects(collisionRectangle2))
                {
                    // Subtract the health from the player based on the enemy damage
                    player.playerHitpoints -= enemiesList[i].enemyDamage;

                    //Since the enemy collided with the player destory it
                    enemiesList[i].enemyHealth = 0;

                    if (player.playerHitpoints <= 0)
                    {
                        player.isActive = false;
                    }

                }
            }

            //Projectile VS Enemey Collision
            for (int i = 0; i < projectileList.Count; i++)
            {
                for (int j = 0; j < enemiesList.Count; j++)
                {
                    // Create the rectangles I need to determine if we collieded with each other
                    collisionRectangle1 = new Rectangle((int)projectileList[i].Position.X - projectileList[i].Width / 2,
                        (int)projectileList[i].Position.Y - projectileList[i].Height / 2, projectileList[i].Width,
                        projectileList[i].Height);
                    collisionRectangle2 = new Rectangle((int)enemiesList[j].enemyPosition.X - enemiesList[j].getWidth / 2,
                       (int)enemiesList[j].enemyPosition.Y - enemiesList[j].getHeight / 2, enemiesList[j].getWidth, enemiesList[j].getHeight);

                    if (collisionRectangle1.Intersects(collisionRectangle2))
                    {
                        enemiesList[j].enemyHealth -= projectileList[i].Damage;
                        projectileList[i].isActive = false;
                    }
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

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport,
                projectileTexture, position);
            projectileList.Add(projectile);
        }


        /// <summary>
        /// This method will handle the projectiles that are fired by the player
        /// Parameters : None
        /// Returns: None
        /// </summary>

        private void UpdateProjectiles()
        {
            //Update the Projectiles
            for (int i = projectileList.Count - 1; i >= 0; i--)
            {
                projectileList[i].Update();
                if (projectileList[i].isActive == false)
                {
                    //If the projectile is no longer active, remove it
                    projectileList.RemoveAt(i);
                }

            }
        }

        /// <summary>
        /// This method will create the explosions
        /// Parameters: 
        ///     * Vector2 position - this will be the position to draw the explosions
        /// Returns: None
        /// </summary>

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();

            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White,
                1f, false);

            explosionList.Add(explosion);
        }

        /// <summary>
        /// This method will Update the explosions, it will remove them based the gametime
        /// Parameter:
        ///     * GameTime gameTime - this will be the time that has passed in the game
        /// Returns: None
        /// </summary>
        /// 
        
        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosionList.Count -1; i >= 0; i--)
            {
                explosionList[i].Update(gameTime);

                if(explosionList[i].isActive == false)
                {
                    explosionList.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// This method will play the music upon Start of the game
        /// Parameter:
        ///     * Song song - this will hold the audio file that I want to play
        /// Returns: None
        /// </summary>
        
        private void PlayMusic(Song song)
        {
            //Catch the expection
            try
            {
                //Play the Music
                MediaPlayer.Play(song);

                MediaPlayer.IsRepeating = true;
            }

            catch
            { }
        }

        #endregion 
    }
}
