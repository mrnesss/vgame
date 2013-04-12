using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class SpriteInfo
    {
        public Dictionary<CollectibleEnum, Sprite> collectibles;
        public Dictionary<EnemyEnum, Sprite> enemies;
    }
}
