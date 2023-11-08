using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        private GameObject[][] keyboardKeys;
        
        
        private enum Direction
        {
            Left = 1,
            Right = 2
        }
        // refinementLevel is a base-10 integer represented as a "string" read from left to right.
        // Each digit in the "string" represents a specific action taken on the keyboard layout.
        // The rightmost digit represents the first action, and the leftmost digit represents the last action.
        // - '1' means "swipe left" to refine the keyboard (hide keys on the left).
        // - '2' means "swipe right" to refine the keyboard (hide keys on the right).
        private int refinementLevel;


        private void Awake()
        {
            keyboardKeys = new GameObject[4][];

            // Initialize an array of key names
            string[] row1KeyNames = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
            string[] row2KeyNames = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
            string[] row3KeyNames = { "Shift", "Z", "X", "C", "V", "B", "N", "M", "Backspace"};
            string[] row4KeyNames = { "Symbol", "Comma", "Space", "Period", "Return"};
            keyboardKeys[0] = FindKeysByName(row1KeyNames);
            keyboardKeys[1] = FindKeysByName(row2KeyNames);
            keyboardKeys[2] = FindKeysByName(row3KeyNames);
            keyboardKeys[3] = FindKeysByName(row4KeyNames);

            ResetRefinement();
        }

        private void OnDisable()
        {
            // ResetRefinement();
        }

        private static GameObject[] FindKeysByName(IReadOnlyList<string> keyNames)
        {
            var keys = new GameObject[keyNames.Count];

            for (var i = 0; i < keyNames.Count; i++)
            {
                keys[i] = GameObject.Find(keyNames[i]);
            }

            return keys;
        }

        public void ResetRefinement()
        {
            refinementLevel = 0;
            RefineKeyboard(0);
        }

        public void RefineLeft()
        {
            refinementLevel = 10 * refinementLevel + (int) Direction.Left;
            RefineKeyboard(refinementLevel);
        }

        public void RefineRight()
        {
            refinementLevel = 10 * refinementLevel + (int) Direction.Right;
            RefineKeyboard(refinementLevel);
        }
        
        private static int ReverseSequence(int sequence)
        {
            var reversed = 0;
            while (sequence > 0)
            {
                reversed = reversed * 10 + sequence % 10;
                sequence /= 10;
            }
            return reversed;
        }

       
        private void RefineKeyboard(int refinementSequence)
        {
            // Make this read from left to right.
            foreach (var row in keyboardKeys)
            {
                var reversedSequence = ReverseSequence(refinementSequence);
                var lowerBound = 0;
                var upperBound = row.Length;

                while (reversedSequence > 0)
                {
                    var action = (Direction) (reversedSequence % 10);

                    switch (action)
                    {
                        case Direction.Left:
                            upperBound = (lowerBound + upperBound) / 2;
                            break;
                        case Direction.Right:
                            lowerBound = (lowerBound + upperBound) / 2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    reversedSequence /= 10;
                }

                // Hide keys outside the bounds determined by the refinement sequence
                for (var i = 0; i < row.Length; i++)
                {
                    if (i < lowerBound || i >= upperBound)
                    {
                        row[i].SetActive(false);
                    }
                    else
                    {
                        row[i].SetActive(true);
                        // var scale = row[i].transform.localScale;
                        // scale.x = (refinementSequence != 0) ? 2 * ((float)Math.Log10(refinementSequence) + 1) * scale.x : 1;
                        // row[i].transform.localScale = scale;
                    }
                }
            }

        }
        
    }
}