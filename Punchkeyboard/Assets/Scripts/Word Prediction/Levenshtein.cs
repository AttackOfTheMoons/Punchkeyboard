using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Word_Prediction
{
    public class Levenshtein : MonoBehaviour
    {
        private const int MaxWordLength = 15;
        private const int MaxLevenshteinCost = 7;
        private const int MinLevenshteinCost = 1;
        [FormerlySerializedAs("NGramHandler")] public NGramGenerator nGramHandler;
        [FormerlySerializedAs("ButtonLabels")] public Text[] buttonLabels;
        private List<string> corpus = new();
        private bool isFirstLetterUpper;
        private bool isUppercase;

        private void Start()
        {
            corpus = nGramHandler.levenshteinCorpus;
            for (var i = 0; i < buttonLabels.Length; i++) buttonLabels[i].text = corpus[i];
        }

        public void RunAutoComplete(string input)
        {
            if (input.Length > 0)
            {
                var lastChar = input[^1..].ToCharArray();
                var lastWord = input.Split(' ').Last();
                var firstCharOfLastWord = lastWord[..].ToCharArray();
                if (firstCharOfLastWord.Length >= 1)
                {
                    isFirstLetterUpper = firstCharOfLastWord[0].ToString().Any(char.IsUpper);
                }

                if (char.IsWhiteSpace(lastChar[0])) return;
                if (lastWord.Length >= MaxWordLength) return;
                var dict = new Dictionary<int, int>();

                for (var i = 0; i < corpus.Count; i++)
                {
                    var cost = LevenshteinDistance.Compute(lastWord.ToLower(), corpus[i]);
                    if (cost is >= MinLevenshteinCost and <= MaxLevenshteinCost) dict.Add(i, cost);
                }

                if (lastWord.All(char.IsUpper)) isUppercase = true;
                if (lastWord.Any(char.IsLower)) isUppercase = false;

                var distanceOrder = dict.OrderBy(kp => kp.Value).Select(kp => kp.Key).ToList();

                for (var i = 0; i < distanceOrder.Count; i++)
                    if (i < buttonLabels.Length)
                    {
                        if (isUppercase)
                            buttonLabels[i].text = corpus[distanceOrder[i]].ToUpper();
                        else if (isFirstLetterUpper && isUppercase == false)
                            buttonLabels[i].text = char.ToUpper(corpus[distanceOrder[i]][0]) +
                                                   corpus[distanceOrder[i]].Substring(1);
                        else if (!isUppercase && isFirstLetterUpper == false)
                            buttonLabels[i].text = corpus[distanceOrder[i]].ToLower();
                    }
            }
        }

        private static class LevenshteinDistance
        {
            // Levenshtein distance computation from http://dotnetpearls.com/levenshtein <Not Active Circa 2023>.
            public static int Compute(string s, string t)
            {
                var n = s.Length;
                var m = t.Length;
                var d = new int[n + 1, m + 1];

                if (n == 0) return m;

                if (m == 0) return n;

                for (var i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (var j = 0; j <= m; d[0, j] = j++)
                {
                }

                for (var i = 1; i <= n; i++)
                for (var j = 1; j <= m; j++)
                {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }

                return d[n, m];
            }
        }
    }
}