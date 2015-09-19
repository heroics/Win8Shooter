using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Win8Shooter
{
    class ParallaxingBackground
    {
        #region Class Level Varaibles

        //The image representing the parallaxing background
        Texture2D backgroundTexture;

        //An Array of Positions of the Parallaxing background
        Vector2[] positionsArray;

        //The speed which the background is moving
        int backgroundSpeed;

        //The Height of the background as an int
        int backgroundHeight;

        //The Width of the background as an int
        int backgroundWidith;

        #endregion

        #region Public Methods

        public void Initialize(ContentManager content, String texturePath,
            int screentWidth, int screenHeight, int speed)
        {
            backgroundHeight = screenHeight;
            backgroundWidith = screentWidth;

            //Load the background of the background texture
            backgroundTexture = content.Load<Texture2D>(texturePath);

            //Set the speed of the background
            backgroundSpeed = speed;

            //If I divide the screen with the texture width then I can determine the number of tiles need

            //Add 1 to it so it have a gap in the tiling
            positionsArray = new Vector2[(screentWidth / backgroundTexture.Width) + 1];

            //Set the intial positions of the parallaxing background
            for (int i = 0; i < positionsArray.Length; i++)
            {
                //Need the tiles to be side by side to create a tiling effect
                positionsArray[i] = new Vector2(i * backgroundTexture.Width, 0);
            }
        }



        public void Update(GameTime gameTime)
        {
            //Update the positionsof the background
            for (int i = 0; i < positionsArray.Length; i++)
            {
                //Update the positions of the background
                positionsArray[i].X += backgroundSpeed;

                if (backgroundSpeed <= 0)
                {
                    //Check to see if the texure is out of view then put that texture at the end of the screen
                    if (positionsArray[i].X <= -backgroundTexture.Width)
                    {
                        positionsArray[i].X = backgroundTexture.Width * (positionsArray.Length - 1);
                    }
                }

                else
                {
                    //Check if the the texture is out of view then poisiton it to the start of the screen
                    if (positionsArray[i].X >= backgroundTexture.Width * (positionsArray.Length - 1))
                    {
                        positionsArray[i].X = -backgroundTexture.Width;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i <positionsArray.Length; i++)
            {
                Rectangle rectangleBackground = new Rectangle((int)positionsArray[i].X,
                    (int)positionsArray[i].Y, backgroundWidith, backgroundHeight);

                spriteBatch.Draw(backgroundTexture, rectangleBackground, Color.White);
            }
        }

        #endregion

    }
}