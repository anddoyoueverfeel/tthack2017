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
        public Guid id { get; }
        public double size { get; }
        public Boolean IsValid { get
            {
                return size > 0;
            } }

        public Room(double size)
        {
            this.id = new Guid();
            this.size = size;
        }
        
        public Dictionary<string, int> step(List<Cell> points)
        {
            int priority = 0;
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (points.Count > size)
            {
                return result;
            }
            HashSet<string> grids = new HashSet<string>();
            foreach (Cell point in points)
            {
                grids.Add(point.ToString());
            }
            foreach (Cell point in points)
            {
                foreach (int stepX in new int[3] { -1, 0, 1 }) {
                    foreach (int stepY in new int[3] { -1, 0, 1 })
                    {
                        Cell newPoint = new Cell(point.X + stepX, point.Y + stepY, 0, null);
                        if (newPoint.X < 0 || newPoint.Y < 0)
                        {
                            continue;
                        }
                        string key = newPoint.ToString();
                        if (!grids.Contains(key))
                        {
                            grids.Add(key);
                            result.Add(newPoint.ToString(), priority);
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
            return new RoomGoo(new Room(Value.size));
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
