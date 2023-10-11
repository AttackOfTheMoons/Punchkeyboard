using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Word_Prediction
{
    public class AutocompleteWordPicker : MonoBehaviour
    {
        [FormerlySerializedAs("TextField")] public InputField textField;
        [FormerlySerializedAs("WordPredictor")] public NGramGenerator wordPredictor;

        public void ReplaceWord(string correctWord)
        {
            var builder = new StringBuilder();
            var input = textField.text;
            var parts = input.Split(' ');
            parts = parts.Take(parts.Length - 1).ToArray();

            var inputText = parts.ToList();

            inputText.Add(correctWord);

            foreach (var w in inputText) builder.Append(w).Append(" ");
            textField.text = builder.ToString();
            textField.ActivateInputField();

            wordPredictor.PredictNextWords(correctWord);
        }

        public static string ReverseString(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}