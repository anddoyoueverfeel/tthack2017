using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    class Utils
    {
        public static Point3d Centroid(List<Cell> cells)
        {
            Point3d point = new Point3d();
            foreach (Cell cell in cells)
            {
                point += cell.point;
            }
            point /= cells.Count;
            return point;
        }
    }
}
