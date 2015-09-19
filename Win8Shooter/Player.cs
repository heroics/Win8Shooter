using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Win8Shooter;


namespace Shooter
{
    class Player
    {
        #region Class Level Variables

        public Animation playerAnimation;

        //Player Sprite
       // public Texture2D playerTexture;

        //Position of the player on the screen
        public Vector2 playerPosition;

        //State of the player (Dead or not)
        public bool isActive;

        //Total number of hitpoints the player has
        public int playerHitpoints;

        #endregion

        #region Properties

        //Returns the width of the player texture
        public int getWidth
        {
            get { return playerAnimation.frameWidth; }
        }

        //Returns the height of the player texture
        public int getHeight
        {
            get { return playerAnimation.frameHeight; }
        }


        #endregion

        #region Public Methods

        public void Initialize(Animation playerAnimation, Vector2 position)
        {
            //Assign the texture to the object
           this.playerAnimation = playerAnimation;

            //Set The Starting position of the player around
            playerPosition = position;

            //Set The player to be active
            isActive = true;

            //Set the player's HP
            playerHitpoints = 100;

        }

        public void Update(GameTime gameTime)
        {
            //Update the player animation

            playerAnimation.spritePosition = playerPosition;

            playerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the player to the screen
            playerAnimation.Draw(spriteBatch);
        }

        #endregion
    }
}
