using _Scripts.Runtime.Enums;

namespace _Scripts.Runtime.Data.ValueObjects
{
    public class GridNode
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellType Type { get; private set; }
        public OccupantType Occupant { get; private set; }
        public bool IsWalkable => Occupant == OccupantType.Empty;

        public GridNode(int x, int y,bool isWalkable ,CellType type, OccupantType occupant)
        {
            X = x;
            Y = y;
            Type = type;
            Occupant = occupant;
        }
        
        //TODO: Calculations about grid logic & gameplay
    }
}