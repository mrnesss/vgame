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
        [ContentSerializerIgnore]
        public bool collision;
        [ContentSerializerIgnore]
        public bool prevCollision;
        [ContentSerializerIgnore]
        public bool isAlive;
        [ContentSerializerIgnore]
        public float distance;
        [ContentSerializerIgnore]
        public float moved;
        [ContentSerializerIgnore]
        public bool up;

        public GameObject()
        {

        }

        public GameObject(Sprite sprite, Vector2 pos, int updateTime)
        {
            this.sprite = sprite;
            this.pos = pos;
            this.updateTime = updateTime;
        }

        public virtual void Initialize()
        {
            frame = 0;
            ellapsedTime = 0;
            alpha = 1.0f;
            collision = false;
            prevCollision = false;
            isAlive = true;
            distance = 5.0f;
            moved = 0.0f;
            up = false;
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

        public void UpdatePosition()
        {
            float step = 25.0f;
            if (Math.Abs(moved) > distance)
                up ^= true;
            if (up)
            {
                pos.Y += distance / step;
                moved += distance / step;
            }
            else
            {
                pos.Y -= distance / step;
                moved -= distance / step;
            }
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
