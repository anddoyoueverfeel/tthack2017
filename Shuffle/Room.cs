using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    public class Room
    {
        public List<Point3d> points { get; }
        public double size { get; }
        public Boolean IsValid { get
            {
                return size > 0;
            } }

        public Room(List<Point3d> points, double size)
        {
            this.points = points;
            this.size = size;
        }

        private int encode(Point3d point, int maxX)
        {
            var X = (int)Math.Round(point.X) + 10;
            var Y = (int)Math.Round(point.Y) + 10;
            return Y * maxX + X;
        }

        public List<Point3d> step()
        {
            List<Point3d> result = new List<Point3d>();
            if (points.Count > size)
            {
                return result;
            }
            int maxX = 0;
            foreach (Point3d point in points)
            {
                if (point.X > maxX)
                {
                    maxX = (int)Math.Round(point.X);
                }
            }
            maxX += 10;
            HashSet<int> grids = new HashSet<int>();
            foreach (Point3d point in points)
            {
                grids.Add(encode(point, maxX));
            }
            foreach (Point3d point in points)
            {
                foreach (int stepX in new int[3] { -1, 0, 1 }) {
                    foreach (int stepY in new int[3] { -1, 0, 1 })
                    {
                        Point3d newPoint = point + new Point3d(stepX, stepY, 0);
                        if (newPoint.X < 0 || newPoint.Y < 0)
                        {
                            continue;
                        }
                        int key = encode(newPoint, maxX);
                        if (!grids.Contains(key))
                        {
                            grids.Add(key);
                            result.Add(newPoint);
                        }
                    }
                }
            }
            return result;
        }
    }

    public class RoomGoo : GH_Goo<Room>
    {
        public override bool IsValid
        {
            get
            {
                return Value != null && Value.IsValid;
            }
        }
        
        public RoomGoo()
        {
            this.Value = null;
        }

        public RoomGoo(Room room)
        {
            Value = room;
        }

        public override string TypeDescription
        {
            get
            {
                return "This is a steppable object";
            }
        }

        public override string TypeName
        {
            get
            {
                return "Room";
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new RoomGoo(new Room(Value.points, Value.size));
        }

        public override string ToString()
        {
            return "Room";
        }
    }

    public class RoomParameter : GH_Param<RoomGoo>
    {
        public RoomParameter() : base("Room", "room", "Room parameter that contains a step function", "Shuffle", "Parameters", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("3ED8C77E-3284-4FEC-B7E3-4155D52FBDCE");
            }
        }
    }
}
