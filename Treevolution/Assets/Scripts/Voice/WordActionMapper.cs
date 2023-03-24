using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordActionMapper
{
    Dictionary<string, Dictionary<BUDDY_ACTION_TYPES, List<string>>> flCategoriesOfActions = new Dictionary<string, Dictionary<BUDDY_ACTION_TYPES, List<string>>>();
    
    public WordActionMapper(string rawBasewordData)
    {
        LoadCachedWords(rawBasewordData);
    }
    private void LoadCachedWords(string rawData)
    {
        string[] data = rawData.Split('\n');

        //Truncating random characters added to end of read data
        for(int i = 0;i < data.Length - 1; i++)
        {
            data[i] = data[i].Substring(0, data[i].Length - 1);
        }

        for(int i = 0;i < data.Length;i += 3)
        {
            string fl = data[i];
            string actionType = data[i + 1];
            string[] wordList = data[i + 2].Split(' ');

            Dictionary<BUDDY_ACTION_TYPES, List<string>> actionDictionary;
            if (flCategoriesOfActions.TryGetValue(fl, out actionDictionary))
            {
                List<string> cachedWords;
                if (actionDictionary.TryGetValue(stringToAction(actionType),out cachedWords))
                {
                    cachedWords.AddRange(wordList);
                }
                else
                {
                    actionDictionary.Add(stringToAction(actionType), new List<string>(wordList));
                }
            }
            else
            {
                actionDictionary = new Dictionary<BUDDY_ACTION_TYPES, List<string>>()
                {
                    {stringToAction(actionType), new List<string>(wordList) }
                };
                flCategoriesOfActions.Add(fl, actionDictionary);
            }
        }
    }

    BUDDY_ACTION_TYPES stringToAction(string toConvert)
    {
        switch (toConvert)
        {
            case "Move":
                return BUDDY_ACTION_TYPES.Move;
            case "Attack":
                return BUDDY_ACTION_TYPES.Attack;
            default:
                Debug.Log(toConvert + " not mapped in stringToAction");
                return BUDDY_ACTION_TYPES.Error;
        }
    }

    public bool GetActionIfCached(string word, out BUDDY_ACTION_TYPES actionType)
    {
        foreach(Dictionary<BUDDY_ACTION_TYPES, List<string>> fl in flCategoriesOfActions.Values)
        {
            foreach(BUDDY_ACTION_TYPES keyType in fl.Keys)
            {
                List<string> wordsOfActionType;
                if(fl.TryGetValue(keyType,out wordsOfActionType))
                {
                    if (wordsOfActionType.Contains(word))
                    {
                        actionType = keyType;
                        return true;
                    }
                }
            }
        }
        actionType = BUDDY_ACTION_TYPES.Error;
        return false;
    }

    public bool GetActionIfSynonymsMatchCachedWords(string functionalLabel,List<string> synonyms,out BUDDY_ACTION_TYPES actionType)
    {
        Dictionary<BUDDY_ACTION_TYPES, List<string>> wordsOfActionTypes;
        if(flCategoriesOfActions.TryGetValue(functionalLabel,out wordsOfActionTypes))
        {
            foreach(BUDDY_ACTION_TYPES keyType in wordsOfActionTypes.Keys)
            {
                List<string> knownWords;
                wordsOfActionTypes.TryGetValue(keyType, out knownWords);
                if (synonyms.Intersect(knownWords).Any())
                {
                    actionType = keyType;
                    return true;
                }
            }
        }
        actionType = BUDDY_ACTION_TYPES.Error;
        return false;
    }
}
