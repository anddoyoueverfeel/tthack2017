using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Shuffle
{
    public class MyComponent1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("CreateCells", "Cells",
              "Create Cells from Points",
              "Shuffle", "sy")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points to turn into cells", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new CellParam(), "cells", "c", "cells made from points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();

            if (!DA.GetDataList(0, points)) return;

            List<Cell> createdCells = new List<Cell>();

            foreach (Point3d point in points)
            {

                int X = Convert.ToInt16(point.X);
                int Y = Convert.ToInt16(point.Y);


                Cell cell = new Shuffle.Cell(X, Y, 0, null);
                createdCells.Add(cell);
            }

            DA.SetDataList(0, createdCells);


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bbe5cbb3-37a0-4a30-bc11-1d3615a6b79d"); }
        }
    }
}