using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

public class WordDictionnary {



    public Dictionary<string, WordEntry> wordDictionnary = new Dictionary<string, WordEntry>();


    public void AddEntry(WordEntry wordEntry) {
        wordDictionnary[wordEntry.wordWithoutSpecialChars] = wordEntry;
    }



    public List<WordEntry> SearchWord(string word) {

        List<WordEntry> results = new List<WordEntry>();

        if(word == null) return results;

        var newWord = WordUtils.RemoveDiacritics(word).ToUpper();

        newWord = WordUtils.RemoveSpecialChars(newWord);

        if(newWord.IndexOf('*') != -1) {
            var starCount = WordUtils.CountSubstring(newWord, "*");
            if(starCount < 4){
                
                var indexesToCheck = new int[starCount];

                for(int i = 0; i < starCount; i++){
                    indexesToCheck[i] = WordUtils.letters.Length - 1;
                }

                var iterationCount = 0;

                while(indexesToCheck[0] >= 0){
                    //Create word

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

                    iterationCount ++;

                    if(iterationCount > 20000){
                        Debug.Log("REACHED ITERATION LIMIT");
                    }
                }


            }
            else{
                
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
