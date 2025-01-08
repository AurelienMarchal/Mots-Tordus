public class Tile{

    public int x;

    public int y;

    public bool isObstacle { get; private set; }

    public bool isVoid { get; private set; }

    public bool isDefinition { get; private set; }


    public Tile(int x, int y, bool isObstacle = false, bool isVoid = false, bool isDefinition = false) {
        this.x = x;
        this.y = y;
        this.isObstacle = isObstacle;
        this.isVoid = isVoid;
        this.isDefinition = isDefinition;
    }


    public override string ToString(){
        return $"Tile ({x}, {y})";
    }
}