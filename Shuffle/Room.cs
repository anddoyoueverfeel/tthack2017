﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Shuffle
{
    public class Room
    {
        public Guid id { get; }
        public double size { get; }
        public Cell cell { get; }
        public double priority { get; }
        public String name { get; }
        public List<ShuffleBlock> blocks;

        public Boolean IsValid { get
            {
                return size > 0;
            } }

        public Room(double size, Cell cell, List<ShuffleBlock> blocks, Guid id, double priority, string name)
        {
            this.id = id;
            this.size = size;
            this.cell = cell;
            this.priority = priority;
            this.name = name;
            this.blocks = blocks;
        }
        
        public Room Duplicate()
        {
            return new Room(size, cell, blocks, id, priority, name);
        }
        
        //take a list of cells from the master solver - these input cells belong to the room
        //check the room's "rules" and "blocks" to make sure they are satisfied, too
        public Dictionary<string, int> step(List<Cell> cells)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            Point3d cellCenter = Utils.Centroid(cells);
            Debug.Print("cell center: " + cellCenter);
            bool includeAllBlocks = true;
            foreach (ShuffleBlock block in blocks) {
                Debug.Print("process block");
                Point3d outsideCenter;
                if (Utils.GetCenterForOutsideCells(block, cells, out outsideCenter))
                {
                    includeAllBlocks = false;
                    Vector3d direction = cellCenter - outsideCenter;
                    int deltaX, deltaY;
                    Utils.QuantifyDirection(direction, out deltaX, out deltaY);
                    block.setDirection(deltaX, deltaY);
                    Debug.Print(deltaX + "," + deltaY);
                } else
                {
                    block.setDirection(0, 0);
                }
            }

            if (includeAllBlocks && cells.Count > size)
            {
                return result;
            }
            HashSet<string> grids = new HashSet<string>();
            foreach (Cell point in cells)
            {
                grids.Add(point.CellAddress());
            }
            foreach (Cell point in cells)
            {
                result.Add(point.CellAddress(), (int)priority);
                foreach (int stepX in new int[3] { -1, 0, 1 }) {
                    foreach (int stepY in new int[3] { -1, 0, 1 })
                    {
                        Cell newPoint = new Cell(point.X + stepX, point.Y + stepY, id, new Dictionary<Guid, int>(), point.point);
                        if (newPoint.X < 0 || newPoint.Y < 0)
                        {
                            continue;
                        }
                        string key = newPoint.ToString();
                        if (!grids.Contains(key))
                        {
                            grids.Add(key);
                            result.Add(newPoint.CellAddress(), (int) priority);
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
            return new RoomGoo(Value.Duplicate());
        }

        public override string ToString()
        {
            return "Room " + Value.id;
        }


        public override bool CastTo<Q>(ref Q target)
        {

            if (typeof(Q).IsAssignableFrom(typeof(Room)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }
            target = default(Q);
            return false;
        }

        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from BoatShell
            if (typeof(Room).IsAssignableFrom(source.GetType()))
            {
                Value = (Room)source;
                return true;
            }

            return false;
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
