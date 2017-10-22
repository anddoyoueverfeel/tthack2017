using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Shuffle
{
    public class NodeMasterSolver : GH_Component
    {
        //cell xy, Cell class 
        private Dictionary<string, Cell> simulationField = new Dictionary<string, Cell>();
        //room id, room class
        private Dictionary<Guid, Room> simulationRooms = new Dictionary<Guid, Room>();
        private int stepCount = 0;

        public NodeMasterSolver()
          : base("ShuffleSolver", "Solver",
              "Description",
              "Shuffle", "Solver")
        {
        }


        //structure for holding a grid matrix
        //cell 
        //has owner for current iteration (can be null)
        //has owner for next iteration (can be null)
        //has location (point3d, never changes)
        //


        //during a solve
        //check to see if "run" is true (if it is, continue, otherwise, quit)
        //check to see if "reset" is true (if it is, clear the persistent variables)
        //check to see if persistent variables have been set for the room info, cell info, etc.

        //if they haven't:
        //read the inputs and set the variables, then continue to the next part

        //read the persistent variable containing cell information (copy it into an iteration-specific variable)

        //foreach room
        //tell a room what its current cells are, get back values for cell priorities (eg every cell and its priority to become/ remain part of that object
        //store this data in a structure where each cell can log an arbitrary number of "priorities"
        //Once every room has been evaluated, go cell by cell and determine what that cell will be for the next iteration (save this in a property)
        //Go through and actually change the values of the cells... (merge with previous step?)




        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new CellParam(), "Cells", "Cells", "All possible cells", GH_ParamAccess.list);
            //pManager.AddTextParameter("Room", "Room", "PLACEHOLDER", GH_ParamAccess.list);
            pManager.AddParameter(new RoomParameter(), "Rooms", "R", "Rooms to solve for", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Run", "Run", "Connect switch and change to 'true' to run the simulation", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reset", "Reset", "Connect button and change to 'true' to restart the simulation (required, if you want changes to other inputs to take effect)", GH_ParamAccess.item, false);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("room1", "room1", "description", GH_ParamAccess.list);
            pManager.AddPointParameter("room2", "room2", "description", GH_ParamAccess.list);
            pManager.AddNumberParameter("stepCount", "stepCount", "adf", GH_ParamAccess.item);
        }

        private void reset()
        {
            simulationField.Clear();
            simulationRooms.Clear();
            stepCount = 0;
        }

        private void initialize(List<Cell> cells, List<Room> rooms)
        {
            //set playingfield from input points
            foreach (Cell cell in cells)
            {
                simulationField.Add(cell.ToString(), new Cell(cell.X, cell.Y, cell.m_owner, cell.desires, cell.point));
            }

            //set rooms from input (rooms are read once and persisted)
            foreach (Room room in rooms)
            {
                simulationRooms.Add(room.id, room);
                simulationField[room.cell.ToString()].m_owner = room.id;
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Cell> cellsToProcess = new List<Cell>();
            //room
            List<Room> roomsToProcess = new List<Room>();
            //List<string> rooms = new List<string>();
            bool runSimulationFlag = false;
            bool resetSimulation = false;
            Dictionary<Guid, List<Cell>> simulationCellList = new Dictionary<Guid, List<Cell>>();

            if (!DA.GetDataList(0, cellsToProcess)) return;
            if (!DA.GetDataList(1, roomsToProcess)) return;
            if (!DA.GetData(2, ref runSimulationFlag)) return;
            if (!DA.GetData(3, ref resetSimulation)) return;

            if (resetSimulation)
            {
                reset();
                initialize(cellsToProcess, roomsToProcess);
                return;
            }

            //if reset button gets pushed, clear the dictionary of the "gameboard"

            //todo: ensure that only one bound exists (for performance reasons...)
            if (!runSimulationFlag)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Simulation is paused... Run variable must be set to true");
                return;
            }

            Debug.Print("------------------------" + stepCount + "------------------");
            
            foreach (Cell cell in simulationField.Values)
            {
                if (cell.m_owner.Equals(Guid.Empty))
                {
                    continue;
                }
                if (simulationCellList.ContainsKey(cell.m_owner))
                {
                    simulationCellList[cell.m_owner].Add(cell);
                }
                else
                {
                    List<Cell> list = new List<Cell>();
                    list.Add(cell);
                    simulationCellList.Add(cell.m_owner, list);
                }
            }
            foreach (Room simulationRoom in simulationRooms.Values)
            {
                if (!simulationCellList.ContainsKey(simulationRoom.id))
                {
                    continue;
                }
                Dictionary<string, int> desiresFromRoom = simulationRoom.step(simulationCellList[simulationRoom.id]);

                foreach (KeyValuePair<string, int> desireFromRoom in desiresFromRoom)
                {
                    // do something with entry.Value or entry.Key
                    //update the field cells to include the desires of the room
                    //to do: refactor this to make it more clear what is going on
                    if (simulationField.ContainsKey(desireFromRoom.Key))
                    {
                        simulationField[desireFromRoom.Key].desires.Add(simulationRoom.id, desireFromRoom.Value);
                        //Debug.Print("desireFromRoom: " + desireFromRoom.Key  + " " + simulationRoom.id + " " + desireFromRoom.Value + "\n");
                    }
                }

            }
            //evaluate each cell in the simulationfield to determine what action should be taken on it
            foreach (Cell simulationCell in simulationField.Values)
            {
                //owner

                //property Dict<int,int> desires
                //first int is what room "wants" this cell
                //second int is how "badly" it wants it

                // results.MaxBy(kvp => kvp.Value).Key;
                if (simulationCell.desires.Count > 0)
                {
                    int maxPriority = -1 ;
                    Guid pickedRoom = Guid.Empty;
                    foreach (KeyValuePair<Guid, int> pair in simulationCell.desires)
                    {
                        if (pair.Value > maxPriority)
                        {
                            maxPriority = pair.Value;
                            pickedRoom = pair.Key;
                        }
                    }
                    simulationCell.m_owner = pickedRoom;
                    simulationCell.desires.Clear();
                }
            }
            
            Dictionary<Guid, List<Point3d>> outputPoints = new Dictionary<Guid, List<Point3d>>();
            simulationCellList.Clear();
            foreach (Cell cell in simulationField.Values)
            {
                if (cell.m_owner.Equals(Guid.Empty))
                {
                    continue;
                }
                if (outputPoints.ContainsKey(cell.m_owner))
                {
                    outputPoints[cell.m_owner].Add(cell.point);
                }
                else
                {
                    List<Point3d> list = new List<Point3d>();
                    list.Add(cell.point);
                    outputPoints.Add(cell.m_owner, list);
                }
            }

            int i = 0;
            foreach (KeyValuePair<Guid, List<Point3d>> pair in outputPoints) { 
                DA.SetDataList(i, pair.Value);
                i++;
            }
            DA.SetData(2, stepCount);

            stepCount++;
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
            get { return new Guid("25c0536d-e4c0-4351-b046-5a0870a9472d"); }
        }
    }
}