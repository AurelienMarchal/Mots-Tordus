using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using System.IO;

public class WordDictionnary {

    private Dictionary<string, WordEntry> wordDictionnary;

    public Dictionary<int, List<WordEntry>> wordDictionnaryByWordLenght = new Dictionary<int, List<WordEntry>>();


    public bool hasChanged;


    public WordDictionnary(){
        wordDictionnary = new Dictionary<string, WordEntry>();

        wordDictionnaryByWordLenght = new Dictionary<int, List<WordEntry>>();
        hasChanged = true;
    }

    public WordDictionnary(Dictionary<string, WordEntry> wordDictionnary, Dictionary<int, List<WordEntry>> wordDictionnaryByWordLenght){
        this.wordDictionnary = wordDictionnary;
        this.wordDictionnaryByWordLenght = wordDictionnaryByWordLenght;
        hasChanged = false;
    }

    public void AddEntry(WordEntry wordEntry) {
        wordDictionnary[wordEntry.wordWithoutSpecialChars] = wordEntry;

        hasChanged = true;
        var wordLenght = wordEntry.wordWithoutSpecialChars.Length;
        if(!wordDictionnaryByWordLenght.ContainsKey(wordLenght)){
            wordDictionnaryByWordLenght[wordLenght] = new List<WordEntry>{wordEntry};
        }
        else{
            
            for (int i = 0; i < wordDictionnaryByWordLenght[wordLenght].Count; i++){
                if(wordDictionnaryByWordLenght[wordLenght][i].complexityScore >= wordEntry.complexityScore){
                    wordDictionnaryByWordLenght[wordLenght].Insert(i, wordEntry);
                    break;
                }
            }
        }
    }

    public void SaveDictionnariesToFile(string wordDictionnaryFilePath, string wordDictionnaryByWordLenghtFilePath){

        if(!hasChanged){
            return;
        }

        if (!File.Exists(wordDictionnaryFilePath)){
            using (FileStream fs = File.Create(wordDictionnaryFilePath)) { };
        }

        using(var sw = new StreamWriter(wordDictionnaryFilePath)){
            sw.Write(JsonConvert.SerializeObject(wordDictionnary));
        }

        if (!File.Exists(wordDictionnaryByWordLenghtFilePath)){
            using (FileStream fs = File.Create(wordDictionnaryByWordLenghtFilePath)) { }
        }

        using(var sw = new StreamWriter(wordDictionnaryByWordLenghtFilePath)){
            sw.Write(JsonConvert.SerializeObject(wordDictionnaryByWordLenght));
        }
    }

    

    public List<WordEntry> SearchWord(string word, out int iterations) {

        List<WordEntry> results = new List<WordEntry>();

        iterations = 0;

        if(word == null) return results;

        var newWord = WordUtils.RemoveDiacritics(word).ToUpper();

        newWord = WordUtils.RemoveSpecialChars(newWord);

        if(newWord.IndexOf('*') != -1) {
            var starCount = WordUtils.CountSubstring(newWord, "*");
            if(false){
                
                var indexesToCheck = new int[starCount];

                for(int i = 0; i < starCount; i++){
                    indexesToCheck[i] = WordUtils.letters.Length - 1;
                }

                while(indexesToCheck[0] >= 0){
                    //Create word

                    iterations ++;

                    var wordToSeachBuilder = new StringBuilder();

                    var nbOfStars = 0;

                    foreach(char c in newWord){
                        if(c == '*'){
                            wordToSeachBuilder.Append(WordUtils.letters[indexesToCheck[nbOfStars]]);
                            nbOfStars ++;
                        }
                        else{
                            wordToSeachBuilder.Append(c);
                        }
                    }

                    if(wordDictionnary.ContainsKey(wordToSeachBuilder.ToString())){
                        results.Add(wordDictionnary[wordToSeachBuilder.ToString()]);
                    }


                    //Decreament indexes

                    indexesToCheck[starCount - 1] --;

                    for(int i = starCount - 1; i > 0; i--){
                        if(indexesToCheck[i] < 0){
                            indexesToCheck[i] = WordUtils.letters.Length - 1;
                            indexesToCheck[i - 1]--;
                        }
                    }

                    

                    if(iterations > 20000){
                        Debug.Log("REACHED ITERATION LIMIT");
                    }
                }


            }
            else{
                if(wordDictionnaryByWordLenght.ContainsKey(newWord.Length)){
                    foreach(var wordEntry in wordDictionnaryByWordLenght[newWord.Length]){
                        
                        if(newWord.Length != wordEntry.wordWithoutSpecialChars.Length){
                            continue;
                        }

                        iterations ++;

                        var matchFound = true;

                        for (int i = 0; i < newWord.Length; i++){
                            matchFound = newWord[i] == '*' || newWord[i] == wordEntry.wordWithoutSpecialChars[i];
                            if(!matchFound){
                                break;
                            }
                        }

                        if(matchFound){
                            results.Add(wordEntry);
                        }
                    }
                }
            }
        }

        else {

            if(wordDictionnary.ContainsKey(newWord)){
                results.Add(wordDictionnary[newWord]);
            }
            else{
                
            }
        }

        return results;

    }
}
