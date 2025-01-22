

public class LetterTile : Tile
{

    public char letter{ get; private set; }




    public LetterTile(int x, int y, char letter) : base(x, y){
        this.letter = letter;
    }
}