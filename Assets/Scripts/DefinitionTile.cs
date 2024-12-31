

public class DefinitionTile : Tile
{
    public string rightDefinition {get; private set;}

    public string downDefinition {get; private set;}

    public bool rightWordGoingDown {get; private set;}

    public bool downWordGoingRight {get; private set;}

    public DefinitionTile(
        int x, 
        int y, 
        string rightDefinition = null, 
        string downDefinition = null, 
        bool rightWordGoingDown = false, 
        bool downWordGoingRight = false
    ) : base(x, y, isDefinition:true){
    
        this.rightDefinition = rightDefinition;
        this.downDefinition = downDefinition;
        this.rightWordGoingDown = rightWordGoingDown;
        this.downWordGoingRight = downWordGoingRight;

    
    }
}