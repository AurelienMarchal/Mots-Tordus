using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class WordEntry 
{
    [ReadOnly]
    public string word;

    [ReadOnly]
    public string wordWithoutDiacritics;

    [ReadOnly]
    public string wordWithoutSpecialChars;

    [ReadOnly]
    public int complexityScore;

    [SerializeField]
    public List<string> level1Definitions;

    [SerializeField]
    public List<string> level2Definitions;

    [SerializeField]
    public List<string> level3Definitions;

    [SerializeField]
    public List<string> level4Definitions;


    public WordEntry(string word){
        level1Definitions = new List<string>();
        level2Definitions = new List<string>();
        level3Definitions = new List<string>();
        level4Definitions = new List<string>();
        this.word = word;
        wordWithoutDiacritics = WordUtils.RemoveDiacritics(word.ToUpper());
        wordWithoutSpecialChars = WordUtils.RemoveSpecialChars(wordWithoutDiacritics);
        complexityScore = WordUtils.CalculateWordComplexityScore(wordWithoutSpecialChars);

    }

    public WordEntry(){

    }


    
}


public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI( Rect position,
                                SerializedProperty property,
                                GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
