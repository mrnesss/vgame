using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SharedContent
{
    public class StartMenu
    {
        public String bg;
        public String[] options;
        [ContentSerializerIgnore]
        public int selectedOption;
        [ContentSerializerIgnore]
        public Texture2D background;

        public StartMenu()
        {

        }

        public StartMenu(Texture2D background)
        {
            this.background = background;
            selectedOption = 0;
        }
    }
}
