using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    using BoundingBox = Dictionary<string, Cell>;
    class Utils
    {
        public static string BBOX_MIN = "min";
        public static string BBOX_MAX = "max";
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
        
        public static int BringToOne(double value)
        {
            return value < 0 ? -1 : 1;
        }

        public static void QuantifyDirection(Vector3d direction, out int deltaX, out int deltaY)
        {
            double angle = Math.Atan2(direction.Y, direction.X);
            double x = Math.Cos(angle);
            double y = Math.Sin(angle);
            if (Math.Abs(x) < 0.38)
            {
                deltaX = 0;
                deltaY = BringToOne(y);
            }
            else if (Math.Abs(y) < 0.38)
            {
                deltaY = 0;
                deltaX = BringToOne(x);
            }
            else
            {
                deltaX = BringToOne(x);
                deltaY = BringToOne(y);
            }
        }

        public static bool IsCellInBoundingBox(Cell cell, BoundingBox box)
        {
            return cell >= box[BBOX_MIN] && cell <= box[BBOX_MAX];
        }
    
        public static Point3d AddCellsInBoundingBox(BoundingBox box)
        {
            Cell min = box[BBOX_MIN];
            Cell max = box[BBOX_MAX];
            return new Point3d((min.X + max.X) * (max.X - min.X) / 2 * (max.Y - min.Y),
                (min.Y + max.Y) * (max.Y - min.Y) / 2 * (max.X - min.X), 0);
        }

        public static int CountCellsInBoundingBox(BoundingBox box)
        {
            Cell min = box[BBOX_MIN];
            Cell max = box[BBOX_MAX];
            return (max.Y - min.Y) * (max.X - min.X);
        }

        public static bool GetCenterForOutsideCells(ShuffleBlock block, List<Cell> cells, out Point3d outsideCenter)
        {
            var boundingBox = block.GetBoundingBox();
            var sumPoint = AddCellsInBoundingBox(boundingBox);
            var totalCells = CountCellsInBoundingBox(boundingBox);
            var sumInsidePoint = new Point3d(0, 0, 0);
            var totalCellsInside = 0;
            foreach (Cell cell in cells)
            {
                if (IsCellInBoundingBox(cell, boundingBox))
                {
                    totalCellsInside++;
                    sumInsidePoint += cell.point;
                }
            }
            int totalCellsOutside = totalCells - totalCellsInside;
            if (totalCellsOutside == 0)
            {
                outsideCenter = new Point3d();
                return false;
            }
            Point3d outsidePoint = (Point3d)(sumPoint - sumInsidePoint);
            outsideCenter = outsidePoint / totalCellsOutside;
            return true;
        }
    }
}
