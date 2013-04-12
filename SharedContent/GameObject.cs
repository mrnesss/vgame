using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class GameObject
    {
        public Sprite sprite;
        public Vector2 pos;
        public int frame;
        public int ellapsedTime;
        public int updateTime;
        public float alpha;
        public bool collision;
        public bool prevCollision;
        public bool alive;

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
        }
    }
}
