using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    

    [SerializeField]
    GameObject tilePrefab;

    [SerializeField]
    float tileWidth;

    [SerializeField]
    float tileHeight;

    private Grid grid_;

    public Grid grid{
        get { return grid_; }
        set { grid_ = value; UpdateAccordingToGrid();}
    }

    private TileManager tileManagerHovered_;

    public TileManager tileManagerHovered{
        get { return tileManagerHovered_; }
        set { 
            ResetAllTileHightlight();
            tileManagerHovered_ = value;
            if(tileManagerHovered != null){
                if(tileManagerHovered.tile is DefinitionTile definitionTile){
                    HighlightTiles(grid.GetTilesReachedByDownDefinitionTile(definitionTile));
                    HighlightTiles(grid.GetTilesReachedByAcrossDefinitionTile(definitionTile));
                }
            }
        }
    }

    private List<TileManager> tileManagers = new List<TileManager>();
    
    
    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateAccordingToGrid(){
        
        Debug.Log("UpdateAccordingToGrid");
        
        for (int i = 0; i < grid.width; i++){
            for (int j = 0; j < grid.height; j++){
                var tile = grid.GetTileAt(i, j);

                if(tile == null){
                    continue;
                }

                var tilePrefabInstance = Instantiate(tilePrefab, transform);
                var tileManager = tilePrefabInstance.GetComponent<TileManager>();
                if(tileManager != null){

                    tileManager.tile = tile;
                    tileManagers.Add(tileManager);
                    tileManager.transform.localPosition = new Vector3(-i * tileWidth, 0, j*tileHeight);
                }

                else{
                    Destroy(tileManager);
                }
                

            }
        }
    }


    void HighlightTiles(List<Tile> tiles){
        foreach (var tileManager in tileManagers){
            if(tiles.Contains(tileManager.tile)){
                tileManager.isHighlighted = true;
            }
        }
    }

    void ResetAllTileHightlight(){
        foreach (var tileManager in tileManagers){
            tileManager.isHighlighted = false;
        }
    }
}
