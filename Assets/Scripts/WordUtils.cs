
using System;
using System.Globalization;
using System.Text;


public class WordUtils{
    public static string RemoveDiacritics(string text) 
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    public static string RemoveSpecialChars(string word){
        
        
        var newWord = string.Copy(word);

        foreach(char c in specialChars){
            newWord = newWord.Replace(c.ToString(), string.Empty);
        }

        return newWord;
    }

    public static int CountSubstring(string text, string value){                  
        int count = 0, minIndex = text.IndexOf(value, 0);
        while (minIndex != -1){
            minIndex = text.IndexOf(value, minIndex + value.Length);
            count++;
        }
        return count;
    }

    public static int CalculateWordComplexityScore(string word){

        int score = 0;

        foreach (char c in word){
            score += letterRarityScore[Array.IndexOf(letters, c)];
        }

        return score;

    }

    public static readonly char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

    public static readonly int[] letterRarityScore = {1, 5, 2, 2, 1, 5, 10, 20, 2, 10, 100, 1, 2, 1, 2, 1, 50, 1, 1, 1, 2, 5, 100, 50, 100, 50};


    public static readonly char[] specialChars = {'-', '\'', '.'};
    
}