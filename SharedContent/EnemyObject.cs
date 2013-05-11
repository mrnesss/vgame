using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SharedContent
{
    public class EnemyObject : GameObject, ICollisionable
    {
        [ContentSerializerIgnore]
        public Vector2 prevPos;
        [ContentSerializerIgnore]
        public bool isFalling;
        public float speed;
        [ContentSerializerIgnore]
        public byte dir;

        public EnemyObject()
        {

        }

        public EnemyObject(Sprite sprite, Vector2 pos, int updateTime, float speed) : base(sprite, pos, updateTime)
        {
            this.speed = speed;
            prevPos = pos;
            isFalling = false;
            dir = 1;
        }

        public void UpdatePosition(Vector2 gravity, Vector2 playerPos)
        {
            pos.X = (playerPos.X > pos.X) ? pos.X += speed : pos.X -= speed;
            if(isFalling)
                pos = Vector2.Add(pos, gravity);
        }

        public bool Collided(Rectangle rect)
        {
            float distance = Math.Max(1, (float)Math.Abs(Math.Ceiling(pos.Y - prevPos.Y)));
            Rectangle r = new Rectangle((int)(pos.X), (int)(pos.Y - distance + sprite.rect.Height), sprite.rect.Width, (int)distance);
            if (pos.X + sprite.rect.Width > rect.Left && pos.X < rect.Right && rect.Intersects(r))
                return true;
            else
                return false;
        }
    }
}
