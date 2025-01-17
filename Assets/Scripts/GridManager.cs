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
                    switch(definitionTile.definitionTileLayout){
                        case DefinitionTileLayout.Across:
                            HighlightTiles(definitionTile.tilesReachedByAcrossDefinition);
                            break;

                        case DefinitionTileLayout.Down:
                            HighlightTiles(definitionTile.tilesReachedByDownDefinition);
                            break;

                        case DefinitionTileLayout.DownAndAcross:
                            HighlightTiles(definitionTile.tilesReachedByAcrossDefinition);
                            HighlightTiles(definitionTile.tilesReachedByDownDefinition);
                            break;

                        default: break;

                    }
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

    // TODO : Optimize by organizing tileManagers by tileCoords
    TileManager FindTileManageryTile(Tile tile){
        
        foreach (var tileManager in tileManagers){
            if(tileManager.tile == tile){
                return tileManager;
            }
        }

        return null;
    }

    void UpdateAccordingToGrid(){
        
        //Debug.Log("UpdateAccordingToGrid");
        
        for (int i = 0; i < grid.width; i++){
            for (int j = 0; j < grid.height; j++){
                var tile = grid.GetTileAt(i, j);

                if(tile == null){
                    continue;
                }

                TileManager tileManager = FindTileManageryTile(tile);

                if(tileManager == null){
                    var tilePrefabInstance = Instantiate(tilePrefab, transform);
                    tileManager = tilePrefabInstance.GetComponent<TileManager>();
                    tileManagers.Add(tileManager);
                }

                tileManager.transform.localPosition = new Vector3(-i * tileWidth, 0, j*tileHeight);
                tileManager.tile = tile;
                
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
