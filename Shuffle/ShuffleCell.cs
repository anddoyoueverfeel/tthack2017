using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    public class Cell
    {
        //id of room that "owns" this cell
        public int m_owner { set; get; }
        //first int is what room "wants" this cell
        //second int is how "badly" it wants it
        public Dictionary<Guid, int> desires { set; get; }

        public int X { set; get; }
        public int Y { set; get; }

        public Cell()
        {
            m_owner = 0;
        }

        public Cell(int X, int Y, int m_owner, Dictionary<Guid, int> desires)
        {
            this.X = X;
            this.Y = Y;
            this.m_owner = m_owner;
            this.desires = desires;
        }

        public override string ToString()
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
                return "Cell";
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new CellGoo(new Cell(Value.X, Value.Y, Value.m_owner, Value.desires));
        }

        public override string ToString()
        {
            return "Cell Goo";
        }
    }

    public class CellParam : GH_Param<CellGoo>
    {
        CellParam() : base("Cell", "C", "The cell x, y", "Shuffle", "Parameters", GH_ParamAccess.item)
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
