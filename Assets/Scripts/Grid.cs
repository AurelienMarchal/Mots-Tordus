using System.Collections.Generic;
using UnityEngine;



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


    
    void UpdateTilesReachedByDefinition(int row, int column){

    }


    bool IsValidTileForDefinition(Tile tile){

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