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
        public Color[][] textureData1, textureData2;

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
            textureData1 = new Color[frames][];
            textureData2 = new Color[frames][];
        }

        public void SetTextureData(Texture2D texture)
        {
            SetTextureData1(texture);
            SetTextureData2();
        }

        public Color[][] GetTextureData(byte dir)
        {
            if (dir == 1)
                return textureData1;
            else
                return textureData2;
        }

        private void SetTextureData1(Texture2D texture)
        {
            for (int i = 0; i < frames; i++)
            {
                textureData1[i] = new Color[rect.Width * rect.Height];
                texture.GetData(0, new Rectangle(rect.Width * i, rect.Y, rect.Width, rect.Height), textureData1[i], 0, rect.Width * rect.Height);
            }
        }

        private void SetTextureData2()
        {
            for (int i = 0; i < frames; i++)
            {
                textureData2[i] = new Color[rect.Width * rect.Height];
                for (int j = 0; j < rect.Height; j++)
                {
                    for (int k = rect.Width - 1; k >= 0; k--)
                    {
                        int index = rect.Width - k - 1;
                        textureData2[i][index + (j * rect.Width)] = textureData1[i][k + (j * rect.Width)];
                    }
                }
            }
        }
    }
}
