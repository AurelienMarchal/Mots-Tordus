

public class LetterTile : Tile
{

    public char letter{ get; private set; }




    public LetterTile(int x, int y, char letter) : base(x, y){
        this.letter = letter;
    }

    public override object Clone()
    {
        LetterTile tileClone = new(x, y, letter);
        return tileClone;
    }
}