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

        grid.SetTile(new DefinitionTile(0, 0, firstWordGoesDown: true, secondWordGoesAcross: true));

        grid.SetTile(new DefinitionTile(2, 0, firstWordGoesDown : true, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(4, 0, firstWordGoesDown : true, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(6, 0, firstWordGoesDown : true, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(8, 0, firstWordGoesDown : false, secondWordGoesAcross: false));

        grid.SetTile(new DefinitionTile(0, 2, firstWordGoesDown : false, secondWordGoesAcross: true));
        grid.SetTile(new DefinitionTile(0, 4, firstWordGoesDown : false, secondWordGoesAcross: true));
        grid.SetTile(new DefinitionTile(0, 6, firstWordGoesDown : false, secondWordGoesAcross: true));
        grid.SetTile(new DefinitionTile(0, 8, firstWordGoesDown : false, secondWordGoesAcross: true));
        grid.SetTile(new DefinitionTile(0, 10, firstWordGoesDown : false, secondWordGoesAcross: false));

        
        grid.SetTile(new DefinitionTile(4, 1, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(3, 3, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(8, 3, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(1, 4, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(5, 5, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(7, 5, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(4, 6, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(2, 7, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(6, 7, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(3, 8, firstWordGoesDown : false, secondWordGoesAcross: false));
        grid.SetTile(new DefinitionTile(4, 10, firstWordGoesDown : false, secondWordGoesAcross: false));

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
        DefinitionTile definitionTile = grid.FindDefinitionTileWithLongestFirstWordEntryMissing();
        if(definitionTile == null){
            return false;
        }

        if(!wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght.ContainsKey(definitionTile.firstWordSearch.Length)){
            return false;
        }

        var randomIndex = Random.Range(0, wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length].Count);
        var wordEntry = wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length][randomIndex];
        return grid.SetFinalFirstDefinition(definitionTile, wordEntry);

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
            var randomIndex = Random.Range(0, definitionTile.possibleFirstWordEntries.Count);
            var wordEntry = definitionTile.possibleFirstWordEntries[randomIndex];
            Debug.Log($"Word chosen for first def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");
            return grid.SetFinalFirstDefinition(definitionTile, wordEntry);
            
        }

        else{
            var randomIndex = Random.Range(0, definitionTile.possibleSecondWordEntries.Count);
            var wordEntry = definitionTile.possibleSecondWordEntries[randomIndex];
            Debug.Log($"Word chosen for second def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");
            return grid.SetFinalSecondDefinition(definitionTile, wordEntry);
            
        }
    }
}
