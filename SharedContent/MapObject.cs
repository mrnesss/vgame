using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SharedContent
{
    public class MapObject
    {
        [ContentSerializerIgnore]
        public Texture2D texture;
        [ContentSerializer(FlattenContent = true)]
        public Sprite sprite;
        [ContentSerializerIgnore]
        public Vector2 pos;
        [ContentSerializerIgnore]
        public float alpha;
        [ContentSerializerIgnore]
        public int frame;
        [ContentSerializerIgnore]
        public int ellapsedTime;
        public int updateTime;

        public MapObject()
        {

        }

        public MapObject(Sprite sprite, Vector2 pos)
        {
            this.sprite = sprite;
            this.pos = pos;
            alpha = 1.0f;
            frame = 0;
            ellapsedTime = 0;
        }

        public MapObject(Texture2D texture, Vector2 pos)
        {
            this.texture = texture;
            this.pos = pos;
            alpha = 1.0f;
        }

        public void UpdateAnimation(int time)
        {
            ellapsedTime += time;
            if (ellapsedTime >= updateTime)
            {
                frame = (frame + 1) % sprite.frames;
                ellapsedTime = 0;
            }
        }
    }
}
