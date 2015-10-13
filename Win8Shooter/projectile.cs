using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Win8Shooter
{
    class Projectile
    {
        #region Fields and Properties

        //Image representing the Projectile
        public Texture2D Texture;

        // The Position of the projectile realitve to the upper left side
        // of the screen
        public Vector2 Position;

        //State of the Projectile
        public bool isActive;

        //The Amount of damage the projectile can inflict  
        public int Damage;

        //This holds the vieable boundary of the game
        Viewport viewport;


        //Get the Width of projectle
        public int Width
        {
            get { return Texture.Width; }
        }

        //Get the Height of the ship
        public int Height
        {
            get { return Texture.Height; }
        }

        float projectileMoveSped;

        #endregion

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;
            this.viewport = viewport;
            isActive = true;
            Damage = 2;

            projectileMoveSped = 20f;
        }

        public void Update()
        {
            //Projectiles always move to the right
            Position.X += projectileMoveSped;

            //Deactivate the bullet if ti goes off screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
                new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
