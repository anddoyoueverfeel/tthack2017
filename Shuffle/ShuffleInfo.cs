using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Shuffle
{
    public class ShuffleInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Shuffle";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("5dcd986f-dc8c-4134-9e49-6e8a8384328c");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Handel Architects";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
