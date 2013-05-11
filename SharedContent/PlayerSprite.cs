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
        public List<Texture2D> textures;
        public int frames;
        public Rectangle rect;
        public Vector2 origin;
        public float speed;
        public Color[][] textureData1, textureData2;

        public PlayerSprite(Enum id, List<Texture2D> textures, Vector2 origin)
        {
            this.id = id;
            this.textures = textures;
            this.origin = origin;
            rect = textures.ElementAt(0).Bounds;
            frames = textures.Count;
            textureData1 = new Color[frames][];
            textureData2 = new Color[frames][];
        }

        public PlayerSprite(Enum id, List<Texture2D> textures, int frames, Rectangle rect, Vector2 origin, float speed)
        {
            this.id = id;
            this.textures = textures;
            this.frames = frames;
            this.rect = rect;
            this.origin = origin;
            this.speed = speed;
            textureData1 = new Color[frames][];
            textureData2 = new Color[frames][];
        }

        public void SetTextureData()
        {
            SetTextureData1();
            SetTextureData2();
        }

        public Color[][] GetTextureData(Direction dir)
        {
            if (dir == Direction.Right)
                return textureData1;
            else
                return textureData2;
        }

        private void SetTextureData1()
        {
            for (int i = 0; i < frames; i++)
            {
                textureData1[i] = new Color[rect.Width * rect.Height];
                textures.ElementAt(i).GetData<Color>(textureData1[i], 0, textureData1[i].Length);
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
