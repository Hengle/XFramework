using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    private static string SPLIT_RE = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
    private static string LINE_SPLIT_RE = "\\r\\n|\\n\\r|\\n|\\r";
    private static char[] TRIM_CHARS = new char[1] { '"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        TextAsset textAsset = Resources.Load(file) as TextAsset;
        if ((Object)textAsset == (Object)null)
        {
            XLogger.LogError("file in Resources There is no");
            return (List<Dictionary<string, object>>)null;
        }
        List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
        string[] strArray1 = Regex.Split(textAsset.text, CSVReader.LINE_SPLIT_RE);
        if (strArray1.Length <= 1)
            return dictionaryList;
        string[] strArray2 = Regex.Split(strArray1[0], CSVReader.SPLIT_RE);
        for (int index1 = 1; index1 < strArray1.Length; ++index1)
        {
            string[] strArray3 = Regex.Split(strArray1[index1], CSVReader.SPLIT_RE);
            if (strArray3.Length != 0 && !(strArray3[0] == string.Empty))
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int index2 = 0; index2 < strArray2.Length && index2 < strArray3.Length; ++index2)
                {
                    string s = strArray3[index2].TrimStart(CSVReader.TRIM_CHARS).TrimEnd(CSVReader.TRIM_CHARS).Replace("\\", string.Empty);
                    object obj = (object)s;
                    if (int.TryParse(s, out int result1))
                    {
                        obj = (object)result1;
                    }
                    else
                    {
                        if (float.TryParse(s, out float result2))
                            obj = (object)result2;
                    }
                    dictionary[strArray2[index2]] = obj;
                }
                dictionaryList.Add(dictionary);
            }
        }
        return dictionaryList;
    }
}
