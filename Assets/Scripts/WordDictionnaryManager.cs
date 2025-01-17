using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class WordDictionnaryManager : MonoBehaviour
{

    [SerializeField]
    TextAsset wordListFile;

    [SerializeField]
    string wordDictionnaryFileName;

    string wordDictionnaryFilePath;

    [SerializeField]
    string wordDictionnaryByWordLenghtFileName;

    string wordDictionnaryByWordLenghtFilePath;

    WordDictionnary wordDictionnary;

    [SerializeField]
    string wordSearch;

    string lastWordSearch;

    [SerializeField]
    List<WordEntry> result;
    
    // Start is called before the first frame update
    void Awake(){
       //Read the text from directly from the test.txt file
        //Debug.Log(wordListFile.text);

        wordDictionnaryFilePath = Path.Combine(Application.persistentDataPath, $"{wordDictionnaryFileName}.json");

        wordDictionnaryByWordLenghtFilePath = Path.Combine(Application.persistentDataPath, $"{wordDictionnaryByWordLenghtFileName}.json");

        Dictionary<string, WordEntry> loadedWordDict = null;

        Dictionary<int, List<WordEntry>> loadedWordDictByWordLenght = null;

        if(File.Exists(wordDictionnaryFilePath) && File.Exists(wordDictionnaryByWordLenghtFilePath)){

            using(var sr1 = new StreamReader(wordDictionnaryFilePath)){
                loadedWordDict = JsonConvert.DeserializeObject<Dictionary<string, WordEntry>>(sr1.ReadToEnd());
            }
            using(var sr2 = new StreamReader(wordDictionnaryByWordLenghtFilePath)){
                loadedWordDictByWordLenght = JsonConvert.DeserializeObject<Dictionary<int, List<WordEntry>>>(sr2.ReadToEnd());
            }
        }

        if(loadedWordDict != null && loadedWordDictByWordLenght != null){
            Debug.Log("WordDictionnary was loaded");
            wordDictionnary = new WordDictionnary(loadedWordDict, loadedWordDictByWordLenght);
        }
        else{
            Debug.Log("WordDictionnary was not loaded. Creating a new one from wordListFile");
            wordDictionnary = new WordDictionnary();
            using (StringReader reader = new StringReader(wordListFile.text)){
            string line;
            while ((line = reader.ReadLine()) != null){
                WordEntry wordEntry = new WordEntry(line);
                //Debug.Log($"{wordEntry.word} : {wordEntry.wordWithoutDiacritics} -> {wordEntry.wordWithoutSpecialChars}, {wordEntry.complexityScore}");
                wordDictionnary.AddEntry(wordEntry);
            }
        }


            foreach(KeyValuePair<int, List<WordEntry>> keyValuePair  in wordDictionnary.wordDictionnaryByWordLenght){
                Debug.Log($"Number of words with {keyValuePair.Key} letters : {keyValuePair.Value.Count}");
            }
        }
        
        
    }

    // Update is called once per frame
    void Update(){
        if(lastWordSearch != wordSearch){
            result = wordDictionnary.SearchWord(wordSearch, out int iterations);
            Debug.Log($"{result.Count} results found in {iterations} iterations");
        }

        lastWordSearch = wordSearch;
    }

    void OnDisable(){
        wordDictionnary.SaveDictionnariesToFile(wordDictionnaryFilePath, wordDictionnaryByWordLenghtFilePath);
    }


    void AddEntry(){
        
    }
}
