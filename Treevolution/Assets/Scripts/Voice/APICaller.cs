using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using Newtonsoft.Json.Linq;

/*[Serializable]
public class SynonymsObject
{
    public object meta;
    public object hwi;
    public string fl;
    public object[] def;
    public string[] shortdef;
}*/

namespace LanguageParsing
{

    public struct SynonymData
    {
        public string partOfWord;
        public string[] synonyms;

        public SynonymData(string partOfWord, string[] synonyms)
        {
            this.partOfWord = partOfWord;
            this.synonyms = synonyms;
        }
    }

    class ThesaurusAPICaller
    {
        const string meta = "meta";
        const string fl = "fl";
        const string syns = "syns";

        string baseUrl = "https://www.dictionaryapi.com/api/v3/references/thesaurus/json/";
        string apiKey;
        public ThesaurusAPICaller(string keyFile)
        {
            try
            {
                apiKey = Resources.Load<TextAsset>(keyFile).text;
            }
            catch (Exception e)
            {
                Debug.Log("If you're seeing this error, ask Will for the API key!");
            }
        }

        public IEnumerator GetSynonyms(string word, Func<List<SynonymData>, IEnumerator> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(getUrl(word));
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                List<SynonymData> homographSynonyms = new List<SynonymData>();
                try
                {
                    JArray result = JArray.Parse(request.downloadHandler.text);
                    for (int homograph = 0; homograph < result.Count; homograph++)
                    {
                        homographSynonyms.Add(new SynonymData(result[homograph][fl].ToString(), result[homograph][meta][syns][0].ToObject<string[]>()));
                    }

                }
                catch (Exception e)
                {
                    Debug.Log("API parsing error: " + e.Message);
                }
                yield return callback(homographSynonyms);
            }
            else
            {
                Debug.Log(word + " web request failed: " + request.result);
                yield return callback(null);
            }
        }

        string getUrl(string word)
        {
            return baseUrl + word + "?key=" + apiKey;
        }
    }

}
