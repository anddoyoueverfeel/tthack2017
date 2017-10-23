using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Shuffle
{
    public class NodeBlockBuilder : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public NodeBlockBuilder()
          : base("Block Builder", "B Build",
              "Description",
              "Shuffle", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //to do: make the thing smart enough to calculate its own corner points..
            pManager.AddCurveParameter("ObjectGeom", "OGeom", "Curves that represent the object itself", GH_ParamAccess.list);
            pManager.AddPointParameter("ObjectCorner1", "OC1", "The bottom left corner of the object", GH_ParamAccess.item);
            pManager.AddPointParameter("ObjectCorner2", "OC2", "The top right corner of the object", GH_ParamAccess.item);

            pManager.AddCurveParameter("ClearGeom", "CGeom", "Curves that represent clearances that must be maintained around the object", GH_ParamAccess.list);
            pManager.AddPointParameter("ClearCorner1", "CC1", "The bottom left corner of the clearance box", GH_ParamAccess.item);
            pManager.AddPointParameter("ClearCorner2", "CC2", "The top right corner of the clearance box", GH_ParamAccess.item);

            pManager.AddPointParameter("Center", "C", "Center-ish point of the object, needed for internal stuff", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority","P","Priority determines how powerfully this object's clearances need to be enforced", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ShuffleBlockParam(), "Block", "B", "A shuffle block for inserting into a shuffle room", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> objGeometry = new List<Curve>();
            Point3d objectCorner1 = new Point3d();
            Point3d objectCorner2 = new Point3d();

            List<Curve> clearGeometry = new List<Curve>();
            Point3d clearanceCorner1 = new Point3d();
            Point3d clearanceCorner2 = new Point3d();

            Point3d centerPoint = new Point3d();
            int priority = -1;

            if (!DA.GetDataList(0, objGeometry)) return;
            if (!DA.GetData(1, ref objectCorner1)) return;
            if (!DA.GetData(2, ref objectCorner2)) return;

            if (!DA.GetDataList(3, clearGeometry)) return;
            if (!DA.GetData(4, ref clearanceCorner1)) return;
            if (!DA.GetData(5, ref clearanceCorner2)) return;

            if (!DA.GetData(6, ref centerPoint)) return;
            if (!DA.GetData(7, ref priority)) return;

            Dictionary<string, int> cornerOffsets = new Dictionary<string, int>();


            int centerX = Convert.ToInt16(centerPoint.X);
            int centerY = Convert.ToInt16(centerPoint.Y);

            int clearanceCorner1X = Convert.ToInt16(clearanceCorner1.X);
            int clearanceCorner1Y = Convert.ToInt16(clearanceCorner1.Y);

            int clearanceCorner2X = Convert.ToInt16(clearanceCorner2.X);
            int clearanceCorner2Y = Convert.ToInt16(clearanceCorner2.Y);

            int objectCorner1X = Convert.ToInt16(objectCorner1.X);
            int objectCorner1Y = Convert.ToInt16(objectCorner1.Y);

            int objectCorner2X = Convert.ToInt16(objectCorner2.X);
            int objectCorner2Y = Convert.ToInt16(objectCorner2.Y);


            cornerOffsets.Add("toBoundingBottomLeftX", clearanceCorner1X - centerX);
            cornerOffsets.Add("toBoundingBottomLeftY", clearanceCorner1Y - centerY);
            cornerOffsets.Add("toBoundinTopRightX", clearanceCorner2X - centerX);
            cornerOffsets.Add("toBoundinTopRightY", clearanceCorner2Y - centerY);


            cornerOffsets.Add("toObjectBottomLeftX", objectCorner1X - centerX);
            cornerOffsets.Add("toObjectBottomLeftY", objectCorner1Y - centerY);
            cornerOffsets.Add("toObjectinTopRightX", objectCorner2X - centerX);
            cornerOffsets.Add("toObjectinTopRightY", objectCorner2Y - centerY);
            

            ShuffleBlock myBlock = new ShuffleBlock(centerPoint, cornerOffsets, priority, objGeometry, clearGeometry);

            DA.SetData(0, myBlock);

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
            get { return new Guid("b40f76a3-380d-4064-b3be-f39accf07b13"); }
        }
    }
}