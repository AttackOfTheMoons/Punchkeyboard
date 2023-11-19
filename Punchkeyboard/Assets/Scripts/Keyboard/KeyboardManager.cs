using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        private GameObject[][] keyboardKeys;
        private Bounds[] originalRows;
        private int[] originalRowCount;
        
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
            originalRows = new Bounds[4];
            originalRowCount = new int[4];

            // Initialize an array of key names
            string[] row1KeyNames = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
            string[] row2KeyNames = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
            string[] row3KeyNames = { "Shift", "Z", "X", "C", "V", "B", "N", "M", "Backspace"};
            string[] row4KeyNames = { "Symbol", "Comma", "Space", "Period", "Return"};
            keyboardKeys[0] = FindKeysByName(row1KeyNames);
            keyboardKeys[1] = FindKeysByName(row2KeyNames);
            keyboardKeys[2] = FindKeysByName(row3KeyNames);
            keyboardKeys[3] = FindKeysByName(row4KeyNames);
            
            for (var row = 0; row < keyboardKeys.Length; row++)
            {
                originalRows[row] = new Bounds();
                foreach (var key in keyboardKeys[row])
                {
                    foreach (var child in key.GetComponentsInChildren<Renderer>())
                    {
                        originalRows[row].Encapsulate(child.bounds);
                    }

                    originalRowCount[row]++;
                }
            }
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
            Bounds[] activeBounds = new Bounds[keyboardKeys.Length];
            int[] activeRowCounts = new int[keyboardKeys.Length];
            // Make this read from left to right.
            for (var j = 0; j < keyboardKeys.Length; j++)
            {
                var row = keyboardKeys[j];
                activeBounds[j] = new Bounds();
                float totalZ = 0;
                var reversedSequence = ReverseSequence(refinementSequence);
                var lowerBound = 0;
                var upperBound = row.Length;

                while (reversedSequence > 0)
                {
                    var action = (Direction) (reversedSequence % 10);

                    switch (action)
                    {
                        case Direction.Left:
                            upperBound = (lowerBound + upperBound + 1) / 2;
                            break;
                        case Direction.Right:
                            lowerBound = (lowerBound + upperBound) / 2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    reversedSequence /= 10;
                }

                var activeRow = new List<GameObject>();
                // Hide keys outside the bounds determined by the refinement sequence
                for (var i = 0; i < row.Length; i++)
                {
                    if (i < lowerBound || i > upperBound || (i == upperBound && lowerBound != upperBound))
                    {
                        row[i].SetActive(false);
                    }
                    else
                    {
                        activeRowCounts[j]++;
                        foreach (var child in row[i].GetComponentsInChildren<Renderer>())
                        {
                            activeBounds[j].Encapsulate(child.bounds);
                        }
                        totalZ += row[i].transform.position.z;
                        row[i].SetActive(true);
                        activeRow.Add(row[i]);
                    }
                }

                var spacing = originalRows[j].size.x / activeRowCounts[j];
                var scaling = originalRows[j].size.x / activeBounds[j].size.x;
                var avgZ = totalZ / activeRowCounts[j];
                for (var i = 0; i < activeRow.Count; i++)
                {
                    var key = activeRow[i];
                    var pos = key.transform.position;
                    var rot = key.transform.rotation;
                    var localScale = key.transform.localScale;
                    rot.y /= 2;
                    pos.x = originalRows[j].min.x + spacing * (i + 0.5f);
                    for (var z = 0; z < keyboardKeys[j].Length; z++)
                    {
                        if (key.name != keyboardKeys[j][z].name) continue;
                        if (z + 1 < keyboardKeys[j].Length && keyboardKeys[j][z + 1].name == "Space")
                        {
                            pos.x -= spacing * (.25f);
                        } 
                        else if (z - 1 >= 0 && keyboardKeys[j][z - 1].name == "Space")
                        {
                            pos.x += spacing * (.25f);
                        }
                    }
                    pos.y = originalRows[j].max.y;
                    pos.z = avgZ;
                    localScale.x *= scaling;
                    key.transform.position = pos;
                    key.transform.rotation = rot;
                    key.transform.localScale = localScale;

                }
            }
        }
        
    }
}