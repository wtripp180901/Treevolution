using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class LanguageParser
{
    const string meta = "meta";
    const string fl = "fl";
    const string syns = "syns";

    WordActionMapper wordActionMapper;
    ThesaurusAPICaller APICaller = new ThesaurusAPICaller("APIKey");

    public LanguageParser(string rawBasewordData)
    {
        wordActionMapper = new WordActionMapper(rawBasewordData);
    }

    /*public List<BuddyAction> GetInstructionStream(JArray[] wordData)
    {
        List<BuddyAction> instructions = new List<BuddyAction>();
        for (int i = 0; i < wordData.Length; i++)
        {
            Dictionary<string, List<string>> synonymsOfFunctionalLabel = new Dictionary<string, List<string>>();
            getSynonymsForEachFunctionalLabel(wordData[i], synonymsOfFunctionalLabel);
            foreach(string key in synonymsOfFunctionalLabel.Keys)
            {
                switch (key)
                {
                    case "verb":
                        List<string> synonyms;
                        if(synonymsOfFunctionalLabel.TryGetValue(key,out synonyms))
                        {
                            if (synonyms.Contains("go")) instructions.Add(new BuddyAction(BUDDY_ACTION_TYPES.Move, new Vector3(0, 0, 0)));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        return instructions;
    }*/

    List<BuddyAction> instructions = new List<BuddyAction>();

    public IEnumerator GetInstructionStream(string[] words)
    {

        foreach(string rawWord in words)
        {
            string word = sanitiseInput(rawWord);
            if (word.Length > 0)
            {
                BUDDY_ACTION_TYPES actionType;
                if (wordActionMapper.GetActionIfCached(word, out actionType))
                {
                    //Word is known
                    instructions.Add(new BuddyAction(actionType, new Vector3(0, 0, 0)));
                    yield return null;
                }
                else
                {
                    //Word is unknown, test if any of it's synonyms match known words
                    yield return APICaller.GetSynonyms(word, synonymAPICallResults);
                    //Rest of exection defered to APICaller.GetSynonyms and synonymAPICallResults
                }
            }
            else
            {
                Debug.Log("strange character");
            }
        }
        string localResult = "";
        foreach(BuddyAction i in instructions)
        {
            localResult = localResult + i.actionType.ToString();
        }
        GameObject.FindWithTag("Logic").GetComponent<VoiceCommandReceiver>().HandleDictationProcessingResults(instructions);
    }

    string sanitiseInput(string word)
    {
        string output = "";
        foreach(char c in word)
        {
            if(c != '.' && c != ',' && c != '!' && c != '?')
            {
                output = output + c;
            }
        }
        return output;
    }
    public IEnumerator synonymAPICallResults(JArray results)
    {
        Debug.Log("hit");
        try
        {
            if (results != null)
            {
                Dictionary<string, List<string>> synonymsOfFunctionalLabel = new Dictionary<string, List<string>>();
                getSynonymsForEachFunctionalLabel(results, synonymsOfFunctionalLabel);
                foreach (string fl in synonymsOfFunctionalLabel.Keys)
                {
                    List<string> flWords;
                    synonymsOfFunctionalLabel.TryGetValue(fl, out flWords);
                    BUDDY_ACTION_TYPES currentActionType;
                    if (wordActionMapper.GetActionIfSynonymsMatchCachedWords(fl, flWords, out currentActionType))
                    {
                        instructions.Add(new BuddyAction(currentActionType, new Vector3(0, 0, 0)));
                    }
                }
            }
        }catch(Exception e)
        {
            Debug.Log("Result processing error: "+e.Message+"\n");
        }
        yield return null;
    }

    private void getSynonymsForEachFunctionalLabel(JArray word, Dictionary<string, List<string>> synonymsOfFunctionalLabel)
    {
        for (int homograph = 0; homograph < word.Count; homograph++)
        {
            List<string> synonyms;
            string functionalLabel = word[homograph][fl].ToString();
            if (synonymsOfFunctionalLabel.TryGetValue(functionalLabel, out synonyms))
            {
                synonyms.AddRange(word[homograph][meta][syns][0].ToObject<string[]>());
            }
            else
            {
                synonyms = new List<string>();
                synonyms.AddRange(word[homograph][meta][syns][0].ToObject<string[]>());
                synonymsOfFunctionalLabel.Add(functionalLabel, synonyms);
            }
            /*if (wordData[i][j]["fl"].ToString() == "noun")
            {
                GetComponent<UIController>().ShowDictation(wordData[i][j]["meta"]["id"] + " " + wordData[i][j]["meta"]["syns"][0].ToObject<string[]>()[0]);
            }*/
        }
    }
}
