
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum DefinitionTileLayout{
    FirstWordOnly, SecondWordOnly, FirstAndSecondWord
}


public class DefinitionTile : Tile
{

    public DefinitionTileLayout definitionTileLayout {get; private set;}

    public WordEntry finalFirstWordEntry {get; set;}

    public WordEntry finalSecondWordEntry {get; set;}

    public bool finalFirstWordEntryIsMissing {get {
        return finalFirstWordEntry == null && 
            (definitionTileLayout == DefinitionTileLayout.FirstWordOnly || definitionTileLayout == DefinitionTileLayout.FirstAndSecondWord);
    }}

    public bool finalSecondWordEntryIsMissing {get {
        return finalSecondWordEntry == null &&
            (definitionTileLayout == DefinitionTileLayout.SecondWordOnly || definitionTileLayout == DefinitionTileLayout.FirstAndSecondWord);
    }}

    public bool firstWordGoesDown {get; private set;}

    public bool secondWordGoesAcross {get; private set;}

    public string firstWordSearch {get; set;}

    public string secondWordSearch {get; set;}

    public bool crossesAtLeastOneWordFirst {get; private set;}

    public bool crossesAtLeastOneWordSecond {get; private set;}

    public List<WordEntry> possibleFirstWordEntries {get; private set;}

    public List<WordEntry> possibleSecondWordEntries {get; private set;}

    private List<Tile> tilesReachedByFirstDefinition_;
    public List<Tile> tilesReachedByFirstDefinition {
        get{
            return tilesReachedByFirstDefinition_;
        } 
        set{
            tilesReachedByFirstDefinition_ = value;
            if(value != null && value.Count > 1){
                if(tilesReachedBySecondDefinition != null && tilesReachedBySecondDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.FirstAndSecondWord;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.FirstWordOnly;
                }
            }
            UpdateWordSearch();
            
        }
    }


    private List<Tile> tilesReachedBySecondDefinition_;

    public List<Tile> tilesReachedBySecondDefinition {
        get {
            return tilesReachedBySecondDefinition_;
        }
        set {
            tilesReachedBySecondDefinition_ = value;
            if(value != null && value.Count > 1){
                if(tilesReachedByFirstDefinition != null && tilesReachedByFirstDefinition.Count > 1){
                    definitionTileLayout = DefinitionTileLayout.FirstAndSecondWord;
                }
                else{
                    definitionTileLayout = DefinitionTileLayout.SecondWordOnly;
                }
                UpdateWordSearch();
            }
        }
    }

    public DefinitionTile(
        int x, 
        int y,
        DefinitionTileLayout definitionTileLayout = DefinitionTileLayout.FirstAndSecondWord, 
        bool secondWordGoesAcross = false, 
        bool firstWordGoesDown = false

    ) : base(x, y, isDefinition:true){
        
        this.definitionTileLayout = definitionTileLayout;
        this.secondWordGoesAcross = secondWordGoesAcross;
        this.firstWordGoesDown = firstWordGoesDown;
        tilesReachedBySecondDefinition = null;
        tilesReachedByFirstDefinition = null;
        possibleFirstWordEntries = null;
        possibleSecondWordEntries = null;
        crossesAtLeastOneWordFirst= false;
        crossesAtLeastOneWordSecond = false;
    }


    public void UpdateWordSearch(){
        if(finalFirstWordEntry != null){
            firstWordSearch = finalFirstWordEntry.wordWithoutSpecialChars;
        }
        
        else if(tilesReachedByFirstDefinition != null){
            crossesAtLeastOneWordFirst = false;
            StringBuilder stringBuilderFirst = new StringBuilder();
            foreach (var tile in tilesReachedByFirstDefinition){
                switch (tile){
                    case LetterTile letterTile:
                        stringBuilderFirst.Append(letterTile.letter);

                        crossesAtLeastOneWordFirst = true;
                        break;

                    default:
                        stringBuilderFirst.Append('*');
                        break;
                }
            }

            firstWordSearch = stringBuilderFirst.ToString();
        }

        if(finalSecondWordEntry != null){
            secondWordSearch = finalSecondWordEntry.wordWithoutSpecialChars;
        }
        
        else if(tilesReachedBySecondDefinition != null){
            crossesAtLeastOneWordSecond = false;
        
            StringBuilder stringBuilderSecond = new StringBuilder();
            foreach (var tile in tilesReachedBySecondDefinition){
                switch (tile){
                    case LetterTile letterTile:
                        stringBuilderSecond.Append(letterTile.letter);
                        crossesAtLeastOneWordSecond = true;
                        break;

                    default:
                        stringBuilderSecond.Append('*');
                        break;
                }
            }

            secondWordSearch = stringBuilderSecond.ToString();
        }
    }


    public bool CanWordFitInFirstWordSearch(string word){
        if (word == null){
            return false;
        }

        if(definitionTileLayout == DefinitionTileLayout.SecondWordOnly){
            return false;
        }


        if(word.Length != firstWordSearch.Length){
            return false;
        }

        for(int i = 0; i < word.Length; i++){
            char charWord = word[i];
            char charSearchWord = firstWordSearch[i];

            if(charSearchWord != '*' && charWord != charSearchWord){
                return false;
            }
        }

        return true;
    }

    public bool CanWordFitInSecondWordSearch(string word){
        if (word == null){
            return false;
        }

        if(definitionTileLayout == DefinitionTileLayout.FirstWordOnly){
            return false;
        }

        if(word.Length != secondWordSearch.Length){
            return false;
        }

        for(int i = 0; i < word.Length; i++){
            char charWord = word[i];
            char charSearchWord = secondWordSearch[i];

            if(charSearchWord != '*' && charWord != charSearchWord){
                return false;
            }
        }

        return true;
    }


    public void InitializePossibleFirstWordEntries(List<WordEntry> wordEntries){
        possibleFirstWordEntries = wordEntries;
    }

    public void InitializePossibleSecondWordEntries(List<WordEntry> wordEntries){
        possibleSecondWordEntries = wordEntries;
    }


    public void UpdatePossibleFirstWordEntries(){

        if(!finalFirstWordEntryIsMissing){
            return;
        }

        if(possibleFirstWordEntries == null){
            Debug.LogWarning($"Trying to update possibleFirstWordEntries of {this} without initializing it first");
            return;
        }

        var index = 0;
        while (index < possibleFirstWordEntries.Count){
            var wordEntryWord = possibleFirstWordEntries[index].wordWithoutSpecialChars;

            if(wordEntryWord.Length != firstWordSearch.Length){
                possibleFirstWordEntries.RemoveAt(index);
                continue;
            }

            var didRemoveEntry = false;
            for (int i = 0; i < firstWordSearch.Length; i++){
                char charWordSearch = firstWordSearch[i];
                char charWordEntryWord = wordEntryWord[i];

                if(charWordSearch != '*' && charWordEntryWord != charWordSearch){
                    possibleFirstWordEntries.RemoveAt(index);
                    //Debug.Log($"For {this}, {wordEntryWord} cannot fit in {firstWordSearch}");
                    didRemoveEntry = true;
                    break;
                }
            }

            if(didRemoveEntry){
                continue;
            }

            //Debug.Log($"For {this}, {wordEntryWord} can fit in {firstWordSearch}");

            

            index ++;
        }

        Debug.Log($"{possibleFirstWordEntries.Count} possible first word entries for {this}");
    }

    public void UpdatePossibleSecondWordEntries(){

        if(!finalSecondWordEntryIsMissing){
            return;
        }

        if(possibleSecondWordEntries == null){
            Debug.LogWarning($"Trying to update possibleSecondWordEntries of {this} without initializing it first");
            return;
        }



        var index = 0;
        while (index < possibleSecondWordEntries.Count){
            var wordEntryWord = possibleSecondWordEntries[index].wordWithoutSpecialChars;
            //Debug.Log($"For {this}, testing if {wordEntryWord} can fit in {secondWordSearch}");
            if(wordEntryWord.Length != secondWordSearch.Length){
                possibleSecondWordEntries.RemoveAt(index);
                continue;
            }

            var didRemoveEntry = false;
            for (int i = 0; i < secondWordSearch.Length; i++){
                char charWordSearch = secondWordSearch[i];
                char charWordEntryWord = wordEntryWord[i];

                if(charWordSearch != '*' && charWordEntryWord != charWordSearch){
                    possibleSecondWordEntries.RemoveAt(index);
                    //Debug.Log($"For {this}, {wordEntryWord} cannot fit in {secondWordSearch}");
                    didRemoveEntry = true;
                    break;
                }
            }

            if(didRemoveEntry){
                continue;
            }

            //Debug.Log($"For {this}, {wordEntryWord} can fit in {secondWordSearch}");

            index ++;
        }

        Debug.Log($"{possibleSecondWordEntries.Count} possible second word entries for {this}");
    }


    public override object Clone()
    {
        DefinitionTile tileClone = new DefinitionTile(x, y, definitionTileLayout, secondWordGoesAcross, firstWordGoesDown);
        tileClone.finalFirstWordEntry = finalFirstWordEntry;
        tileClone.finalSecondWordEntry = finalSecondWordEntry;
        return tileClone;
    }

}


