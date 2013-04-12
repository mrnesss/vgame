using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public class ObjectInfo
    {
        public Dictionary<CollectibleEnum, GameObject> collectibles;
        public Dictionary<EnemyEnum, EnemyObject> enemies;
    }
}
