using Rhino.Geometry;
using Rhino.Display;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace Occamy
{


    public class BasePanel
    {

        #region methods that must be implemented for all panel types
        public virtual Point3d findPoint(ContextualPointSpec Pointspec) { return new Point3d(0, 0, 0); }
        public virtual Point3d fromBottomLeft(bool useRelativeUnitsTB, double distTB, bool useRelativeUnitsLR, double distLR) { return new Point3d(0, 0, 0); }
        public virtual Point3d fromBottomRight(bool useRelativeUnitsTB, double distTB, bool useRelativeUnitsLR, double distLR) { return new Point3d(0, 0, 0); }
        public virtual Point3d fromTopLeft(bool useRelativeUnitsTB, double distTB, bool useRelativeUnitsLR, double distLR) { return new Point3d(0, 0, 0); }
        public virtual Point3d fromTopRight(bool useRelativeUnitsTB, double distTB, bool useRelativeUnitsLR, double distLR) { return new Point3d(0, 0, 0); }
        public virtual Surface panelContextAsSurface() { return null; }
        public virtual BasePanel AddProperties(Dictionary<string, string> propertiesToAdd) { return null; }
        public virtual BasePanel AddComponents(List<PanelComponent> componentsToAdd) { return null; }
        #endregion


        #region properties
        public virtual List<PanelComponent> panelComponents { get; }
        public virtual Dictionary<string, string> properties { get; }
        public virtual bool IsValid { get; }
        public virtual Dictionary<Point3d, DisplayMaterial> preview_pointsToDraw { get; }
        public virtual Dictionary<Line, DisplayMaterial> preview_arrowsToDraw { get; }
        //public virtual Dictionary<Curve, DisplayMaterial> preview_wiresToDraw { get; }
        //public virtual Dictionary<Mesh, DisplayMaterial> preview_meshesToDraw { get; }
        #endregion


        #region Constructors
        public virtual BasePanel Duplicate() { return null; }
        #endregion
    }


    public class BasePanelGoo : GH_Goo<BasePanel>, IGH_PreviewData
    {

        public BasePanelGoo()
        {
            this.Value = new BasePanel();
        }

        public BasePanelGoo(BasePanel basePanel)
        {
            if (basePanel == null)
            {
                this.Value = new BasePanel();
            }
            this.Value = basePanel;
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateBasePanel();
        }

        public BasePanelGoo DuplicateBasePanel()
        {

            BasePanel foobar = new BasePanel();

            if (Value == null)
            {
                foobar = new BasePanel();
            }

            else
            {
                foobar = Value.Duplicate();
            }

            return new BasePanelGoo(foobar);

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
                if (Value == null) { return "No internal Panel instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Panel instance"; //Todo: beef this up to be more informative.
            }
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null type of panel that you shouldnt be seeing anyway... Base class";
            else
                return Value.ToString();
        }

        public override string TypeName
        {
            get { return ("Base Panel"); }
        }

        public override string TypeDescription
        {
            get { return ("Defines a single panel, but this is the base class"); }
        }
        #endregion


        public override bool CastTo<Q>(ref Q target)
        {

            if (typeof(Q).IsAssignableFrom(typeof(BasePanel)))
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
            if (typeof(BasePanel).IsAssignableFrom(source.GetType()))
            {
                Value = (BasePanel)source;
                return true;
            }

            return false;
        }

        //turning off IGH_PreviewData means that no custom stuff will be drawn in the viewport
        //it is not possible to define custom geometry in the "Parameter" class
        #region drawing methods
        public BoundingBox Boundingbox
        {
            get
            {
                return BoundingBox.Empty;
            }
        }

        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        public virtual void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            //this view will also implement all of the things in DrawViewportWires
        }

        public virtual void DrawViewportWires(GH_PreviewWireArgs args)
        {
            //if (Value == null) { return; }
            return;
        }

        #endregion

    }


    public class BasePanelParamter : GH_Param<BasePanelGoo>, IGH_PreviewObject
    {
        public BasePanelParamter()
          : base(new GH_InstanceDescription("Panels!", "Base class for panels", "Maintains a collection of panels", "Occamy", "Panels"))
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




        //REPLACE THIS WITH A NEW GUID
        public override Guid ComponentGuid
        {
            get { return new Guid("3965e2e6-10fa-4532-b06f-95c75da0ebe7"); }
        }






        //This stuff seems to effect whether a preview is generated for the make from 2 points component
        //but the preview still generates if you pass it through a "data" component...

        #region preview methods
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }


        //Controls what gets drawn when meshes are previewed
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            //Meshes aren't drawn.
            if (args.Document.PreviewMode == GH_PreviewMode.Shaded && args.Display.SupportsShading)
            {
                //Preview_DrawMeshes(args);
            }
        }

        //Controls what is drawn when meshes are turned off. These items ALSO get drawn when mesh previews are on!!
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            //Use a standard method to draw gunk, you don't have to specifically implement this.
            //This function iterates over all items in the Volatile data tree and calls DrawViewportWires 
            //on each item that implements Preview.IGH_PreviewData. 

            //Interesting fact: this Preview_DrawWires is only triggered within parameters of my making..
            //for example, a generic "data" type component in GH will still preview without this function,
            //because it calls Preview_DrawWires within itself 
            Preview_DrawWires(args);


            //Turning on mesh previews here would basically ignore the user's ability to turn mesh previews off for this component
            //(so dont do that)
            //Preview_DrawMeshes(args);
        }

        private bool m_hidden = false;

        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion

    }


}
