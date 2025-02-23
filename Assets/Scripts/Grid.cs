using System;
using System.Collections.Generic;
using UnityEngine;


// TODO: have list for only definition tiles
public class Grid : ICloneable{

    private Tile[] tiles;
    public int width { get; private set; }
    public int height { get; private set; }

    public bool definitionTileLayoutFinished;

    private List<Obstacle> obstacles;

    public Grid(int width, int height, List<Obstacle> obstacles){
        this.width = width;
        this.height = height;

        tiles = new Tile[width * height];

        definitionTileLayoutFinished = false;

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


    public void GenerateTopAndRightDefinitionTiles(){
        foreach (var tile in tiles){
            if(tile.isVoid && IsTileAtTopLeftCorner(tile) && IsValidTileForDefinition(tile)){
                SetTile(new DefinitionTile(tile.x, tile.y, firstWordGoesDown:true, secondWordGoesAcross:true));
            }
        }

        
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                var currentTile = GetTileAt(i, j);
                if(currentTile == null){
                    continue;
                }
                if(currentTile.isVoid){
                    if(IsTileAtTheTop(currentTile) && IsValidTileForDefinition(currentTile)){
                        SetTile(new DefinitionTile(i, j, firstWordGoesDown:true));
                    }
                    if(IsTileOnTheLeft(currentTile) && IsValidTileForDefinition(currentTile)){
                        SetTile(new DefinitionTile(i, j, secondWordGoesAcross:true));
                    }
                }
            }
        }
    }

    bool IsValidTileForDefinition(Tile tile){
        var topTile = GetTileAt(tile.x, tile.y - 1);
        var leftTile = GetTileAt(tile.x - 1, tile.y);
        if(leftTile != null && IsTileAtTheTop(tile) && leftTile.isDefinition){
            return false;
        }
        if(topTile != null & IsTileOnTheLeft(tile) && topTile.isDefinition){
            return false;
        }
        return true;


    }

    bool IsDefinitionTilesLayoutValid(){

        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(definitionTile.tilesReachedByFirstDefinition != null && definitionTile.tilesReachedByFirstDefinition.Count < 2){
                    return false;
                }
                if(definitionTile.tilesReachedBySecondDefinition != null && definitionTile.tilesReachedBySecondDefinition.Count < 2){
                    return false;
                }
            }
        }

        return true;
    }

    public void GetDefinitionTileWithMostTilesReachedByDefinition(out DefinitionTile definitionTile, out bool isFirstDefinition){

        var maxTilesReachedByDef = 0;
        isFirstDefinition = true;
        definitionTile = null;

        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile_){
                if(definitionTile_.tilesReachedByFirstDefinition != null){
                    if(definitionTile_.tilesReachedByFirstDefinition.Count > maxTilesReachedByDef){
                        definitionTile = definitionTile_;
                        isFirstDefinition = true;
                        maxTilesReachedByDef = definitionTile_.tilesReachedByFirstDefinition.Count;
                    }
                }
                if(definitionTile_.tilesReachedBySecondDefinition != null){
                    if(definitionTile_.tilesReachedBySecondDefinition.Count > maxTilesReachedByDef){
                        definitionTile = definitionTile_;
                        isFirstDefinition = false;
                        maxTilesReachedByDef = definitionTile_.tilesReachedBySecondDefinition.Count;
                    }
                }
            }
        }
    }



    bool IsTileAtTopLeftCorner(Tile tile){
        var topTile = GetTileAt(tile.x, tile.y - 1);
        var leftTile = GetTileAt(tile.x - 1, tile.y);

        return (topTile == null || topTile.isObstacle) && (leftTile == null || leftTile.isObstacle);
    }

    bool IsTileAtTheTop(Tile tile){
        var topTile = GetTileAt(tile.x, tile.y - 1);
        return topTile == null || topTile.isObstacle;
    }

    bool IsTileOnTheLeft(Tile tile){
        var leftTile = GetTileAt(tile.x - 1, tile.y);
        return leftTile == null || leftTile.isObstacle;
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

    public bool IsGenerationFinished(){
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if (definitionTile.finalFirstWordEntryIsMissing || definitionTile.finalSecondWordEntryIsMissing){
                    return false;
                }
            }
        }
        return true;
    }


    public void UpdateTilesReachedByDefinition(){
        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                definitionTile.tilesReachedByFirstDefinition = GetTilesReachedByFirstDefinition(definitionTile);
                definitionTile.tilesReachedBySecondDefinition = GetTilesReachedBySecondDefinition(definitionTile);
            }
        }
    }

    public void UpdateWordSearches(){
        //Loop through def tile list
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                definitionTile.UpdateWordSearch();
            }
        }
    }

    public DefinitionTile FindDefinitionTileWithLongestFirstWordEntryMissing(){
        DefinitionTile definitionTileToReturn = null;
        int maxWordLenght = 0;

        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(
                    definitionTile.finalFirstWordEntryIsMissing &&
                    definitionTile.tilesReachedByFirstDefinition.Count > maxWordLenght
                ){
                    definitionTileToReturn = definitionTile;
                    maxWordLenght = definitionTile.tilesReachedByFirstDefinition.Count;
                }
            }
        }

        return definitionTileToReturn;
    }

    public DefinitionTile FindDefinitionTileWithLongestSecondWordEntryMissing(){
        DefinitionTile definitionTileToReturn = null;
        int maxWordLenght = 0;

        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if(
                    definitionTile.finalSecondWordEntryIsMissing &&
                    definitionTile.tilesReachedBySecondDefinition.Count > maxWordLenght
                ){
                    definitionTileToReturn = definitionTile;
                    maxWordLenght = definitionTile.tilesReachedBySecondDefinition.Count;
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
                if(definitionTile.finalFirstWordEntryIsMissing && definitionTile.crossesAtLeastOneWordFirst){
                    atLeastOneTileUpdated = true;
                    if(definitionTile.possibleFirstWordEntries == null){
                        var results = wordDictionnary.SearchWord(definitionTile.firstWordSearch, out int iterations);
                        Debug.Log($"Initializing {definitionTile} possibleFirstWordEntries with [{definitionTile.firstWordSearch}], {results.Count} found in {iterations} iterations");
                        definitionTile.InitializePossibleFirstWordEntries(results);
                    }
                    else{
                        definitionTile.UpdatePossibleFirstWordEntries();
                    }
                }

                if(definitionTile.finalSecondWordEntryIsMissing && definitionTile.crossesAtLeastOneWordSecond){
                    atLeastOneTileUpdated = true;
                    if(definitionTile.possibleSecondWordEntries == null){
                        var results = wordDictionnary.SearchWord(definitionTile.secondWordSearch, out int iterations);
                        Debug.Log($"Initializing {definitionTile} possibleSecondWordEntries with [{definitionTile.secondWordSearch}], {results.Count} found in {iterations} iterations");
                        definitionTile.InitializePossibleSecondWordEntries(results);
                    }
                    else{
                        definitionTile.UpdatePossibleSecondWordEntries();
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
                if( definitionTile.finalFirstWordEntryIsMissing && 
                    definitionTile.possibleFirstWordEntries != null && 
                    definitionTile.possibleFirstWordEntries.Count == 0){
                        return true;
                }

                if( definitionTile.finalSecondWordEntryIsMissing && 
                    definitionTile.possibleSecondWordEntries != null && 
                    definitionTile.possibleSecondWordEntries.Count == 0){
                        return true;
                }
            }
        }

        return false;
    }

    public DefinitionTile GetDefinitionTileWithTheLeastPossibleWordEntry(out bool isFirstDefinition){
        //Loop through def tile list
        DefinitionTile definitionTileToReturn = null;
        int minPossibleWordEntry = int.MaxValue;
        isFirstDefinition = true;
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if( definitionTile.finalFirstWordEntryIsMissing && 
                    definitionTile.possibleFirstWordEntries != null &&
                    definitionTile.possibleFirstWordEntries.Count < minPossibleWordEntry){
                        minPossibleWordEntry = definitionTile.possibleFirstWordEntries.Count;
                        definitionTileToReturn = definitionTile;
                        isFirstDefinition = true;
                }

                if( definitionTile.finalSecondWordEntryIsMissing && 
                    definitionTile.possibleSecondWordEntries != null &&
                    definitionTile.possibleSecondWordEntries.Count < minPossibleWordEntry){
                        minPossibleWordEntry = definitionTile.possibleSecondWordEntries.Count;
                        definitionTileToReturn = definitionTile;
                        isFirstDefinition = false;
                    
                }
            }
        }

        return definitionTileToReturn;
    }

    public bool IsThereADefinitionTileWith0PossibleWordEntry(out bool isFirstDefinition, out DefinitionTile definitionTileFound){
        //Loop through def tile list
        definitionTileFound = null;
        isFirstDefinition = true;
        foreach (var tile in tiles){
            if(tile is DefinitionTile definitionTile){
                if( definitionTile.finalFirstWordEntryIsMissing && 
                    definitionTile.possibleFirstWordEntries != null &&
                    definitionTile.possibleFirstWordEntries.Count == 0){
                        definitionTileFound = definitionTile;
                        isFirstDefinition = true;
                        return true;
                }

                if( definitionTile.finalSecondWordEntryIsMissing && 
                    definitionTile.possibleSecondWordEntries != null &&
                    definitionTile.possibleSecondWordEntries.Count == 0){
                        definitionTileFound = definitionTile;
                        isFirstDefinition = false;
                        return true;
                    
                }
            }
        }

        return false;
    }

    public bool SetFinalFirstDefinition(DefinitionTile definitionTile, WordEntry wordEntry){
        if(definitionTile == null){
            return false;
        }

        if(!definitionTile.finalFirstWordEntryIsMissing){
            return false;
        }

        if(!definitionTile.CanWordFitInFirstWordSearch(wordEntry.wordWithoutSpecialChars)){
            return false;
        }

        definitionTile.finalFirstWordEntry = wordEntry;

        for (int i = 0; i < definitionTile.tilesReachedByFirstDefinition.Count; i++){
            var tile = definitionTile.tilesReachedByFirstDefinition[i];
            if(tile is VoidTile voidTile){
                SetTile(new LetterTile(voidTile.x, voidTile.y, wordEntry.wordWithoutSpecialChars[i]));
            }
        }

        return true;
    }
    

    public bool SetFinalSecondDefinition(DefinitionTile definitionTile, WordEntry wordEntry){
        if(definitionTile == null){
            return false;
        }

        if(!definitionTile.finalSecondWordEntryIsMissing){
            return false;
        }

        if(!definitionTile.CanWordFitInSecondWordSearch(wordEntry.wordWithoutSpecialChars)){
            return false;
        }

        definitionTile.finalSecondWordEntry = wordEntry;

        for (int i = 0; i < definitionTile.tilesReachedBySecondDefinition.Count; i++){
            var tile = definitionTile.tilesReachedBySecondDefinition[i];
            if(tile is VoidTile voidTile){
                SetTile(new LetterTile(voidTile.x, voidTile.y, wordEntry.wordWithoutSpecialChars[i]));
            }
        }

        return true;
    }

    void UpdateTilesReachedByDefinition(int row, int column){

    }


    

    public List<Tile> GetTilesReachedByFirstDefinition(DefinitionTile definitionTile){
        

        var tiles = new List<Tile>();

        var dX = definitionTile.firstWordGoesDown ? 0 : 1;
        var dY = definitionTile.firstWordGoesDown ? 1 : 0;

        var tileReached = GetTileAt(definitionTile.x + 1, definitionTile.y);

        while(tileReached != null && !tileReached.isObstacle && !tileReached.isDefinition){
            tiles.Add(tileReached);
            tileReached = GetTileAt(tileReached.x + dX, tileReached.y + dY);
        }

        return tiles;
    }

    public List<Tile> GetTilesReachedBySecondDefinition(DefinitionTile definitionTile){
        var tiles = new List<Tile>();

        var dX = definitionTile.secondWordGoesAcross ? 1 : 0;
        var dY = definitionTile.secondWordGoesAcross ? 0 : 1;

        var tileReached = GetTileAt(definitionTile.x, definitionTile.y + 1);
        
        while(tileReached != null && !tileReached.isObstacle && !tileReached.isDefinition){
            tiles.Add(tileReached);
            tileReached = GetTileAt(tileReached.x + dX, tileReached.y + dY);
        }

        return tiles;
    }



    public bool IsValid(){
        return false;
    }

    public object Clone(){
        Grid gridClone = new(width, height, obstacles);
        gridClone.definitionTileLayoutFinished = definitionTileLayoutFinished;
        foreach (var tile in tiles){
            gridClone.SetTile((Tile)tile.Clone());
        }
        
        gridClone.UpdateTilesReachedByDefinition();

        return gridClone;
    }
}