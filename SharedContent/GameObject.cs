using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SharedContent
{
    public class GameObject
    {
        [ContentSerializer(FlattenContent = true)]
        public Sprite sprite;
        [ContentSerializerIgnore]
        public Vector2 pos;
        [ContentSerializerIgnore]
        public int frame;
        [ContentSerializerIgnore]
        public int ellapsedTime;
        public int updateTime;
        [ContentSerializerIgnore]
        public float alpha;
        public float speed;
        [ContentSerializerIgnore]
        public bool collision;
        [ContentSerializerIgnore]
        public bool prevCollision;
        [ContentSerializerIgnore]
        public bool alive;

        public GameObject()
        {

        }

        public GameObject(Sprite sprite, Vector2 pos, int updateTime)
        {
            this.sprite = sprite;
            this.pos = pos;
            this.updateTime = updateTime;
            frame = 0;
            ellapsedTime = 0;
            alpha = 1.0f;
            collision = false;
            prevCollision = false;
            alive = true;
            speed = 0;
        }

        public GameObject(Sprite sprite, Vector2 pos, int updateTime, float speed) : this(sprite, pos, updateTime)
        {
            this.speed = speed;
        }

        public void UpdateCollision()
        {
                prevCollision = collision;
        }

        public bool Collided()
        {
            if (!prevCollision && collision)
                return true;
            else return false;
        }

        public void Update(int time)
        {
            ellapsedTime += time;
            if (ellapsedTime >= updateTime)
            {
                frame = (frame + 1) % sprite.frames;
                ellapsedTime = 0;
            }
            pos.X += speed;
        }
    }
}
