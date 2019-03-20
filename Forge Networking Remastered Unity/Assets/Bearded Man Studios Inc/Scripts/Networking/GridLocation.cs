using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeardedManStudios
{
    public struct GridLocation
    {
        public int x { get; set; }
        public int y { get; set; }

        public GridLocation(float X, float Y, float cellSize)
        {
            x = (int)(X / cellSize);
            y = (int)(Y / cellSize);
        }

        public bool IsSameOrNeighbourCell(GridLocation otherLocation)
        {
            if (otherLocation.x == x && otherLocation.y == y || 
                otherLocation.x == x - 1 && otherLocation.y == y - 1 ||
                otherLocation.x == x - 1 && otherLocation.y == y ||
                otherLocation.x == x - 1 && otherLocation.y == y + 1 ||
                otherLocation.x == x && otherLocation.y == y - 1 ||
                otherLocation.x == x && otherLocation.y == y + 1 ||
                otherLocation.x == x + 1 && otherLocation.y == y - 1 ||
                otherLocation.x == x + 1 && otherLocation.y == y ||
                otherLocation.x == x + 1 && otherLocation.y == y + 1
                )
                return true;

            return false;
        }
    }
}
