using _Scripts.Runtime.Enums;

namespace _Scripts.Runtime.Data.ValueObjects
{
    public class GridNode
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellType Type { get; private set; }
        public OccupantType Occupant { get; private set; }
        public bool IsWalkable => Type != CellType.Obstructed && Occupant == OccupantType.Empty;

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public GridNode CameFromNode { get; set; }


        public GridNode(int x, int y, CellType type, OccupantType occupant)
        {
            X = x;
            Y = y;
            Type = type;
            Occupant = occupant;
        }

        public void SetOccupant(OccupantType newOccupant)
        {
            Occupant = newOccupant;
            
        }
    }
}