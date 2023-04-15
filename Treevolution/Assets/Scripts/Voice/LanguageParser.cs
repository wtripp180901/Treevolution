using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

namespace LanguageParsing
{
    /// <summary>
    /// Entrypoint class for converting recorded dictation into a stream of BuddyActions
    /// </summary>
    public class LanguageParser
    {

        WordTokenMapper WordTokenMapper;
        ThesaurusAPICaller APICaller = new ThesaurusAPICaller("APIKey");
        ActionResolver actionResolver = new ActionResolver();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawBasewordData">The result of TextAsset.text from the TextAsset of basewords.txt via Resources.Load()</param>
        public LanguageParser(string rawBasewordData)
        {
            WordTokenMapper = new WordTokenMapper(rawBasewordData);
        }

        List<BuddyToken> tokenStream = new List<BuddyToken>();

        /// <summary>
        /// Converts recorded dictation into an ordered list of BuddyActions
        /// </summary>
        /// <param name="words">The dictation result separated by spaces</param>
        /// <param name="callback">A callback function to handle the resulting list of BuddyActions when they arrive</param>
        /// <returns></returns>
        public IEnumerator GetInstructionStream(string[] words, Action<List<BuddyAction>> callback)
        {

            foreach (string rawWord in words)
            {
                string word = sanitiseInput(rawWord);
                if (word.Length > 0)
                {
                    BuddyToken token;
                    if (WordTokenMapper.GetTokenIfMatched(word, out token))
                    {
                        //Word is known
                        if(token.tokenType != TOKEN_TYPES.Error) tokenStream.Add(token);
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
            callback(getInstructionsFromTokens());
        }


        /// <summary>
        /// Parses ordered list of BuddyTokens into BuddyActions
        /// </summary>
        /// <returns></returns>
        List<BuddyAction> getInstructionsFromTokens()
        {
            List<BuddyAction> instructions = new List<BuddyAction>();
            bool resolvingAction = false;
            bool connectiveMode = false;
            List<RESTRICTION_TYPES> currentRestrictions = new List<RESTRICTION_TYPES>();
            BUDDY_ACTION_TYPES actionToResolve = BUDDY_ACTION_TYPES.Error;
            BUDDY_SUBJECT_TYPES lastSubject = BUDDY_SUBJECT_TYPES.Unresolved;
            for (int i = 0; i < tokenStream.Count; i++)
            {
                BuddyToken token = tokenStream[i];
                switch (token.tokenType)
                {
                    case TOKEN_TYPES.Action:
                        if (resolvingAction)
                        {
                            resolveAction(actionToResolve, lastSubject, currentRestrictions,instructions);
                            currentRestrictions.Clear();
                        }
                        resolvingAction = true;
                        actionToResolve = ((ActionBuddyToken)token).actionType;
                        break;
                    case TOKEN_TYPES.Subject:
                        if (resolvingAction)
                        {
                            lastSubject = ((SubjectBuddyToken)token).subjectType;
                            /*BuddyAction resolvedAction = actionResolver.ResolveAction(actionToResolve, lastSubject, currentRestrictions.ToArray());
                            if(resolvedAction != null) instructions.Add(resolvedAction);
                            resolvingAction = false;*/
                        }
                        break;
                    case TOKEN_TYPES.Restriction:
                        currentRestrictions.Add(((RestrictionBuddyToken)token).restrictionType);
                        break;
                    case TOKEN_TYPES.Connective:
                        connectiveMode = true;
                        break;
                }
            }
            if (resolvingAction) resolveAction(actionToResolve, lastSubject, currentRestrictions, instructions);
            return instructions;
        }

        void resolveAction(BUDDY_ACTION_TYPES actionToResolve,BUDDY_SUBJECT_TYPES subject,List<RESTRICTION_TYPES> restrictions,List<BuddyAction> instructions)
        {
            BuddyAction action = actionResolver.ResolveAction(actionToResolve, subject, restrictions.ToArray());
            if(action != null) instructions.Add(action);
        }

        string sanitiseInput(string word)
        {
            word = word.ToLower();
            string output = "";
            foreach (char c in word)
            {
                if (c != '.' && c != ',' && c != '!' && c != '?')
                {
                    output = output + c;
                }
            }
            return output;
        }

        /// <summary>
        /// A callback function handling the results of an API call to a Thesaurus. Adds tokens to the current list if the synonyms contain any items from basewords.txt
        /// </summary>
        /// <param name="results">The synonyms identified by the API call</param>
        /// <returns></returns>
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
                        if(token.tokenType != TOKEN_TYPES.Error) tokenStream.Add(token);
                    }
                }
            }
            yield return null;
        }


        /// <summary>
        /// Helper function for synonymAPICallResults which initialises a dictionary of synonyms of a word organised by their functional labels
        /// </summary>
        /// <param name="homographSynonyms">The synonyms of one homograph of the word</param>
        /// <param name="synonymsOfFunctionalLabel">Dictionary to initialise</param>
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

}
