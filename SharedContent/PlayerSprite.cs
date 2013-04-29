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
        public Color[][] textureData;

        public PlayerSprite(Enum id, List<Texture2D> textures, Vector2 origin)
        {
            this.id = id;
            this.textures = textures;
            this.origin = origin;
            rect = textures.ElementAt(0).Bounds;
            frames = textures.Count;
            textureData = new Color[frames][];
        }

        public PlayerSprite(Enum id, List<Texture2D> textures, int frames, Rectangle rect, Vector2 origin, float speed)
        {
            this.id = id;
            this.textures = textures;
            this.frames = frames;
            this.rect = rect;
            this.origin = origin;
            this.speed = speed;
            textureData = new Color[frames][];
        }

        public void SetTextureData()
        {
            for (int i = 0; i < frames; i++)
            {
                textureData[i] = new Color[rect.Width * rect.Height];
                textures.ElementAt(i).GetData(0, new Rectangle(0, 0, rect.Width, rect.Height), textureData[i], 0, rect.Width * rect.Height);
            }
        }
    }
}
