using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordTokenMapper
{
    Dictionary<string, Dictionary<BuddyToken, List<string>>> flCategoriesOfActions = new Dictionary<string, Dictionary<BuddyToken, List<string>>>();
    
    public WordTokenMapper(string rawBasewordData)
    {
        LoadCachedWords(rawBasewordData);
    }

    /// <summary>
    /// Initialises flCategoriesOfActions by reading data from local file
    /// </summary>
    /// <param name="rawData">The data read from the basewords.txt file via a call to Resources.Load</param>
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
            string tokenType = data[i + 1];
            string[] wordList = data[i + 2].Split(' ');

            Dictionary<BuddyToken, List<string>> actionDictionary;
            if (flCategoriesOfActions.TryGetValue(fl, out actionDictionary))
            {
                List<string> cachedWords;
                if (actionDictionary.TryGetValue(stringToToken(tokenType),out cachedWords))
                {
                    cachedWords.AddRange(wordList);
                }
                else
                {
                    actionDictionary.Add(stringToToken(tokenType), new List<string>(wordList));
                }
            }
            else
            {
                actionDictionary = new Dictionary<BuddyToken, List<string>>()
                {
                    {stringToToken(tokenType), new List<string>(wordList) }
                };
                flCategoriesOfActions.Add(fl, actionDictionary);
            }
        }
    }

    BuddyToken stringToToken(string toConvert)
    {
        switch (toConvert)
        {
            case "Move":
                return new ActionBuddyToken(BUDDY_ACTION_TYPES.Move);
            case "Attack":
                return new ActionBuddyToken(BUDDY_ACTION_TYPES.Attack);
            case "PointerLocation":
                return new SubjectBuddyToken(BUDDY_SUBJECT_TYPES.PointerLocation);
            case "Connective":
                return new ConnectiveBuddyToken();
            default:
                Debug.Log(toConvert + " not mapped in stringToToken");
                return null;
        }
    }

    public bool GetTokenIfMatched(string word, out BuddyToken token)
    {
        foreach(Dictionary<BuddyToken, List<string>> fl in flCategoriesOfActions.Values)
        {
            foreach(BuddyToken keyToken in fl.Keys)
            {
                List<string> wordsOfActionType;
                if(fl.TryGetValue(keyToken,out wordsOfActionType))
                {
                    if (wordsOfActionType.Contains(word))
                    {
                        token = keyToken;
                        return true;
                    }
                }
            }
        }
        token = null;
        return false;
    }

    public bool GetTokenIfSynonymsMatchCachedWords(string functionalLabel,List<string> synonyms,out BuddyToken token)
    {
        Dictionary<BuddyToken, List<string>> wordsOfActionTypes;
        if(flCategoriesOfActions.TryGetValue(functionalLabel,out wordsOfActionTypes))
        {
            foreach(BuddyToken tokenType in wordsOfActionTypes.Keys)
            {
                List<string> knownWords;
                wordsOfActionTypes.TryGetValue(tokenType, out knownWords);
                if (synonyms.Intersect(knownWords).Any())
                {
                    token = tokenType;
                    return true;
                }
            }
        }
        token = null;
        return false;
    }
}
