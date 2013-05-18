using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class Chef
    {
        public ChefSpriteEnum state;
        public ChefSpriteEnum prevState;
        public Dictionary<Enum, CharacterSprite> sprites;
        public Vector2 pos;
        public Vector2 prevPos;
        public Vector2 startPos;
        public Direction dir;
        public int frame;
        public float speed;
        public bool isAlive;
        public bool isAttacking;
        public bool isAppearing;
        public bool canAppear;
        public float health;

        public String test;

        public Chef(ChefSpriteEnum state, Vector2 pos, Direction dir, float speed)
        {
            this.state = state;
            this.pos = pos;
            this.dir = dir;
            this.speed = speed;
            sprites = new Dictionary<Enum, CharacterSprite>();
            prevPos = pos;
            health = 100.0f;
        }

        public void Initialize()
        {
            pos = startPos;
            prevPos = pos;
            prevState = state;
            frame = 0;
            health = 100.0f;
            isAlive = true;
            isAttacking = false;
            isAppearing = false;
            canAppear = true;
        }

        public void AddSprite(ChefSpriteEnum id, CharacterSprite sprite)
        {
            sprites.Add(id, sprite);
        }

        public float GetHealth()
        {
            return health;
        }

        public void SetHealth(float h)
        {
            health += h;
            if (health >= 100.0f)
                health = 100.0f;
            else if (health < 0.0f)
                health = 0.0f;
        }

        public void SetPosition(Vector2 pos)
        {
            this.pos = pos;
        }

        public void SetStartingPosition(Vector2 startPos) {
            this.startPos = startPos;
        }
    }
}
