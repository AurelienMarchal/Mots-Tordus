using System.Collections.Generic;



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
        GenerateTopAndRightDefinitionTiles();
    }

    void SetTile(Tile tile){
        if(tiles == null){
            return;
        }

        if(tile.x < 0 || tile.x > width - 1){
            return;
        }
        if(tile.y < 0 || tile.y > height - 1){
            return;
        }

        var index = tile.x * width + tile.y;

        if(index < 0 || index > tiles.Length - 1){
            return;
        }

        if(!tiles[index].isVoid){
            return;
        }

        tiles[index] = tile;
    }

    Tile GetTileAt(int x, int y){

        if(x < 0 || x > width - 1){
            return null;
        }
        if(y < 0 || y > height - 1){
            return null;
        }

        var index = x * width + y;

        if(index < 0 || index > tiles.Length - 1){
            return null;
        }
        return tiles[index];
    }


    void GenerateTopAndRightDefinitionTiles(){
        for (int i = 0; i < width; i += 2){
            SetTile(new DefinitionTile(i, 0));
        }

        for (int j = 0; j < height; j += 2){
            SetTile(new DefinitionTile(0, j));
        }
    }

    void GenerateObstacleTiles(){
        foreach (var obstacle in obstacles){
            for (int i = 0; i < obstacle.width; i++){
                for (int j = 0; j < obstacle.height; j++){
                    
                }
            }
        }
    }





    public bool IsValid(){
        return false;
    }
}