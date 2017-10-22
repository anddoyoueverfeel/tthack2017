using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    public class Cell
    {
        //id of room that "owns" this cell
        public Guid m_owner { set; get; }
        
        //key is id of the room "wants" this cell
        //value (int) is how "badly" it wants it
        public Dictionary<Guid, int> desires { set; get; }

        //Point that the cell is made from / represents / is tied to
        public Point3d point = new Point3d(0,0,0);

        //
        public int X { set; get; }
        public int Y { set; get; }

        //Constructor.. New cells initialize without an owner and with an empty list of rooms that want them.
        public Cell()
        {
            m_owner = Guid.Empty;
            desires = new Dictionary<Guid, int>();
        }

        //constructor, explicitly construct a new cell from an old one
        public Cell(int X, int Y, Guid m_owner, Dictionary<Guid, int> desires, Point3d point)
        {
            this.X = X;
            this.Y = Y;
            this.m_owner = m_owner;
            this.desires = desires;
            this.point = point;
        }

        //constructor, duplicate an old cell exactly
        public Cell(Cell oldCell)
        {
            this.X = oldCell.X;
            this.Y = oldCell.Y;
            this.m_owner = oldCell.m_owner;
            this.desires = oldCell.desires;
            this.point = oldCell.point;
        }

        //to do: refactor this
        public override string ToString()
        {
            return X + "," + Y;
        }

        public string CellAddress()
        {
            return X + "," + Y;
        }
    }


    public class CellGoo : GH_Goo<Cell>
    {
        public CellGoo()
        {
            Value = new Cell();
        }

        public CellGoo(Cell cell)
        {
            Value = cell;
        }

        //to do: is this the real validity test?
        public override bool IsValid
        {
            get
            {
                return Value.X >= 0 && Value.Y >= 0;
            }
        }


        public override string TypeDescription
        {
            get
            {
                return "Shuffle 2d cell";
            }
        }


        public override string TypeName
        {
            get
            {
                return "Shuffle Cell";
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new CellGoo(new Cell(Value.X, Value.Y, Value.m_owner, Value.desires, Value.point));
        }

        public override string ToString()
        {
            return Value.ToString();
        }


        public override bool CastTo<Q>(ref Q target)
        {

            if (typeof(Q).IsAssignableFrom(typeof(Cell)))
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
            if (typeof(Cell).IsAssignableFrom(source.GetType()))
            {
                Value = (Cell)source;
                return true;
            }

            return false;
        }
    }


    public class CellParam : GH_Param<CellGoo>
    {
        public CellParam() : base("Cell", "C", "The cell x, y", "Shuffle", "Parameters", GH_ParamAccess.item)
        {

        }
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("32551492-B80A-4618-B4E2-97D81CD52D6E");
            }
        }
    }
}
