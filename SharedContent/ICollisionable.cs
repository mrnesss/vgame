using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SharedContent
{
    public interface ICollisionable
    {
        bool Collided(Rectangle rect);
    }
}
