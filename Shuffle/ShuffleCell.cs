using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    public class Cell
    {
        
        
        //id of room that "owns" this cell
        public int m_owner;
        //first int is what room "wants" this cell
        //second int is how "badly" it wants it
        public Dictionary<int, int> desires;

        public Cell()
        {
            m_owner = 0;
        }


    }


    public class Room
    {
        private int m_id;

        public Room(int id)
        {
            m_id = id;
        }

        public int id {
            get { return m_id; }
        }

        public Dictionary<string, int> step() {
            Dictionary<string, int> myDict = new Dictionary<string, int>();

            myDict.Add("10", 15);
            myDict.Add("5", 10);

            return myDict;
        }

    }
}
