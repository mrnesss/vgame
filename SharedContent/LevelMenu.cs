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
        [ContentSerializerIgnore]
        public Texture2D background;
        public String bg;
        public int levels;
        public Vector2[] positions;

        public LevelMenu()
        {

        }

        public LevelMenu(Texture2D background)
        {
            this.background = background;
        }
    }
}
