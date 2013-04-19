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
        public Dictionary<int, String> options;
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

        public void ChangeOption(int change, int maxOption)
        {
            selectedOption += change;
            if (selectedOption < 0)
                selectedOption = maxOption - 1;
            else if (selectedOption >= maxOption)
                selectedOption = 0;
        }

        public int GetOption()
        {
            return selectedOption;
        }
    }
}
