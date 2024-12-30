using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordDictionnaryManager : MonoBehaviour
{

    [SerializeField]
    TextAsset wordListFile;

    [SerializeField]
    string wordDictionnaryFileName;

    string wordDictionnaryFilePath;

    WordDictionnary wordDictionnary;

    [SerializeField]
    string wordSearch;

    [SerializeField]
    List<WordEntry> result;
    
    // Start is called before the first frame update
    void Awake(){
       //Read the text from directly from the test.txt file
        //Debug.Log(wordListFile.text);

        wordDictionnary = new WordDictionnary();

        wordDictionnaryFilePath = Path.Combine(Application.persistentDataPath, $"{wordDictionnaryFileName}.json");
        
        using (StringReader reader = new StringReader(wordListFile.text)){
            string line;
            while ((line = reader.ReadLine()) != null){
                WordEntry wordEntry = new WordEntry(line);
                //Debug.Log($"{wordEntry.word} : {wordEntry.wordWithoutDiacritics} -> {wordEntry.wordWithoutSpecialChars}");
                wordDictionnary.AddEntry(wordEntry);
            }
        }
    }

    // Update is called once per frame
    void Update(){
        result = wordDictionnary.SearchWord(wordSearch);
    }
}
