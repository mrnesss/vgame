using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SharedContent
{
    public class Map
    {
        public struct Scenery
        {
            public MapEnum type;
            public Vector2 pos;
        };

        public struct Platform
        {
            public MapEnum type;
            public Vector2 pos;
        };

        public struct Item
        {
            public CollectibleEnum type;
            public Vector2 pos;
        };

        public struct Enemy
        {
            public EnemyEnum type;
            public Vector2 pos;
        };

        public struct Interactive
        {
            public MapEnum type;
            public Vector2 pos;
            public Direction dir;
        };

        [ContentSerializerIgnore]
        public bool isCompleted;
        public String background;
        public TimeSpan time;
        public Vector2 startPos;
        public Rectangle rect;
        public List<Interactive> interactives;
        public List<Scenery> scenery;
        public List<Platform> platforms;
        public List<Item> items;
        public List<Enemy> enemies;
        public Dictionary<CollectibleEnum, int[]> collectibles;

        public Map()
        {
            isCompleted = false;
        }

        public List<String> GetPlatformTypes()
        {
            List<String> types = new List<String>();
            foreach (Platform e in platforms)
            {
                if (!types.Contains(e.type.ToString()))
                    types.Add(e.type.ToString());
            }
            return types;
        }

        public List<String> GetItemTypes()
        {
            List<String> types = new List<String>();
            foreach (Item e in items)
            {
                if(!types.Contains(e.type.ToString()))
                    types.Add(e.type.ToString());
            }
            return types;
        }

        public List<String> GetEnemyTypes()
        {
            List<String> types = new List<String>();
            foreach (Enemy e in enemies)
            {
                if (!types.Contains(e.type.ToString()))
                    types.Add(e.type.ToString());
            }
            return types;
        }

        public List<String> GetSceneryTypes()
        {
            List<String> types = new List<String>();
            foreach (Scenery e in scenery)
            {
                if (!types.Contains(e.type.ToString()))
                    types.Add(e.type.ToString());
            }
            return types;
        }

        public List<String> GetInteractiveTypes()
        {
            List<String> types = new List<String>();
            foreach (Interactive e in interactives)
            {
                if (!types.Contains(e.type.ToString()))
                    types.Add(e.type.ToString());
            }
            return types;
        }
    }
}
