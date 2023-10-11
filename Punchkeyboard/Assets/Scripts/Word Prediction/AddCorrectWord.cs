using UnityEngine;
using UnityEngine.UI;

namespace Word_Prediction
{
    public class AddCorrectWord : MonoBehaviour
    {
        private AutocompleteWordPicker wordPicker;

        private void Start()
        {
            wordPicker = gameObject.GetComponentInParent<AutocompleteWordPicker>();
        }

        public void WordChosen()
        {
            wordPicker.ReplaceWord(gameObject.GetComponentInChildren<Text>().text);
        }
    }
}