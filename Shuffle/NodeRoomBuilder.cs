using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Drawing;

namespace Shuffle
{
    public class NodeRoomBuilder : GH_Component
    {
        public NodeRoomBuilder() : base("Room Builder", "roomBuilder", "Creating a room", "Shuffle", "Room")
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
            pManager.AddNumberParameter("minSize", "minSize", "Minimum size of the room", GH_ParamAccess.item);
            pManager.AddParameter(new CellParam(), "seed", "seed", "The starting cell of the room", GH_ParamAccess.item);
            pManager.AddNumberParameter("priority", "priority", "The priority of getting the room right", GH_ParamAccess.item);
            pManager.AddTextParameter("name", "name", "The name of the room", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new RoomParameter(), "room", "room", "A room that can evolve.", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double size = 0;
            Cell cell = new Shuffle.Cell();
            double priority = 0;
            string name = "";
            List<ShuffleBlock> blocks = new List<ShuffleBlock>();
            if (!DA.GetData(0, ref size))
            {
                return;
            }
            if (!DA.GetData(1, ref cell))
            { return; }
            if (!DA.GetData(2, ref priority))
            {
                return;
            }
            if (!DA.GetData(3, ref name))
            {
                return;
            }
            // Add read of blocks;
            DA.SetData(0, new Room(size, cell, blocks, Guid.NewGuid(), priority, name));
        }
    }
}
