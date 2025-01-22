using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{
    [SerializeField]
    GridManager gridManager;

    [SerializeField]
    WordDictionnaryManager wordDictionnaryManager;

    [SerializeField]
    int width;

    [SerializeField]
    int height;

    Grid grid;

    [SerializeField]
    int seed;

    // Start is called before the first frame update
    void Start(){

        Random.InitState(seed);

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

        grid.UpdateTilesReachedByDefinition();

        SetFirstWord(grid);

        grid.UpdateTilesReachedByDefinition();

        gridManager.grid = grid;
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            NextGenerationIteration(grid);
            gridManager.grid = grid;
        }
    }

    bool SetFirstWord(Grid grid){
        DefinitionTile definitionTile = grid.FindDefinitionTileWithLongestDownWordEntryMissing();
        if(definitionTile == null){
            return false;
        }

        if(!wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght.ContainsKey(definitionTile.downWordSearch.Length)){
            return false;
        }

        var randomIndex = Random.Range(0, wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.downWordSearch.Length].Count);
        var wordEntry = wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.downWordSearch.Length][randomIndex];
        return grid.SetFinalDownDefinition(definitionTile, wordEntry);

    }

    bool NextGenerationIteration(Grid grid){
        grid.UpdateTilesReachedByDefinition();
        
        bool didUpdate = grid.UpdatePossibleWordEntries(wordDictionnaryManager.wordDictionnary);

        if(!didUpdate){
            return false;
        }

        DefinitionTile definitionTile = grid.GetDefinitionTileWithTheLeastPossibleWordEntry(out bool isDownDefinition);

        Debug.Log($"DefinitionTile with least possible word entry {definitionTile}");

        if(definitionTile == null){
            return false;
        }

        if(isDownDefinition){
            var randomIndex = Random.Range(0, definitionTile.possibleDownWordEntries.Count);
            var wordEntry = definitionTile.possibleDownWordEntries[randomIndex];
            Debug.Log($"Word chosen for down def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");
            return grid.SetFinalDownDefinition(definitionTile, wordEntry);
            
        }

        else{
            var randomIndex = Random.Range(0, definitionTile.possibleAcrossWordEntries.Count);
            var wordEntry = definitionTile.possibleAcrossWordEntries[randomIndex];
            Debug.Log($"Word chosen for across def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");
            return grid.SetFinalAcrossDefinition(definitionTile, wordEntry);
            
        }
    }
}
