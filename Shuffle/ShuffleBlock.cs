using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Diagnostics;

namespace Shuffle
{
    public class ShuffleBlock
    {
        //fields
        private Point3d m_centerPoint;
        private Dictionary<string, int> m_cornerOffsets;
        private int m_rotationDegrees; //for now this is only intervals of 90
        private int m_priority;
        private List<Curve> m_geomOfObject;
        private List<Curve> m_gGeomOfClearance;
        private int deltaX = 0;
        private int deltaY = 0;

        //constructors
        public ShuffleBlock()
        {

        }

        public ShuffleBlock(Point3d centerPoint, Dictionary<string, int> cornerOffsets, int priority, List<Curve> GeomOfObject, List<Curve> GeomOfClearance)
        {
            this.m_centerPoint = centerPoint;
            this.m_cornerOffsets = cornerOffsets;
            this.m_priority = priority;
            this.m_geomOfObject = GeomOfObject;
            this.m_gGeomOfClearance = GeomOfClearance;
        }

        public ShuffleBlock(ShuffleBlock oldShuffleBlock)
        {
            //all properties get set the same as in the oldShuffleBlock
            this.m_centerPoint = oldShuffleBlock.centerPoint;
        }


        public virtual ShuffleBlock Duplicate()
        {
            return new ShuffleBlock(this);
        }


        //properties
        public Point3d centerPoint { get { return m_centerPoint; } }
        public int priority { set; get; }
        public virtual bool IsValid { get; }


        //methods
        public Dictionary<string, Cell> GetBoundingBox()
        {
            Dictionary<string, Cell> temp = new Dictionary<string, Cell>();
            temp.Add("min", new Cell());
            temp.Add("max", new Cell());
            return temp;
        }


        public Dictionary<string, Cell> GetObjectBoundingBox()
        {
            Dictionary<string, Cell> temp = new Dictionary<string, Cell>();
            temp.Add("min", new Cell());
            temp.Add("max", new Cell());
            return temp;
        }

        //to do: does this actually do the moving?
        public void setDirection(int deltaX, int deltaY)
        {
            this.deltaX = deltaX;
            this.deltaY = deltaY;
        }

        public void Move()
        {
            var xf = Rhino.Geometry.Transform.Translation(deltaX, deltaY, 0);
            m_centerPoint.Transform(xf);

            Debug.Print("Move deltaX: " + deltaX + " deltaY: " + deltaY);
            foreach (Curve thing in m_geomOfObject)
            {
                thing.Transform(xf);
                Debug.Print("Move curve: " + deltaX + " deltaY: " + deltaY);
            }
            foreach (Curve thing in m_gGeomOfClearance)
            {
                thing.Transform(xf);
                Debug.Print("Move thing: " + deltaX + " deltaY: " + deltaY);
            }
        }


    }


    public class ShuffleBlockGoo : GH_Goo<ShuffleBlock>
    {

        public ShuffleBlockGoo()
        {
            this.Value = new ShuffleBlock();
        }

        public ShuffleBlockGoo(ShuffleBlock shuffleBlock)
        {
            if (shuffleBlock == null)
            {
                this.Value = new ShuffleBlock();
            }
            this.Value = shuffleBlock;
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateShuffleBlock();
        }

        public ShuffleBlockGoo DuplicateShuffleBlock()
        {

            ShuffleBlock foobar = new ShuffleBlock();

            if (Value == null)
            {
                foobar = new ShuffleBlock();
            }

            else
            {
                foobar = Value.Duplicate();
            }

            return new ShuffleBlockGoo(foobar);

        }

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Shuffle block instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Shuffle Block instance"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "A invalid, null, or empty shuffle block... what do you want to know?";
            else
                return Value.ToString();
        }

        public override string TypeName
        {
            get { return ("Shuffle Block"); }
        }

        public override string TypeDescription
        {
            get { return ("An instance of a shuffle block"); }
        }
        #endregion


        public override bool CastTo<Q>(ref Q target)
        {

            if (typeof(Q).IsAssignableFrom(typeof(ShuffleBlock)))
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
            if (typeof(ShuffleBlock).IsAssignableFrom(source.GetType()))
            {
                Value = (ShuffleBlock)source;
                return true;
            }

            return false;
        }
    }


    public class ShuffleBlockParam : GH_Param<ShuffleBlockGoo>
    {
        public ShuffleBlockParam()
          : base(new GH_InstanceDescription("Shuffle blocks!", "These things live in rooms", "Maintains a collection of shuffle blocks", "Shuffle", "Params"))
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null; //Todo, provide an icon.
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                // If you want to provide this parameter on the toolbars, use something other than hidden.
                return GH_Exposure.hidden;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("588F6617-3134-4F5A-8E72-6B8D8F42E75C");
            }
        }






    }



}
