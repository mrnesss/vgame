using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class MapObject
    {
        public Sprite sprite;
        public Vector2 pos;
        public float alpha;

        public MapObject(Sprite sprite, Vector2 pos)
        {
            this.sprite = sprite;
            this.pos = pos;
            alpha = 1.0f;
        }
    }
}
