using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Punch
{
    class Inimigo
    {
        private Vector2 pos;
        private float scale;
        private Texture2D texture;

        public Inimigo(float x, float y, Texture2D texture)
        {
            pos.X = x;
            pos.Y = y;
            this.texture = texture;
            scale = 0;
        }

        public float Scale
        {
            get { return scale; }
        }

        public void Update()
        {
            scale += 0.015f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 1);
        }

        public bool Hit(Vector2 impactPoint)
        {
            if ((pos - impactPoint).Length() < 50)
                return true;
            return false;
        }
    }
}
