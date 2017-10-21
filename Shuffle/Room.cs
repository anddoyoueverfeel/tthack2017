using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    public class Room : GH_Component
    {
        public override Guid ComponentGuid
        {
            get { return new Guid("19EFD237-E973-4BFD-96E1-8687CE2ED3F8"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("cells", "cells", "Cells that belong to the room", GH_ParamAccess.list);
            pManager.AddNumberParameter("minSize", "minSize", "Minimum size of the room", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("result cells", "cells", "Cells that the room wants to grow", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point> points = new List<Point>();
            double size = 0.0;

            if (!DA.GetData(0, ref points)) return;
            if (!DA.GetData(1, ref size)) return;


        }
    }
}
