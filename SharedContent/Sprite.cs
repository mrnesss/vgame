using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SharedContent
{
    public class Sprite
    {
        [ContentSerializerIgnore]
        public Enum id;
        [ContentSerializerIgnore]
        public Texture2D texture;
        public int frames;
        [ContentSerializerIgnore]
        public Rectangle rect;
        [ContentSerializerIgnore]
        public Vector2 origin;
        [ContentSerializerIgnore]
        public Color[][] textureData;

        public Sprite()
        {
            
        }

        public Sprite(Enum id, Texture2D texture, int frames, Rectangle rect, Vector2 origin)
        {
            this.id = id;
            this.texture = texture;
            this.frames = frames;
            this.rect = rect;
            this.origin = origin;
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
