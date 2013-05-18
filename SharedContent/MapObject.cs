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
        [ContentSerializerIgnore]
        public bool isAlive;
        public int updateTime;
        [ContentSerializer(Optional = true)]
        public Direction dir;
        [ContentSerializer(Optional = true)]
        public float speed;
        [ContentSerializer(Optional = true)]
        public float damage;

        public MapObject()
        {

        }

        public MapObject(Sprite sprite, Vector2 pos)
        {
            this.sprite = sprite;
            this.pos = pos;
            isAlive = true;
            alpha = 1.0f;
            frame = 0;
            ellapsedTime = 0;
        }

        public MapObject(Sprite sprite, Vector2 pos, float speed, float damage) : this(sprite, pos)
        {
            this.speed = speed;
            this.damage = damage;
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

        public void SetPosition(Vector2 pos)
        {
            this.pos = pos;
        }

        public void UpdatePosition(int mapHeight)
        {
            pos.Y += speed;
            if (pos.Y > mapHeight)
                isAlive = false;
        }
    }
}
