using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalizationSystem 
{
    public enum Language
    {
        English,
        French
    }

    public static Language language = Language.English;

    private static Dictionary<string, string> _localizedEN;
    private static Dictionary<string, string> _localizedFR;

    public static bool isInit;

    public static CSVLoader csvLoader;
    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();
        
        UpdateDictionaries();
        
        isInit = true;
    }
    
    public static void UpdateDictionaries()
    {
        _localizedEN = csvLoader.GetDictionaryValues("en");
        _localizedFR = csvLoader.GetDictionaryValues("fr");
    }

    public static Dictionary<string, string> GetDictionaryForEditor()
    {
        if (!isInit)
        {
            Init();
        }

        return _localizedEN;
    }
    public static string GetLocalizedValue(string key)
    {
        if (!isInit)
        {
            Init();
        }

        string value = key;

        switch (language)
        {
            case Language.English:
                _localizedEN.TryGetValue(key, out value);
                break;
            case Language.French:
                _localizedFR.TryGetValue(key, out value);
                break;
        }

        return value;
    }

    public static void Add(string key, string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();;
        }
        
        csvLoader.LoadCSV();
        csvLoader.Add(key,value);
        csvLoader.LoadCSV();
        
        UpdateDictionaries();
    }

    public static void Replace(string key, string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();;
        }
        
        csvLoader.LoadCSV();
        csvLoader.Edit(key,value);
        csvLoader.LoadCSV();
        
        UpdateDictionaries();
    }

    public static void Remove(string key)
    {
        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();;
        }
        
        csvLoader.LoadCSV();
        csvLoader.Remove(key);
        csvLoader.LoadCSV();
        
        UpdateDictionaries();
    }
}
