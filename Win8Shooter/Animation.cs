using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Win8Shooter
{
    class Animation
    {
        #region Class Level Variables 

        //The Image Representing the collection of images used for animation
        Texture2D spriteSheet;

        //The Scale used to display the Spirte Sheet
        float spriteScale;

        //The time since we last updated the frame
        int elapsedTime;

        //The time we display a frame until the next one
        int frameTime;

        //The number of frames the animation
        int frameTotal;

        //The index of the current frame we are displaying
        int currentFrame;

        //The color of the frame we will be displaying
        Color spriteColor;

        //The area of the sprite sheet we want to display
        Rectangle sourceRectangle = new Rectangle();

        //The Area where I want to display the image strip in the game
        Rectangle destinationRectangle = new Rectangle();

        //Width of a given frame
        public int frameWidth;

        //Height of a given frame
        public int frameHeight;

        //Determines if the animation will keep playing or deactivate after one run
        bool isLooping;

        //The state of the Animation
        bool isActive;

        //The Postion of the animation
        public Vector2 spritePosition;

        #endregion

        #region Public Methods

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight,
            int frameTotal, int frameTime, Color color, float scale, bool looping)
        {
            //Keep a local copy of the values passed in
            this.spriteSheet = texture;
            this.frameTotal = frameTotal;
            this.spritePosition = position;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameTime = frameTime;
            this.spriteColor = color;
            this.spriteScale = scale;
            this.isLooping = looping;

            //Set the time to zero
            elapsedTime = 0;

            currentFrame = 0;

            //Set the Animation to by default
            isActive = true;
        }

        public void Update(GameTime gameTime)
        {
            //Do not update the game if we are not active
            if (isActive == false)
            {
                return;
            }

            //Update the elasped time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //If the elasped time is large than the frame time then switch frames
            if (elapsedTime > frameTime)
            {
                //Move to the next frame
                currentFrame++;

                //If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameTotal)
                {
                    currentFrame = 0;

                }

                //If we are not loping deactivate the animation 
                if (isLooping == false)
                {
                    isActive = false;
                }

                //reset the elapsed time back to zero
                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip bt multipying the currentFrame index by the FrameWidth
            sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            destinationRectangle = new Rectangle((int)spritePosition.X - (int)(frameWidth * spriteScale) / 2,
                (int)spritePosition.Y - (int)(frameHeight * spriteScale) / 2,
                (int)(frameWidth * spriteScale),
                (int)(frameHeight * spriteScale));


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Only draw the animation when they are active
            if (isActive)
            {
                spriteBatch.Draw(spriteSheet, destinationRectangle, sourceRectangle, spriteColor);
            }
        }


        #endregion
    }
}
