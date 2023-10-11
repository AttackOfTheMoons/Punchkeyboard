using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//using UnityEditor;

namespace Word_Prediction
{
    public class NGramGenerator : MonoBehaviour
    {
        [FormerlySerializedAs("ButtonLabels")] public Text[] buttonLabels;
        [FormerlySerializedAs("LevenshteinCorpus")] public List<string> levenshteinCorpus = new();

        private Dictionary<string, int> biGramDict = new();
        private readonly List<string> biGramPredictionCorpus = new();
        private Dictionary<string, int> levenshteinDict = new();


        private void Awake()
        {
            // Uncomment this when working in the Unity Editor or with new dictionaries
            // /*
            var directoryPath = Application.dataPath + "/Resources/WordPrediction";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if (!File.Exists(directoryPath + "/biGramDict.txt") || !File.Exists(directoryPath + "/levenshteinDict.txt"))
            {
                Debug.Log("No dictionaries found, building a new one. This can take a while depending on the corpus size.");
                var loadedCorpus = Resources.Load("Sample") as TextAsset;
                var stringsCorpus = loadedCorpus.ToString();

                GenerateBiGrams(stringsCorpus);
                GenerateLevenshteinDict(stringsCorpus);

                levenshteinCorpus = levenshteinDict.Keys.ToList();
                Debug.Log("Dictionaries were successfully generated.");
            }
            else
            {
                Debug.Log("Dictionaries found, word prediction is running.");
                biGramDict = LoadDictionary("WordPrediction/biGramDict");
                levenshteinDict = LoadDictionary("WordPrediction/levenshteinDict");

                levenshteinCorpus = levenshteinDict.Keys.ToList();
            }
            // */

            biGramDict = LoadDictionary("WordPrediction/biGramDict");
            levenshteinDict = LoadDictionary("WordPrediction/levenshteinDict");

            levenshteinCorpus = levenshteinDict.Keys.ToList();
        }

        private Dictionary<string, int> OrderDictionary(string filePath)
        {
            var loadedDict = Resources.Load(filePath) as TextAsset;
            var stringDict = loadedDict.ToString();
            var dict = GetDict(stringDict);
            var orderedEnum = from entry in dict orderby entry.Value descending select entry;
            var orderedDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

            return orderedDict;
        }

        private Dictionary<string, int> LoadDictionary(string filePath)
        {
            var loadedDict = Resources.Load(filePath) as TextAsset;
            var stringDict = loadedDict.ToString();
            var dict = GetDict(stringDict);

            return dict;
        }

        private void GenerateBiGrams(string corpus)
        {
            var nGrams = MakeNgrams(corpus, 2);

            var enumerable = nGrams as string[] ?? nGrams.ToArray();
            for (var i = 0; i < enumerable.Count(); i++)
                if (biGramDict.ContainsKey(enumerable.ElementAt(i)))
                    biGramDict[enumerable.ElementAt(i)] += 1;
                else
                    biGramDict.Add(enumerable.ElementAt(i), 1);
            var orderedEnum = from entry in biGramDict orderby entry.Value descending select entry;
            biGramDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

            var s = GetLine(biGramDict);
            File.WriteAllText(Application.dataPath + "/Resources/AutoCorrect/biGramDict.txt", s);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        private void GenerateLevenshteinDict(string corpus)
        {
            var wordPattern = new Regex("[\\w']+");

            foreach (Match match in wordPattern.Matches(corpus))
            {
                levenshteinDict.TryGetValue(match.Value, out var currentCount);

                currentCount++;
                levenshteinDict[match.Value] = currentCount;
            }

            var orderedEnum = from entry in levenshteinDict orderby entry.Value descending select entry;
            levenshteinDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

            var s = GetLine(levenshteinDict);
            File.WriteAllText(Application.dataPath + "/Resources/AutoCorrect/levenshteinDict.txt", s);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public void PredictNextWords(string input)
        {
            foreach (var kvp in biGramDict)
                if (kvp.Key.Contains(input.ToLower() + " "))
                    biGramPredictionCorpus.Add(kvp.Key.Split(' ')[1]);

            if (biGramPredictionCorpus.Count < buttonLabels.Length)
            {
                for (var i = 0; i < biGramPredictionCorpus.Count; i++) buttonLabels[i].text = biGramPredictionCorpus[i];
                for (var i = biGramPredictionCorpus.Count; i < buttonLabels.Length; i++)
                    //Don't forget to filter repeating stuff like "to" "to" etc.
                    buttonLabels[i].text = levenshteinCorpus[i - biGramPredictionCorpus.Count];
            }
            else
            {
                for (var i = 0; i < buttonLabels.Length; i++) buttonLabels[i].text = biGramPredictionCorpus[i];
            }

            biGramPredictionCorpus.Clear();
        }

        // N-gram creator by Jake Drew bit.ly/N-grams
        private static IEnumerable<string> MakeNgrams(string text, byte nGramSize)
        {
            var nGram = new StringBuilder();
            var wordLengths = new Queue<int>();
            var wordCount = 0;
            var lastWordLen = 0;

            if (text != "" && char.IsLetterOrDigit(text[0]))
            {
                nGram.Append(text[0]);
                lastWordLen++;
            }

            for (var i = 1; i < text.Length - 1; i++)
            {
                var before = text[i - 1];
                var after = text[i + 1];

                if (char.IsLetterOrDigit(text[i]) || (text[i] != ' '
                                                      && (char.IsSeparator(text[i]) || char.IsPunctuation(text[i]))
                                                      && char.IsLetterOrDigit(before) && char.IsLetterOrDigit(after)))
                {
                    nGram.Append(text[i]);
                    lastWordLen++;
                }
                else if (lastWordLen > 0)
                {
                    wordLengths.Enqueue(lastWordLen);
                    lastWordLen = 0;
                    wordCount++;

                    if (wordCount >= nGramSize)
                    {
                        yield return nGram.ToString();
                        nGram.Remove(0, wordLengths.Dequeue() + 1);
                        wordCount -= 1;
                    }

                    nGram.Append(" ");
                }
            }
        }

        private string GetLine(Dictionary<string, int> d)
        {
            var builder = new StringBuilder();
            foreach (var pair in d) builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            var result = builder.ToString();
            result = result.TrimEnd(',');
            return result;
        }

        private static Dictionary<string, int> GetDict(string s)
        {
            var d = new Dictionary<string, int>();
            var tokens = s.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i += 2)
            {
                var token = tokens[i];
                var freq = tokens[i + 1];

                var count = int.Parse(freq);
                if (d.ContainsKey(token))
                    d[token] += count;
                else
                    d.Add(token, count);
            }

            return d;
        }
    }
}