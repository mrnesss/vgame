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
        public Vector2 startPos;
        [ContentSerializerIgnore]
        public Vector2 prevPos;
        [ContentSerializerIgnore]
        public bool isFalling;
        public float speed;
        [ContentSerializerIgnore]
        public byte dir;
        [ContentSerializerIgnore]
        public bool right;
        public float damage;

        public EnemyObject()
        {

        }

        public EnemyObject(Sprite sprite, Vector2 pos, int updateTime, float speed, float damage) : base(sprite, pos, updateTime)
        {
            this.speed = speed;
            this.damage = damage;
            this.startPos = pos;
        }

        public void UpdatePosition(Vector2 gravity, Vector2 playerPos, Rectangle playerRect)
        {
            prevPos = pos;
            //pos.X = (playerPos.X > pos.X + sprite.rect.Width) ? pos.X += speed : pos.X -= speed;
            if ((pos.X + sprite.rect.Width / 2) - playerRect.Width > playerPos.X)
                right = false;
            if ((pos.X + sprite.rect.Width / 2) + playerRect.Width < playerPos.X)
                right = true;
            if (right)
                pos.X += speed;
            else
                pos.X -= speed;
            if(isFalling)
                pos = Vector2.Add(pos, gravity);
            if (prevPos.X != pos.X)
                dir = (prevPos.X < pos.X) ? (byte)2 : (byte)1; 
        }

        public override void Initialize()
        {
            pos = startPos;
            prevPos = pos;
            isFalling = false;
            isAlive = true;
            dir = 1;
            right = false;
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
