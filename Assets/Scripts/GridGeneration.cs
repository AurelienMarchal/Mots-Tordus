using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    bool generationFinised = false;

    Grid grid;

    List<Grid> validGridHistory;

    List<DefinitionTile> definitionTilesChangedHistory;

    List<bool> changeWasOnFirstWordHistory;

    List<WordEntry> wordEntrySetHistory;

    [SerializeField]
    int seed;

    // Start is called before the first frame update
    void Start(){

        validGridHistory = new List<Grid>();
        definitionTilesChangedHistory = new List<DefinitionTile>();
        wordEntrySetHistory = new List<WordEntry>();
        changeWasOnFirstWordHistory = new List<bool>();

        UnityEngine.Random.InitState(seed);

        grid = new Grid(9, 11, new List<Obstacle>{new Obstacle(0, 0, 2, 2), new Obstacle(4, 4, 2, 2)});

        grid.GenerateTopAndRightDefinitionTiles();

        gridManager.grid = grid;

        generationFinised = true;

        /*
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

        UpdateGrid();
        validGridHistory.Add((Grid)grid.Clone());
        ResetGenerationAndSetFirstWord();
        */
    }

    // Update is called once per frame
    void Update(){
        if(!grid.IsGenerationFinished() && !generationFinised){
            //Debug.Log("validGridHistory : "  + string.Join(", ", validGridHistory));
            //Debug.Log("definitionTilesChangedHistory : "  + string.Join(", ", definitionTilesChangedHistory));
            //Debug.Log("wordEntrySetHistory : "  + string.Join(", ", wordEntrySetHistory));

            UpdateGrid();
            gridManager.grid = grid;

            var gridValidity = CheckGridValidity();

            if(!gridValidity){
                Debug.Log("Grid not valid, discarding last word and going back");

                grid = validGridHistory.Last();

                var lastWordEntry = wordEntrySetHistory.Last();
                var lastDefinitionTileChanged = definitionTilesChangedHistory.Last();
                var lastChangeWasOnFirstWord = changeWasOnFirstWordHistory.Last();

                var wasDiscarded = DiscardWordFromPossibleWordEntries(lastDefinitionTileChanged.x, lastDefinitionTileChanged.y, lastWordEntry, lastChangeWasOnFirstWord);
                if(!wasDiscarded){
                    Debug.LogError($"Could not discard word entry");
                }
                GoBackOneHistoryIteration();
            }
            else{
                validGridHistory.Add((Grid)grid.Clone());
                var didChooseWord = ChooseNextWord(out DefinitionTile definitionTileChanged, out WordEntry wordEntryChoosen, out bool isFirstWord);
                if(didChooseWord){
                    if(isFirstWord){
                        grid.SetFinalFirstDefinition(definitionTileChanged, wordEntryChoosen);
                    }
                    else{
                        grid.SetFinalSecondDefinition(definitionTileChanged, wordEntryChoosen);
                    }

                    wordEntrySetHistory.Add(wordEntryChoosen);
                    definitionTilesChangedHistory.Add(definitionTileChanged);
                    changeWasOnFirstWordHistory.Add(isFirstWord);

                }
                else{
                    Debug.LogError("Could not choose word");
                }
            }
        }

        else if(!generationFinised){
            generationFinised = true;
            UpdateGrid();
            gridManager.grid = grid;
        }

        if(Input.GetKeyDown(KeyCode.R)){
            ResetGenerationAndSetFirstWord();
            gridManager.grid = grid;
            
        }
    }

    void ResetGenerationAndSetFirstWord(){
        generationFinised = false;
        grid = validGridHistory.First();
        validGridHistory.Clear();
        validGridHistory.Add((Grid)grid.Clone());
        definitionTilesChangedHistory.Clear();
        wordEntrySetHistory.Clear();
        changeWasOnFirstWordHistory.Clear();
        SetFirstWord();
        UpdateGrid();
    }

    bool SetFirstWord(){
        DefinitionTile definitionTile = grid.FindDefinitionTileWithLongestFirstWordEntryMissing();
        if(definitionTile == null){
            return false;
        }

        if(!wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght.ContainsKey(definitionTile.firstWordSearch.Length)){
            return false;
        }

        var randomIndex = UnityEngine.Random.Range(0, wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length].Count);
        var wordEntry = wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length][randomIndex];
        return grid.SetFinalFirstDefinition(definitionTile, wordEntry);

    }


    bool UpdateGrid(){
        grid.UpdateTilesReachedByDefinition();
        return grid.UpdatePossibleWordEntries(wordDictionnaryManager.wordDictionnary);
    }

    bool CheckGridValidity(){
        return !grid.AtLeastOneDefinitionTileHasNoPossibleWordEntry();
    }

    bool ChooseNextWord(out DefinitionTile definitionTileChanged, out WordEntry wordEntryChoosen, out bool isFirstWord){
        
        definitionTileChanged = grid.GetDefinitionTileWithTheLeastPossibleWordEntry(out isFirstWord);

        wordEntryChoosen = null;

        Debug.Log($"DefinitionTile with least possible word entry {definitionTileChanged}");

        if(definitionTileChanged == null){
            return false;
        }

        if(isFirstWord){
            if(definitionTileChanged.possibleFirstWordEntries.Count == 0){
                return false;
            }

            var randomIndex = UnityEngine.Random.Range(0, definitionTileChanged.possibleFirstWordEntries.Count);
            //randomIndex = definitionTileChanged.possibleFirstWordEntries.Count - 1;
            wordEntryChoosen = definitionTileChanged.possibleFirstWordEntries[randomIndex];
            //wordEntrySetHistory.Add(wordEntrySet);
            //definitionTilesChangedHistory.Add(definitionTileChanged);
            //changeWasOnFirstWordHistory.Add(true);
            Debug.Log($"Word chosen for first def of {definitionTileChanged} is {wordEntryChoosen.wordWithoutDiacritics}");

            return true;
            
        }

        else{
            if(definitionTileChanged.possibleSecondWordEntries.Count == 0){
                return false;
            }

            var randomIndex = UnityEngine.Random.Range(0, definitionTileChanged.possibleSecondWordEntries.Count);
            //randomIndex = definitionTileChanged.possibleSecondWordEntries.Count - 1;
            wordEntryChoosen = definitionTileChanged.possibleSecondWordEntries[randomIndex];
            //wordEntrySetHistory.Add(wordEntrySet);
            //definitionTilesChangedHistory.Add(definitionTileChanged);
            //changeWasOnFirstWordHistory.Add(false);
            Debug.Log($"Word chosen for second def of {definitionTileChanged} is {wordEntryChoosen.wordWithoutDiacritics}");

            return true;
            
        }
    }

    void GoBackOneHistoryIteration(){
        validGridHistory.RemoveAt(validGridHistory.Count - 1);
        definitionTilesChangedHistory.RemoveAt(definitionTilesChangedHistory.Count - 1);
        wordEntrySetHistory.RemoveAt(wordEntrySetHistory.Count - 1);
        changeWasOnFirstWordHistory.RemoveAt(changeWasOnFirstWordHistory.Count - 1);
    }



    bool DiscardWordFromPossibleWordEntries(int definitionTileX, int definitionTileY, WordEntry wordEntryToRemove, bool isFirstDef){
        var tile = grid.GetTileAt(definitionTileX, definitionTileY);
        if(tile != null && tile is DefinitionTile definitionTile){
            if(isFirstDef){
                if(!definitionTile.discardedFirstWordEntries.Contains(wordEntryToRemove)){
                    definitionTile.discardedFirstWordEntries.Add(wordEntryToRemove);
                    Debug.Log($"Discarding {wordEntryToRemove.wordWithoutDiacritics} from {definitionTile} possible first words");
                    
                }
            }
            else{
                if(!definitionTile.discardedSecondWordEntries.Contains(wordEntryToRemove)){
                    definitionTile.discardedSecondWordEntries.Add(wordEntryToRemove);
                    Debug.Log($"Discarding {wordEntryToRemove.wordWithoutDiacritics} from {definitionTile} possible second words");
                }
            }
            return true;
        }
        
        return false;
        
    }
}
