using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// TODO: have list for only definition tiles
public class Grid{

    private Tile[] tiles;
    public int width { get; private set; }
    public int height { get; private set; }

    private List<Obstacle> obstacles;

    public Grid(int width, int height, List<Obstacle> obstacles){
        this.width = width;
        this.height = height;

        tiles = new Tile[width * height];

        for(int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                SetTile(new VoidTile(i, j));
            }
        }
        
        if(obstacles == null){
            this.obstacles = new List<Obstacle>();
        }
        else{
            this.obstacles = obstacles;
        }

        GenerateObstacleTiles();
        //GenerateTopAndRightDefinitionTiles();
    }

    public void SetTile(Tile tile){
        if(tiles == null){
            return;
        }

        if(tile.x < 0 || tile.x > width - 1){
            return;
        }
        if(tile.y < 0 || tile.y > height - 1){
            return;
        }

        var index = tile.x * height + tile.y;

        //Debug.Log($"Putting {tile} at index {index}");

        if(index < 0 || index > tiles.Length - 1){
            return;
        }

        tiles[index] = tile;
    }

    public Tile GetTileAt(int x, int y){

        if(x < 0 || x > width - 1){
            return null;
        }
        if(y < 0 || y > height - 1){
            return null;
        }

        var index = x * height + y;

        if(index < 0 || index > tiles.Length - 1){
            return null;
        }

        //Debug.Log($"At {x}, {y} there is tile {tiles[index]}");
        return tiles[index];
    }


    void GenerateTopAndRightDefinitionTiles(){
        foreach (var tile in tiles){
            if(IsTileAtTopLeftCorner(tile)){
                SetTile(new DefinitionTile(tile.x, tile.y, acrossWordStartsOneTileLower:true, downWordStartsOneTileRight:true));
            }
        }

        /*
        for (int i = 2; i < width; i += 2){
            SetTile(new DefinitionTile(i, 0));
        }

        for (int j = 2; j < height; j += 2){
            SetTile(new DefinitionTile(0, j));
        }
        */
    }

    bool IsTileAtTopLeftCorner(Tile tile){
        var topTile = GetTileAt(tile.x, tile.y - 1);
        var leftTile = GetTileAt(tile.x - 1, tile.y);

        return (topTile == null || topTile.isObstacle) && (leftTile == null || leftTile.isObstacle);
    }

    void GenerateObstacleTiles(){
        foreach (var obstacle in obstacles){
            for (int i = 0; i < obstacle.width; i++){
                for (int j = 0; j < obstacle.height; j++){
                    SetTile(new ObstacleTile(obstacle.x + i, obstacle.y + j));
                }
            }
        }
    }

    public void UpdateTilesReachedByDefinition(){
        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                definitionTile.tilesReachedByDownDefinition = GetTilesReachedByDownDefinitionTile(definitionTile);
                definitionTile.tilesReachedByAcrossDefinition = GetTilesReachedByAcrossDefinitionTile(definitionTile);
                /*
                switch(definitionTile.definitionTileLayout){
                    case DefinitionTileLayout.DownAndAcross:
                        Debug.Log($"{definitionTile} is DownAndAcross");
                        break;

                    case DefinitionTileLayout.Down:
                        Debug.Log($"{definitionTile} is Down");
                        break;

                    case DefinitionTileLayout.Across:
                        Debug.Log($"{definitionTile} is Across");
                        break;
                    
                }
                */
            }
        }
    }

    public DefinitionTile FindDefinitionTileWithLongestDownWordEntryMissing(){
        DefinitionTile definitionTileToReturn = null;
        int maxWordLenght = 0;

        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(
                    definitionTile.finalDownWordEntryIsMissing &&
                    definitionTile.tilesReachedByDownDefinition.Count > maxWordLenght
                ){
                    definitionTileToReturn = definitionTile;
                    maxWordLenght = definitionTile.tilesReachedByDownDefinition.Count;
                }
            }
        }

        return definitionTileToReturn;
    }

    public DefinitionTile FindDefinitionTileWithLongestAcrossWordEntryMissing(){
        DefinitionTile definitionTileToReturn = null;
        int maxWordLenght = 0;

        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(
                    definitionTile.finalAcrossWordEntryIsMissing &&
                    definitionTile.tilesReachedByAcrossDefinition.Count > maxWordLenght
                ){
                    definitionTileToReturn = definitionTile;
                    maxWordLenght = definitionTile.tilesReachedByAcrossDefinition.Count;
                }
            }
        }


        return definitionTileToReturn;
    }


    public bool UpdatePossibleWordEntries(WordDictionnary wordDictionnary){

        var atLeastOneTileUpdated = false;
        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(definitionTile.finalDownWordEntryIsMissing && definitionTile.crossesAtLeastOneWordDown){
                    atLeastOneTileUpdated = true;
                    if(definitionTile.possibleDownWordEntries == null){
                        var results = wordDictionnary.SearchWord(definitionTile.downWordSearch, out int iterations);
                        Debug.Log($"Initializing {definitionTile} possibleDownWordEntries with [{definitionTile.downWordSearch}], {results.Count} found in {iterations} iterations");
                        definitionTile.InitializePossibleDownWordEntries(results);
                    }
                    else{
                        definitionTile.UpdatePossibleDownWordEntries();
                    }
                }

                if(definitionTile.finalAcrossWordEntryIsMissing && definitionTile.crossesAtLeastOneWordAcross){
                    atLeastOneTileUpdated = true;
                    if(definitionTile.possibleAcrossWordEntries == null){
                        var results = wordDictionnary.SearchWord(definitionTile.acrossWordSearch, out int iterations);
                        Debug.Log($"Initializing {definitionTile} possibleAcrossWordEntries with [{definitionTile.acrossWordSearch}], {results.Count} found in {iterations} iterations");
                        definitionTile.InitializePossibleAcrossWordEntries(results);
                    }
                    else{
                        definitionTile.UpdatePossibleAcrossWordEntries();
                    }
                }
                
            }
        }

        return atLeastOneTileUpdated;
    }
    

    public bool AtLeastOneDefinitionTileHasNoPossibleWordEntry(){
        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if( definitionTile.finalDownWordEntryIsMissing && 
                    definitionTile.possibleDownWordEntries != null && 
                    definitionTile.possibleDownWordEntries.Count == 0){
                        return true;
                }

                if( definitionTile.finalAcrossWordEntryIsMissing && 
                    definitionTile.possibleAcrossWordEntries != null && 
                    definitionTile.possibleAcrossWordEntries.Count == 0){
                        return true;
                }
            }
        }

        return false;
    }

    public DefinitionTile GetDefinitionTileWithTheLeastPossibleWordEntry(out bool isDownDefinition){
        //Loop through def tile list
        DefinitionTile definitionTileToReturn = null;
        int minPossibleWordEntry = int.MaxValue;
        isDownDefinition = true;
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if( definitionTile.finalDownWordEntryIsMissing && 
                    definitionTile.possibleDownWordEntries != null &&
                    definitionTile.possibleDownWordEntries.Count < minPossibleWordEntry){
                        minPossibleWordEntry = definitionTile.possibleDownWordEntries.Count;
                        definitionTileToReturn = definitionTile;
                        isDownDefinition = true;
                }

                if( definitionTile.finalAcrossWordEntryIsMissing && 
                    definitionTile.possibleAcrossWordEntries != null &&
                    definitionTile.possibleAcrossWordEntries.Count < minPossibleWordEntry){
                        minPossibleWordEntry = definitionTile.possibleAcrossWordEntries.Count;
                        definitionTileToReturn = definitionTile;
                        isDownDefinition = false;
                    
                }
            }
        }

        return definitionTileToReturn;
    }

    public bool SetFinalDownDefinition(DefinitionTile definitionTile, WordEntry wordEntry){
        if(definitionTile == null){
            return false;
        }

        if(!definitionTile.finalDownWordEntryIsMissing){
            return false;
        }

        if(!definitionTile.CanWordFitDown(wordEntry.wordWithoutSpecialChars)){
            return false;
        }

        definitionTile.finalDownWordEntry = wordEntry;

        for (int i = 0; i < definitionTile.tilesReachedByDownDefinition.Count; i++){
            var tile = definitionTile.tilesReachedByDownDefinition[i];
            if(tile is VoidTile voidTile){
                SetTile(new LetterTile(voidTile.x, voidTile.y, wordEntry.wordWithoutSpecialChars[i]));
            }
        }

        return true;
    }

    public bool SetFinalAcrossDefinition(DefinitionTile definitionTile, WordEntry wordEntry){
        if(definitionTile == null){
            return false;
        }

        if(!definitionTile.finalAcrossWordEntryIsMissing){
            return false;
        }

        if(!definitionTile.CanWordFitAcross(wordEntry.wordWithoutSpecialChars)){
            return false;
        }

        definitionTile.finalAcrossWordEntry = wordEntry;

        for (int i = 0; i < definitionTile.tilesReachedByAcrossDefinition.Count; i++){
            var tile = definitionTile.tilesReachedByAcrossDefinition[i];
            if(tile is VoidTile voidTile){
                SetTile(new LetterTile(voidTile.x, voidTile.y, wordEntry.wordWithoutSpecialChars[i]));
            }
        }

        return true;
    }

    void UpdateTilesReachedByDefinition(int row, int column){

    }


    bool IsValidTileForDefinition(Tile tile){

        return true;


    }

    bool IsDefinitionTilesLayoutValid(){
        return true;
    }

    public List<Tile> GetTilesReachedByAcrossDefinitionTile(DefinitionTile definitionTile){
        var tiles = new List<Tile>();

        var tileReached = GetTileAt(definitionTile.x + (!definitionTile.acrossWordStartsOneTileLower ? 1 : 0), definitionTile.y + (definitionTile.acrossWordStartsOneTileLower ? 1 : 0));

        while(tileReached != null && !tileReached.isObstacle && !tileReached.isDefinition){
            tiles.Add(tileReached);
            tileReached = GetTileAt(tileReached.x + 1, tileReached.y);
        }

        return tiles;
    }

    public List<Tile> GetTilesReachedByDownDefinitionTile(DefinitionTile definitionTile){
        var tiles = new List<Tile>();

        var tileReached = GetTileAt(definitionTile.x + (definitionTile.downWordStartsOneTileRight ? 1 : 0), definitionTile.y + (!definitionTile.downWordStartsOneTileRight ? 1 : 0));

        while(tileReached != null && !tileReached.isObstacle && !tileReached.isDefinition){
            tiles.Add(tileReached);
            tileReached = GetTileAt(tileReached.x, tileReached.y + 1);
        }

        return tiles;
    }



    public bool IsValid(){
        return false;
    }
}