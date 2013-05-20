using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SharedContent
{
    public class LevelMenu
    {
        public struct Level
        {
            public int level;
            public Vector2 pos;
            public bool isAvailable;
            public Level(int level, Vector2 pos, bool available)
            {
                this.level = level;
                this.pos = pos;
                this.isAvailable = available;
            }
        };
        [ContentSerializerIgnore]
        public Texture2D background;
        public String bg;
        public List<Level> levels;

        public LevelMenu()
        {

        }

        public LevelMenu(Texture2D background)
        {
            this.background = background;
        }
    }
}
