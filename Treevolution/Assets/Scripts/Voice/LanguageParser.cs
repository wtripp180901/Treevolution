using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class LanguageParser
{

    WordTokenMapper WordTokenMapper;
    ThesaurusAPICaller APICaller = new ThesaurusAPICaller("APIKey");

    public LanguageParser(string rawBasewordData)
    {
        WordTokenMapper = new WordTokenMapper(rawBasewordData);
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

    List<BuddyToken> tokenStream = new List<BuddyToken>();

    public IEnumerator GetInstructionStream(string[] words)
    {

        foreach(string rawWord in words)
        {
            string word = sanitiseInput(rawWord);
            if (word.Length > 0)
            {
                BuddyToken token;
                if (WordTokenMapper.GetTokenIfMatched(word, out token))
                {
                    //Word is known
                    tokenStream.Add(token);
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
        GameObject.FindWithTag("Logic").GetComponent<VoiceCommandReceiver>().HandleDictationProcessingResults(getInstructionsFromTokens());
    }

    List<BuddyAction> getInstructionsFromTokens()
    {
        List<BuddyAction> instructions = new List<BuddyAction>();
        bool resolvingAction = false;
        bool connectiveMode = false;
        BUDDY_ACTION_TYPES actionToResolve= BUDDY_ACTION_TYPES.Error;
        BUDDY_SUBJECT_TYPES lastSubject = BUDDY_SUBJECT_TYPES.Error;
        foreach(BuddyToken token in tokenStream)
        {
            switch (token.tokenType)
            {
                case TOKEN_TYPES.Action:
                    if (resolvingAction)
                    {
                        //Handle unresolved tokens
                    }
                    resolvingAction = true;
                    actionToResolve = ((ActionBuddyToken)token).actionType;
                    break;
                case TOKEN_TYPES.Subject:
                    if (resolvingAction)
                    {
                        lastSubject = BUDDY_SUBJECT_TYPES.PointerLocation;
                        instructions.Add(new BuddyAction(actionToResolve, getSubject(lastSubject)));
                        resolvingAction = false;
                    }
                    break;
                case TOKEN_TYPES.Connective:
                    connectiveMode = true;
                    break;
            }
        }
        return instructions;
    }

    Vector3 getSubject(BUDDY_SUBJECT_TYPES subjectType)
    {
        switch (subjectType)
        {
            case BUDDY_SUBJECT_TYPES.PointerLocation:
                Vector3 pointer = GameObject.FindWithTag("Logic").GetComponent<PointerLocationTracker>().pointer.transform.position;
                GameObject.FindWithTag("MoveToMarker").transform.position = pointer;
                return pointer;
        }
        return new Vector3(0,0,0);
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
    public IEnumerator synonymAPICallResults(List<SynonymData> results)
    {
        if (results != null && results.Count > 0)
        {
            Dictionary<string, List<string>> synonymsOfFunctionalLabel = new Dictionary<string, List<string>>();
            getSynonymsForEachFunctionalLabel(results, synonymsOfFunctionalLabel);
            foreach (string fl in synonymsOfFunctionalLabel.Keys)
            {
                List<string> flWords;
                synonymsOfFunctionalLabel.TryGetValue(fl, out flWords);
                BuddyToken token;
                if (WordTokenMapper.GetTokenIfSynonymsMatchCachedWords(fl, flWords, out token))
                {
                    tokenStream.Add(token);
                }
            }
        }
        yield return null;
    }

    private void getSynonymsForEachFunctionalLabel(List<SynonymData> homographSynonyms, Dictionary<string, List<string>> synonymsOfFunctionalLabel)
    {
        for (int homograph = 0; homograph < homographSynonyms.Count; homograph++)
        {
            List<string> synonyms;
            string partOfWord = homographSynonyms[homograph].partOfWord;
            if (synonymsOfFunctionalLabel.TryGetValue(partOfWord, out synonyms))
            {
                synonyms.AddRange(homographSynonyms[homograph].synonyms);
            }
            else
            {
                synonyms = new List<string>();
                synonyms.AddRange(homographSynonyms[homograph].synonyms);
                synonymsOfFunctionalLabel.Add(partOfWord, synonyms);
            }
            /*if (wordData[i][j]["fl"].ToString() == "noun")
            {
                GetComponent<UIController>().ShowDictation(wordData[i][j]["meta"]["id"] + " " + wordData[i][j]["meta"]["syns"][0].ToObject<string[]>()[0]);
            }*/
        }
    }
}
