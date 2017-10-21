using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Shuffle
{
    public class RoomBuilder : GH_Component
    {
        public RoomBuilder() : base("Room Builder", "roomBuilder", "Creating a room", "Shuffle", "Room")
        {

        }
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("0089362E-44D0-4F8E-ADBD-6C3D411290CD");
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("cells", "cells", "Cells that belong to the room", GH_ParamAccess.list);
            pManager.AddNumberParameter("minSize", "minSize", "Minimum size of the room", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new RoomParameter(), "room", "room", "A room that can evolve.", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Cell> cells = new List<Cell>();
            double size = 0;
            if (!DA.GetDataList(0, cells))
            {
                return;
            }
            if (!DA.GetData(1, ref size))
            {
                return;
            }
            DA.SetData(0, new RoomGoo(new Room(size)));
        }
    }
}
