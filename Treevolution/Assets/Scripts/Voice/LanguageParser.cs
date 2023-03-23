using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LanguageParser
{
    const string meta = "meta";
    const string fl = "fl";
    const string syns = "syns";

    WordActionMapper wordActionMapper = new WordActionMapper();
    ThesaurusAPICaller APICaller = new ThesaurusAPICaller("APIKey");

    public List<BuddyAction> GetInstructionStream(JArray[] wordData)
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
    }

    public IEnumerator GetInstructionStream(string[] words)
    {
        List<BuddyAction> instructions = new List<BuddyAction>();

        foreach(string word in words)
        {
            BUDDY_ACTION_TYPES actionType;
            if(wordActionMapper.GetActionIfCached(word,out actionType))
            {
                //Word is known
                instructions.Add(new BuddyAction(actionType, new Vector3(0, 0, 0)));
                yield return null;
            }
            else
            {
                //Word is unknown, test if any of it's synonyms match known words
                JArray synonymData = APICaller.GetSynonyms(word);
                if (synonymData != null)
                {
                    Dictionary<string, List<string>> synonymsOfFunctionalLabel = new Dictionary<string, List<string>>();
                    getSynonymsForEachFunctionalLabel(synonymData, synonymsOfFunctionalLabel);
                    foreach(string fl in synonymsOfFunctionalLabel.Keys)
                    {
                        List<string> flWords;
                        synonymsOfFunctionalLabel.TryGetValue(fl, out flWords);
                        BUDDY_ACTION_TYPES currentActionType;
                        if(wordActionMapper.GetActionIfSynonymsMatchCachedWords(fl,flWords,out currentActionType))
                        {
                            instructions.Add(new BuddyAction(currentActionType, new Vector3(0, 0, 0)));
                        }
                    }
                }
                yield return null;
            }
        }
        //GameObject.FindWithTag("Logic").Ge
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
