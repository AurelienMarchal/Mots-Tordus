
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum DefinitionTileLayout{
    Across, Down, DownAndAcross
}


public class DefinitionTile : Tile
{

    public DefinitionTileLayout definitionTileLayout {get; private set;}

    public WordEntry finalDownWordEntry {get; set;}

    public WordEntry finalAcrossWordEntry {get; set;}

    public bool finalDownWordEntryIsMissing {get {
        return (definitionTileLayout == DefinitionTileLayout.Down || definitionTileLayout == DefinitionTileLayout.DownAndAcross) && finalDownWordEntry == null;
    }}

    public bool finalAcrossWordEntryIsMissing {get {
        return (definitionTileLayout == DefinitionTileLayout.Across || definitionTileLayout == DefinitionTileLayout.DownAndAcross) && finalAcrossWordEntry == null;
    }}

    public bool acrossWordStartsOneTileLower {get; private set;}

    public bool downWordStartsOneTileRight {get; private set;}

    public string downWordSearch {get; set;}

    public string acrossWordSearch {get; set;}

    public bool crossesAtLeastOneWordDown {get; private set;}

    public bool crossesAtLeastOneWordAcross {get; private set;}

    public List<WordEntry> possibleDownWordEntries {get; private set;}

    public List<WordEntry> possibleAcrossWordEntries {get; private set;}

    private List<Tile> tilesReachedByDownDefinition_;
    public List<Tile> tilesReachedByDownDefinition {
        get{
            return tilesReachedByDownDefinition_;
        } 
        set{
            tilesReachedByDownDefinition_ = value;
            if(value != null && value.Count > 1){
                if(tilesReachedByAcrossDefinition != null && tilesReachedByAcrossDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.DownAndAcross;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.Down;
                }
            }
            UpdateWordSearch();
            
        }
    }


    private List<Tile> tilesReachedByAcrossDefinition_;

    public List<Tile> tilesReachedByAcrossDefinition {
        get {
            return tilesReachedByAcrossDefinition_;
        }
        set {
            tilesReachedByAcrossDefinition_ = value;
            if(value != null && value.Count > 1){
                if(tilesReachedByDownDefinition != null && tilesReachedByDownDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.DownAndAcross;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.Across;
                }
                UpdateWordSearch();
            }
        }
    }

    public DefinitionTile(
        int x, 
        int y,
        DefinitionTileLayout definitionTileLayout = DefinitionTileLayout.DownAndAcross, 
        bool acrossWordStartsOneTileLower = false, 
        bool downWordStartsOneTileRight = false

    ) : base(x, y, isDefinition:true){
        
        this.definitionTileLayout = definitionTileLayout;
        this.acrossWordStartsOneTileLower = acrossWordStartsOneTileLower;
        this.downWordStartsOneTileRight = downWordStartsOneTileRight;
        tilesReachedByAcrossDefinition = null;
        tilesReachedByDownDefinition = null;
        possibleDownWordEntries = null;
        possibleAcrossWordEntries = null;
        crossesAtLeastOneWordAcross = false;
        crossesAtLeastOneWordAcross = false;
    }


    public void UpdateWordSearch(){
        if(finalDownWordEntry != null){
            downWordSearch = finalDownWordEntry.wordWithoutSpecialChars;
        }
        
        else if(tilesReachedByDownDefinition != null){
            crossesAtLeastOneWordDown = false;
            StringBuilder stringBuilderDown = new StringBuilder();
            foreach (var tile in tilesReachedByDownDefinition){
                switch (tile){
                    case LetterTile letterTile:
                        stringBuilderDown.Append(letterTile.letter);

                        crossesAtLeastOneWordDown = true;
                        break;

                    default:
                        stringBuilderDown.Append('*');
                        break;
                }
            }

            downWordSearch = stringBuilderDown.ToString();
        }

        if(finalAcrossWordEntry != null){
            acrossWordSearch = finalAcrossWordEntry.wordWithoutSpecialChars;
        }
        
        else if(tilesReachedByAcrossDefinition != null){
            crossesAtLeastOneWordAcross = false;
        
            StringBuilder stringBuilderAcross = new StringBuilder();
            foreach (var tile in tilesReachedByAcrossDefinition){
                switch (tile){
                    case LetterTile letterTile:
                        stringBuilderAcross.Append(letterTile.letter);
                        crossesAtLeastOneWordAcross = true;
                        break;

                    default:
                        stringBuilderAcross.Append('*');
                        break;
                }
            }

            acrossWordSearch = stringBuilderAcross.ToString();
        }
    }


    public bool CanWordFitAcross(string word){
        if (word == null){
            return false;
        }

        if(definitionTileLayout == DefinitionTileLayout.Down){
            return false;
        }

        if(word.Length != acrossWordSearch.Length){
            return false;
        }

        for(int i = 0; i < word.Length; i++){
            char charWord = word[i];
            char charSearchWord = acrossWordSearch[i];

            if(charSearchWord != '*' && charWord != charSearchWord){
                return false;
            }
        }

        return true;
    }



    public bool CanWordFitDown(string word){
        if (word == null){
            return false;
        }

        if(definitionTileLayout == DefinitionTileLayout.Across){
            return false;
        }

        if(word.Length != downWordSearch.Length){
            return false;
        }

        for(int i = 0; i < word.Length; i++){
            char charWord = word[i];
            char charSearchWord = downWordSearch[i];

            if(charSearchWord != '*' && charWord != charSearchWord){
                return false;
            }
        }

        return true;
    }


    public void InitializePossibleDownWordEntries(List<WordEntry> wordEntries){
        possibleDownWordEntries = wordEntries;
    }

    public void InitializePossibleAcrossWordEntries(List<WordEntry> wordEntries){
        possibleAcrossWordEntries = wordEntries;
    }


    public void UpdatePossibleDownWordEntries(){

        if(!finalDownWordEntryIsMissing){
            return;
        }

        if(possibleDownWordEntries == null){
            Debug.LogWarning($"Trying to update possibleDownWordEntries of {this} without initializing it first");
            return;
        }

        var index = 0;
        while (index < possibleDownWordEntries.Count){
            var wordEntryWord = possibleDownWordEntries[index].wordWithoutSpecialChars;

            //Debug.Log($"For {this}, testing if {wordEntryWord} can fit in {downWordSearch}");

            if(wordEntryWord.Length != downWordSearch.Length){
                possibleDownWordEntries.RemoveAt(index);
                continue;
            }

            for (int i = 0; i < downWordSearch.Length; i++){
                char charWordSearch = downWordSearch[i];
                char charWordEntryWord = wordEntryWord[i];

                if(charWordSearch != '*' && charWordEntryWord != charWordSearch){
                    possibleDownWordEntries.RemoveAt(index);
                    continue;
                }

            }

            index ++;
        }

        Debug.Log($"{possibleDownWordEntries.Count} possible down word entries for {this}");
    }

    public void UpdatePossibleAcrossWordEntries(){

        if(!finalAcrossWordEntryIsMissing){
            return;
        }

        if(possibleAcrossWordEntries == null){
            Debug.LogWarning($"Trying to update possibleAcrossWordEntries of {this} without initializing it first");
            return;
        }



        var index = 0;
        while (index < possibleAcrossWordEntries.Count){
            var wordEntryWord = possibleAcrossWordEntries[index].wordWithoutSpecialChars;
            //Debug.Log($"For {this}, testing if {wordEntryWord} can fit in {acrossWordSearch}");
            if(wordEntryWord.Length != acrossWordSearch.Length){
                possibleAcrossWordEntries.RemoveAt(index);
                continue;
            }

            for (int i = 0; i < acrossWordSearch.Length; i++){
                char charWordSearch = acrossWordSearch[i];
                char charWordEntryWord = wordEntryWord[i];

                if(charWordSearch != '*' && charWordEntryWord != charWordSearch){
                    possibleAcrossWordEntries.RemoveAt(index);
                    continue;
                }

            }

            index ++;
        }

        Debug.Log($"{possibleAcrossWordEntries.Count} possible across word entries for {this}");
    }

}


