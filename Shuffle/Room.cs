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
        public Room() : base("ShuttleRoom", "Room", "When connecting to the solver, updates the cells", "Shuttle", "Room")
        {
        }
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

        private int encode(Point3d point, int maxX)
        {
            var X = (int)Math.Round(point.X) + 10;
            var Y = (int)Math.Round(point.Y) + 10;
            return Y * maxX + X;
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            double size = 0.0;

            if (!DA.GetDataList(0, points)) return;
            if (!DA.GetData(1, ref size)) return;

            List<Point3d> result = new List<Point3d>();
            if (points.Count > size)
            {
                DA.SetDataList(0, result);
                return;
            }
            int maxX = 0;
            foreach (Point3d point in points)
            {
                if (point.X > maxX)
                {
                    maxX = (int) Math.Round(point.X);
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
                foreach (int stepX in new int[3]{-1, 0, 1}) {
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
            DA.SetDataList(0, result);
        }
    }
}
