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
        if(!grid.IsGenerationFinished()){
            Debug.Log("validGridHistory : "  + String.Join(", ", validGridHistory));
            Debug.Log("definitionTilesChangedHistory : "  + String.Join(", ", definitionTilesChangedHistory));
            Debug.Log("wordEntrySetHistory : "  + String.Join(", ", wordEntrySetHistory));


            var gridIsValid = NextGenerationIteration(grid);
            gridManager.grid = grid;
            if(!gridIsValid){
                Debug.Log("Change was not valid, going back to last valid grid");
                validGridHistory.RemoveAt(validGridHistory.Count - 1);
                grid = validGridHistory.Last();
                
                gridManager.grid = grid;
                var tileChanged = grid.GetTileAt(definitionTilesChangedHistory.Last().x, definitionTilesChangedHistory.Last().y);
                if(tileChanged is DefinitionTile definitionTileChanged){
                    if(changeWasOnFirstWordHistory.Last()){
                        definitionTileChanged.discardedFirstWordEntries.Add(wordEntrySetHistory.Last());
                        Debug.Log($"Discarding {wordEntrySetHistory.Last().wordWithoutDiacritics} from {definitionTilesChangedHistory.Last()} possible first words");
                    }
                    else{
                        definitionTileChanged.discardedSecondWordEntries.Add(wordEntrySetHistory.Last());
                        Debug.Log($"Discarding {wordEntrySetHistory.Last().wordWithoutDiacritics} from {definitionTilesChangedHistory.Last()} possible second words");
                    }
                    definitionTilesChangedHistory.RemoveAt(definitionTilesChangedHistory.Count - 1);
                    wordEntrySetHistory.RemoveAt(wordEntrySetHistory.Count - 1);
                    changeWasOnFirstWordHistory.RemoveAt(changeWasOnFirstWordHistory.Count - 1);

                    grid.UpdatePossibleWordEntries(wordDictionnaryManager.wordDictionnary);
                    var check = grid.IsThereADefinitionTileWith0PossibleWordEntry(out bool isFirstWord, out DefinitionTile definitionTileFound);
                    if(check){
                        validGridHistory.RemoveAt(validGridHistory.Count - 1);

                        tileChanged = grid.GetTileAt(definitionTilesChangedHistory.Last().x, definitionTilesChangedHistory.Last().y);

                        if(tileChanged is DefinitionTile){
                            definitionTileChanged = (DefinitionTile)tileChanged;
                            if(changeWasOnFirstWordHistory.Last()){
                                definitionTileChanged.discardedFirstWordEntries.Add(wordEntrySetHistory.Last());
                                Debug.Log($"Discarding {wordEntrySetHistory.Last().wordWithoutDiacritics} from {definitionTilesChangedHistory.Last()} possible first words");
                            }
                            else{
                                definitionTileChanged.discardedSecondWordEntries.Add(wordEntrySetHistory.Last());
                                Debug.Log($"Discarding {wordEntrySetHistory.Last().wordWithoutDiacritics} from {definitionTilesChangedHistory.Last()} possible second words");
                            }
                            definitionTilesChangedHistory.RemoveAt(definitionTilesChangedHistory.Count - 1);
                            wordEntrySetHistory.RemoveAt(wordEntrySetHistory.Count - 1);
                            changeWasOnFirstWordHistory.RemoveAt(changeWasOnFirstWordHistory.Count - 1);

                            grid = validGridHistory.Last();
                            gridManager.grid = grid;
                        }
                    }
                }
            }
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

        var randomIndex = UnityEngine.Random.Range(0, wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length].Count);
        var wordEntry = wordDictionnaryManager.wordDictionnary.wordDictionnaryByWordLenght[definitionTile.firstWordSearch.Length][randomIndex];
        return grid.SetFinalFirstDefinition(definitionTile, wordEntry);

    }

    bool NextGenerationIteration(Grid grid){
        grid.UpdateTilesReachedByDefinition();
        
        bool didUpdate = grid.UpdatePossibleWordEntries(wordDictionnaryManager.wordDictionnary);

        if(!didUpdate){
            return false;
        }

        DefinitionTile definitionTile = grid.GetDefinitionTileWithTheLeastPossibleWordEntry(out bool isFirstDefinition);

        Debug.Log($"DefinitionTile with least possible word entry {definitionTile}");

        if(definitionTile == null){
            return false;
        }

        if(isFirstDefinition){
            if(definitionTile.possibleFirstWordEntries.Count == 0){
                return false;
            }
            else{
                validGridHistory.Add((Grid)grid.Clone());
            }

            var randomIndex = UnityEngine.Random.Range(0, definitionTile.possibleFirstWordEntries.Count);
            randomIndex = definitionTile.possibleFirstWordEntries.Count - 1;
            var wordEntry = definitionTile.possibleFirstWordEntries[randomIndex];
            wordEntrySetHistory.Add(wordEntry);
            definitionTilesChangedHistory.Add(definitionTile);
            changeWasOnFirstWordHistory.Add(true);
            Debug.Log($"Word chosen for first def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");

            return grid.SetFinalFirstDefinition(definitionTile, wordEntry);
            
        }

        else{
            if(definitionTile.possibleSecondWordEntries.Count == 0){
                return false;
            }
            else{
                validGridHistory.Add((Grid)grid.Clone());
            }


            var randomIndex = UnityEngine.Random.Range(0, definitionTile.possibleSecondWordEntries.Count);
            randomIndex = 0;
            var wordEntry = definitionTile.possibleSecondWordEntries[randomIndex];
            wordEntrySetHistory.Add(wordEntry);
            definitionTilesChangedHistory.Add(definitionTile);
            changeWasOnFirstWordHistory.Add(false);
            Debug.Log($"Word chosen for second def of {definitionTile} is {wordEntry.wordWithoutDiacritics}");
            
            
            return grid.SetFinalSecondDefinition(definitionTile, wordEntry);
            
        }
    }
}
