



//TO REMOVE ITS JUST A LETTER TILE WITH NO LETTER
public class VoidTile : Tile
{
    public VoidTile(int x, int y) : base(x, y, isVoid:true){
    }

    public override object Clone()
    {
        VoidTile tileClone = new(x, y);
        return tileClone;
    }
}