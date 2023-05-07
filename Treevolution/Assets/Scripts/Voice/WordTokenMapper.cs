using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BuddyActions;
using Tokens;

namespace LanguageParsing
{
    /// <summary>
    /// Converts words from recorded dictation into tokens for interpretation by LanguageParser
    /// </summary>
    class WordTokenMapper
    {
        Dictionary<string, Dictionary<BuddyToken, List<string>>> flCategoriesOfActions = new Dictionary<string, Dictionary<BuddyToken, List<string>>>();
        Dictionary<BuddyToken, List<string>> tokensForPluralNouns = new Dictionary<BuddyToken, List<string>>();

        const string nounFunctionalLabel = "noun";

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
            for (int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i].Substring(0, data[i].Length - 1);
            }

            for (int i = 0; i < data.Length; i += 4)
            {
                string fl = data[i];
                string tokenType = data[i + 1];
                string[] wordList = data[i + 2].Split(' ');

                Dictionary<BuddyToken, List<string>> actionDictionary;
                if (flCategoriesOfActions.TryGetValue(fl, out actionDictionary))
                {
                    List<string> cachedWords;
                    if (actionDictionary.TryGetValue(stringToToken(tokenType), out cachedWords))
                    {
                        cachedWords.AddRange(wordList);
                        if (fl == nounFunctionalLabel && tokensForPluralNouns.TryGetValue(stringToToken(tokenType),out cachedWords))
                        {
                            cachedWords.AddRange(wordList.Select(x => pluralize(x)).ToList());
                        }
                    }
                    else
                    {
                        actionDictionary.Add(stringToToken(tokenType), new List<string>(wordList));
                        if (fl == nounFunctionalLabel) tokensForPluralNouns.Add(stringToToken(tokenType), wordList.Select(x => pluralize(x)).ToList());
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

        /// <summary>
        /// Maps words from basewords.txt to tokens to be used by the language parser
        /// </summary>
        /// <param name="toConvert">The string to be mapped to a token. Must be contained within basewords.txt</param>
        /// <returns>The resolved token, returns a DummyToken if unresolved</returns>
        BuddyToken stringToToken(string toConvert)
        {
            switch (toConvert)
            {
                case "Move":
                    return new ActionBuddyToken(BUDDY_ACTION_TYPES.Move);
                case "Attack":
                    return new ActionBuddyToken(BUDDY_ACTION_TYPES.Attack);
                case "Repair":
                    return new ActionBuddyToken(BUDDY_ACTION_TYPES.Repair);
                case "Defend":
                    return new ActionBuddyToken(BUDDY_ACTION_TYPES.Defend);
                case "Buff":
                    return new ActionBuddyToken(BUDDY_ACTION_TYPES.Buff);
                case "PointerLocation":
                    return new SubjectBuddyToken(BUDDY_SUBJECT_TYPES.PointerLocation);
                case "Connective":
                    return new ConnectiveBuddyToken();
                case "Objective Singular Pronoun":
                case "Subject Singular Pronoun":
                    return new SubjectBuddyToken(BUDDY_SUBJECT_TYPES.SingleClosestToPointer);
                case "Objective Plural Pronoun":
                case "Subject Plural Pronoun":
                    return new SubjectBuddyToken(BUDDY_SUBJECT_TYPES.GroupCloseToPointer);
                case "Wall":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Wall);
                case "Plant":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Plant);
                case "Cactus":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Cactus);
                case "Mushroom":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Mushroom);
                case "Poison":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Poisonous);
                case "Flower":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Flower);
                case "Ant":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Ant);
                case "Hornet":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Hornet);
                case "Stag Beetle":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Stagbeetle);
                case "Cockroach":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Cockroach);
                case "Tree":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Tree);
                case "Dragonfly":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Dragonfly);
                case "Flying":
                case "In Air":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Flying);
                case "Armoured":
                case "Has Armour":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Armoured);
                case "Unarmoured":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Unarmoured);
                case "Big":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Large);
                case "Small":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Small);
                case "Grounded":
                case "On Ground":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Grounded);
                case "Trap":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Trap);
                case "Black":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Black);
                case "Blue":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Blue);
                case "Silver":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Silver);
                case "Brown":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Brown);
                case "Yellow":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Yellow);
                case "Green":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Green);
                case "Red":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Red);
                case "Tall":
                    return new RestrictionBuddyToken(RESTRICTION_TYPES.Tall);
                case "Ignore":
                    return new DummyToken();
                default:
                    Debug.Log(toConvert + " not mapped in stringToToken");
                    return new DummyToken();
            }
        }


        /// <summary>
        /// Checks if a word is stored locally in basewords.txt and returns the appropriate token if it is
        /// </summary>
        /// <param name="word">The word to match</param>
        /// <param name="token">Will be written to with appropriate token if word is stored locally</param>
        /// <returns>True if word matches one stored locally, indicating that token has a result written</returns>
        public bool GetTokenIfMatched(string word, out BuddyToken token)
        {
            foreach (Dictionary<BuddyToken, List<string>> fl in flCategoriesOfActions.Values)
            {
                if (tokenDictionaryContainsWordOrSynonyms(fl, word, out token)) return true;
            }
            if (tokenDictionaryContainsWordOrSynonyms(tokensForPluralNouns, word, out token)) return true;
            token = null;
            return false;
        }

        /// <summary>
        /// Checks if a list of synonyms match any words stored locally in basewords.txt. If they do, returns true and gives the corresponding token of that words
        /// </summary>
        /// <param name="functionalLabel">The functional label of the synonyms</param>
        /// <param name="synonyms">A list of synonyms to match ot the cache</param>
        /// <param name="token">If true, the appropriate buddy token is written to this</param>
        /// <returns>True if synoynms match any cached words, indicating that token has a result written</returns>
        public bool GetTokenIfSynonymsMatchCachedWords(string functionalLabel, List<string> synonyms, out BuddyToken token)
        {
            Dictionary<BuddyToken, List<string>> wordsOfActionTypes;
            if (functionalLabel == nounFunctionalLabel)
            {
                if (tokenDictionaryContainsWordOrSynonyms(tokensForPluralNouns, synonyms, out token)) return true;
            }
            if (flCategoriesOfActions.TryGetValue(functionalLabel, out wordsOfActionTypes))
            {
                if (tokenDictionaryContainsWordOrSynonyms(wordsOfActionTypes, synonyms, out token)) return true;
            }
            token = null;
            return false;
        }

        bool tokenDictionaryContainsWordOrSynonyms(Dictionary<BuddyToken, List<string>> dictionary, string word, out BuddyToken token)
        {
            return tokenDictionaryContainsWordOrSynonyms(dictionary, new List<string>() { word }, out token);
        }
        /// <summary>
        /// If any of the list of synonyms are known to the model (true), will give a corresponding BuddyToken
        /// </summary>
        /// <param name="dictionary">The dictionary of known words</param>
        /// <param name="synonyms">The list of synonyms to be checked against the dictionary</param>
        /// <param name="token">If the functions evaluates to true, the corresponding token will be written here. Null if false</param>
        /// <returns>Returns true if the dictionary contains any of the listed synonyms and the BuddyToken has been written</returns>
        bool tokenDictionaryContainsWordOrSynonyms(Dictionary<BuddyToken,List<string>> dictionary,List<string> synonyms,out BuddyToken token)
        {
            foreach (BuddyToken tokenType in dictionary.Keys)
            {
                List<string> knownWords;
                dictionary.TryGetValue(tokenType, out knownWords);
                if (synonyms.Intersect(knownWords).Any())
                {
                    token = tokenType;
                    return true;
                }
            }
            token = null;
            return false;
        }

        /*bool containsOrContainsPlural(string word,List<string> list,out bool isPlural)
        {
            isPlural = false;
            if (list.Contains(word)) return true;
            List<string> pluralList = list.Select
        }*/

        /// <summary>
        /// Converts nouns into their plural form
        /// </summary>
        /// <param name="original">The singular noun</param>
        /// <returns>The pluralised version of the noun</returns>
        string pluralize(string original)
        {
            int lastCharIndex = original.Length - 1;
            string truncated = original.Substring(0, original.Length - 1);
            switch (original[lastCharIndex])
            {
                case 'h':
                    return original + "es";
                case 'y':
                    return truncated + "ies";
                default:
                    return original + "s";
            }
        }
    }
}