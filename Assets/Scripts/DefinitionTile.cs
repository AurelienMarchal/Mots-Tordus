

using System.Collections.Generic;

public class DefinitionTile : Tile
{
    public string acrossDefinition {get; private set;}

    public string downDefinition {get; private set;}

    public bool acrossWordStartsOneTileLower {get; private set;}

    public bool downWordStartsOneTileRight {get; private set;}

    public List<Tile> tilesReachedByDownDefinition;

    public List<Tile> tilesReachedByAcrossDefinition;

    public DefinitionTile(
        int x, 
        int y, 
        string acrossDefinition = null, 
        string downDefinition = null, 
        bool acrossWordStartsOneTileLower = false, 
        bool downWordStartsOneTileRight = false

    ) : base(x, y, isDefinition:true){
    
        this.acrossDefinition = acrossDefinition;
        this.downDefinition = downDefinition;
        this.acrossWordStartsOneTileLower = acrossWordStartsOneTileLower;
        this.downWordStartsOneTileRight = downWordStartsOneTileRight;
        tilesReachedByAcrossDefinition = new List<Tile>();
        tilesReachedByDownDefinition = new List<Tile>();

    
    }
}