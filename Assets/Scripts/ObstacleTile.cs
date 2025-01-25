


public class ObstacleTile : Tile{
    public ObstacleTile(int x, int y) : base(x, y, isObstacle:true){
    
    }

    public override object Clone()
    {
        ObstacleTile tileClone = new(x, y);
        return tileClone;
    }
}