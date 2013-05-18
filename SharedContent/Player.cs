using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class Player : ICollisionable
    {
        public PlayerSpriteEnum state;
        public PlayerSpriteEnum prevState;
        public Dictionary<Enum, CharacterSprite> sprites;
        public Vector2 pos;
        public Vector2 prevPos;
        public Vector2 startPos;
        public Direction dir;
        public int frame;
        public int level;
        public float jump;
        public Vector2 jumpVel;
        public float speed;
        public Vector2 acceleration;
        public Vector2 velocity;
        public bool isJumping;
        public bool isFalling;
        public bool canJump;
        public bool isInvincible;
        public bool blink;
        public bool isAlive;
        public float health;
        public int iFrames;
        public int iCounter;
        public int blinkCounter;

        public String test;

        public Player(PlayerSpriteEnum state, Vector2 pos, Direction dir, float speed, float jump, Vector2 acceleration)
        {
            this.state = state;
            this.pos = pos;
            this.dir = dir;
            this.jump = jump;
            this.speed = speed;
            this.acceleration = acceleration;
            sprites = new Dictionary<Enum, CharacterSprite>();
            prevPos = pos;
            jumpVel = new Vector2(0, jump);
            health = 100.0f;
        }

        public void Initialize()
        {
            pos = startPos;
            prevPos = pos;
            prevState = state;
            velocity = Vector2.Zero;
            isJumping = false;
            isFalling = false;
            canJump = false;
            frame = 0;
            level = 0;
            health = 100.0f;
            isAlive = true;
            isInvincible = false;
            iFrames = 1500;
            iCounter = 0;
            blink = false;
            blinkCounter = 0;
        }

        public void AddSprite(PlayerSpriteEnum id, CharacterSprite sprite)
        {
            sprites.Add(id, sprite);
        }

        public void UpdateDirection()
        {
            if (velocity.X < 0)
                dir = Direction.Left;
            else if (velocity.X > 0)
                dir = Direction.Right;
        }

        public void UpdateInvincibility(int gameTime)
        {
            if (isInvincible)
            {
                iCounter += gameTime;
                blinkCounter += gameTime;
                if (blinkCounter >= 250)
                {
                    blink ^= true;
                    blinkCounter = 0;
                }
            }
            if (iCounter >= iFrames)
            {
                isInvincible = false;
                blink = false;
                iCounter = 0;
            }
        }

        public void UpdateJump(Vector2 gravity)
        {
            jumpVel = Vector2.Subtract(jumpVel, Vector2.Divide(gravity, 10.0f));
            if (jumpVel.Y < -gravity.Length())
                jumpVel = -gravity;
        }

        public void ResetJump()
        {
            jumpVel.Y = jump;
        }

        public Vector2 GetJump()
        {
            return jumpVel;
        }

        public void UpdateVelocity(Vector2 a) {
            velocity = Vector2.Add(velocity, a);
            if (velocity.Length() > speed)
                velocity = Vector2.Normalize(velocity) * speed;
        }

        public void SetVelocity(Vector2 v)
        {
            velocity = v;
        }

        public Vector2 GetVelocity()
        {
            return velocity;
        }

        public bool Collided(Rectangle rect)
        {
            float distance = Math.Max(1, (float)Math.Abs(Math.Ceiling(pos.Y - prevPos.Y)));
            Rectangle r = new Rectangle((int)(pos.X - sprites[state].origin.X), (int)(pos.Y - distance), sprites[state].rect.Width, (int)distance);
            if (pos.X + 45 > rect.Left && pos.X - 45 < rect.Right && rect.Intersects(r))
                return true;
            else
                return false;
        }

        public void ChangeLevel(int change, int maxLevel)
        {
            level += change;
            if (level < 0)
                level = 0;
            else if (level >= maxLevel)
                level = maxLevel - 1;
        }

        public int GetLevel()
        {
            return level;
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

        public Vector2 GetPosition()
        {
            return pos;
        }

        public void SetStartingPosition(Vector2 startPos) {
            this.startPos = startPos;
        }
    }
}
