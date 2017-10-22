using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace Shuffle
{
    public class ShuffleBlock
    {
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


        public Point3d centerPoint {
            set; get;
        }


        


        public int priority { set; get; }

        public void setDirection(int deltaX, int deltaY)
        {

        }

    }
}
