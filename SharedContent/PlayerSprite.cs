using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharedContent
{
    public class PlayerSprite
    {
        public Enum id;
        public int frames;
        public Rectangle rect;
        public Vector2 origin;
        public float speed;
        public Color[][] textureData;

        public PlayerSprite(Enum id, int frames, Rectangle rect, Vector2 origin, float speed)
        {
            this.id = id;
            this.frames = frames;
            this.rect = rect;
            this.origin = origin;
            this.speed = speed;
            textureData = new Color[frames][];
        }

        public void SetTextureData(Texture2D texture)
        {
            for (int i = 0; i < frames; i++)
            {
                textureData[i] = new Color[rect.Width * rect.Height];
                texture.GetData(0, new Rectangle(rect.Width * i, rect.Y, rect.Width, rect.Height), textureData[i], 0, rect.Width * rect.Height);
            }
        }
    }
}
