using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{
    [SerializeField]
    GridManager gridManager;

    [SerializeField]
    int width;

    [SerializeField]
    int height;

    Grid grid;
    
    
    
    // Start is called before the first frame update
    void Start(){
        grid = new Grid(9, 11, new List<Obstacle>{new Obstacle(7, 9, 2, 2)});

        grid.SetTile(new DefinitionTile(0, 0, acrossWordStartsOneTileLower : true, downWordStartsOneTileRight: true));

        grid.SetTile(new DefinitionTile(2, 0, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: true));
        grid.SetTile(new DefinitionTile(4, 0, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: true));
        grid.SetTile(new DefinitionTile(6, 0, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: true));

        grid.SetTile(new DefinitionTile(0, 2, acrossWordStartsOneTileLower : true, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(0, 4, acrossWordStartsOneTileLower : true, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(0, 6, acrossWordStartsOneTileLower : true, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(0, 8, acrossWordStartsOneTileLower : true, downWordStartsOneTileRight: false));

        grid.SetTile(new DefinitionTile(8, 0, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(4, 1, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(3, 3, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(8, 3, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(2, 4, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(5, 5, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(7, 5, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(4, 6, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(2, 7, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(6, 7, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(3, 8, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(0, 10, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));
        grid.SetTile(new DefinitionTile(4, 10, acrossWordStartsOneTileLower : false, downWordStartsOneTileRight: false));



        gridManager.grid = grid;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
