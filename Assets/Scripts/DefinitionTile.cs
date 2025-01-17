

using System.Collections.Generic;

public enum DefinitionTileLayout{
    Across, Down, DownAndAcross
}


public class DefinitionTile : Tile
{

    public DefinitionTileLayout definitionTileLayout {get; private set;}

    public string acrossWord {get; private set;}

    public string downWord {get; private set;}
    public string acrossDefinition {get; private set;}

    public string downDefinition {get; private set;}

    public bool acrossWordStartsOneTileLower {get; private set;}

    public bool downWordStartsOneTileRight {get; private set;}

    private List<Tile> tilesReachedByDownDefinition_;
    public List<Tile> tilesReachedByDownDefinition {
        get{
            return tilesReachedByDownDefinition_;
        } 
        set{
            tilesReachedByDownDefinition_ = value;
            if(value != null){
                if(tilesReachedByAcrossDefinition != null && tilesReachedByAcrossDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.DownAndAcross;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.Down;
                }
            }
        }
    }


    private List<Tile> tilesReachedByAcrossDefinition_;

    public List<Tile> tilesReachedByAcrossDefinition {
        get {
            return tilesReachedByAcrossDefinition_;
        }
        set {
            tilesReachedByAcrossDefinition_ = value;
            if(value != null){
                if(tilesReachedByDownDefinition != null && tilesReachedByDownDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.DownAndAcross;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.Across;
                }
            }
        }
    }

    public DefinitionTile(
        int x, 
        int y,
        DefinitionTileLayout definitionTileLayout = DefinitionTileLayout.DownAndAcross,
        string acrossWord = null,
        string downWord = null,
        string acrossDefinition = null, 
        string downDefinition = null, 
        bool acrossWordStartsOneTileLower = false, 
        bool downWordStartsOneTileRight = false

    ) : base(x, y, isDefinition:true){
        
        this.definitionTileLayout = definitionTileLayout;
        this.acrossWord = acrossWord;
        this.downWord = downWord;
        this.acrossDefinition = acrossDefinition;
        this.downDefinition = downDefinition;
        this.acrossWordStartsOneTileLower = acrossWordStartsOneTileLower;
        this.downWordStartsOneTileRight = downWordStartsOneTileRight;
        tilesReachedByAcrossDefinition = new List<Tile>();
        tilesReachedByDownDefinition = new List<Tile>();

    
    }
}


