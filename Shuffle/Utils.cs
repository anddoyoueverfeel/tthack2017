using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Shuffle
{
    using BoundingBox = Dictionary<string, Cell>;
    class Utils
    {
        public static string BBOX_MIN = "bottomLeft";
        public static string BBOX_MAX = "topRight";
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

            Debug.Print("Direction: " + direction + " angle: " + angle + " x: " + x + " y: " + y);
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
            return cell.X >= box[BBOX_MIN].X + 0.7 && cell.Y >= box[BBOX_MIN].Y + 0.7 &&
                cell.X <= box[BBOX_MAX].X - 0.7 && cell.Y <= box[BBOX_MAX].Y - 0.7;
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
            if (block == null)
            {
                outsideCenter = new Point3d();
                return false;
            }
            
            var boundingBox = block.GetBoundingBox();/*
            boundingBox[BBOX_MIN] = new Cell(0, 0, Guid.Empty, new Dictionary<Guid, int>(), new Point3d(0, 0, 0));
            boundingBox[BBOX_MAX] = new Cell(60, 60, Guid.Empty, new Dictionary<Guid, int>(), new Point3d(60, 60, 0));
            */
            Debug.Print("boundingBox: " + boundingBox[BBOX_MIN] + "." + boundingBox[BBOX_MAX] + "\n");
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
            Debug.Print("sumInsidePoint: " + sumInsidePoint + "\n");
            int totalCellsOutside = totalCells - totalCellsInside;
            Debug.Print("total cells: " + totalCells + "\n");
            Debug.Print("total inside cells: " + totalCellsInside + "\n");
            if (totalCellsOutside < 100)
            {
                outsideCenter = new Point3d();
                return false;
            }
            Point3d outsidePoint = (Point3d)(sumPoint - sumInsidePoint);
            outsideCenter = outsidePoint / totalCellsOutside;
            Debug.Print("outside center: " + outsideCenter + "\n");
            return true;
        }
    }
}
