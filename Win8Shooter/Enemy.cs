using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Win8Shooter
{
    class Enemy
    {
        #region Class Level Variables
        //Animation Representing the Enemy
        public Animation enemyAnimation;

        //The positionof the enemy ship realitve to the top left of the screen
        public Vector2 enemyPosition;

        //The State of the enemy ship
        public bool isAlive;

        //The hitpoints of the enemy, if this goes to zero the enemy dies
        public int enemyHealth;

        //The amount of damage the enemy inflicts on player ship
        public int enemyDamage;

        //The score value of the enemy 
        public int enemyValue;

        //The speed in which the enemy moves
        float enemyMovementSpeed;

        //Get the Width of the enemy ship
        public int getWidth
        {
            get { return enemyAnimation.frameWidth; }
        }

        //Get the Height of the enemy ship
        public int getHeight
        {
            get { return enemyAnimation.frameHeight;  }
        }

        #endregion

        #region Public Methods

        public void Initialze(Animation animation, Vector2 position)
        {
            //Load the enemy ship texture
            this.enemyAnimation = animation;
            
            //Set the position of the enemy
            this.enemyPosition = position;

            //Intialize the enemy to alive so it will update
            this.isAlive = true;

            //Set the Health of the enemy ship
            enemyHealth = 8;

            //Set the damage the enemy can do
            enemyDamage = 10;

            //Set how fast the enemy moves
            enemyMovementSpeed = 5f;

            //Set the value of the enemy
            enemyValue = 100;
        }



        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement its x position
            enemyPosition.X -= enemyMovementSpeed;

            //Update the position of the animation
            enemyAnimation.spritePosition = enemyPosition;

            //Update the animation for the enemy
            enemyAnimation.Update(gameTime);

            if(enemyPosition.X < -getWidth || enemyHealth <=0)
            {
                //Delete the enemy by setting it false, which will remove the object.
                isAlive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw The Enemy On the Screen
            enemyAnimation.Draw(spriteBatch);
        }

        #endregion

    }
}
