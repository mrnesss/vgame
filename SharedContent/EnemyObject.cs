using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class EnemyObject : GameObject
    {
        public float speed;

        public EnemyObject()
        {

        }

        public EnemyObject(Sprite sprite, Vector2 pos, int updateTime, float speed) : base(sprite, pos, updateTime)
        {
            this.speed = speed;
        }

        public void UpdatePosition(Vector2 gravity)
        {
            pos.X += speed;
            pos += gravity;
        }
    }
}
