using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        private GameObject[][] keyboardKeys;
        private readonly Dictionary<GameObject[], Bounds> originalRowBounds = new();
        
        private readonly Dictionary<GameObject, Vector3> originalPositions  = new();
        private readonly Dictionary<GameObject, Vector3> originalScales = new();
        
        private readonly Dictionary<GameObject, Vector3> targetPositions = new();
        private readonly Dictionary<GameObject, Vector3> targetScales = new();

        private readonly Dictionary<GameObject, Renderer[]> objectRenderers = new();
        private const float InterpolationSpeed = 5f;
        private const float SpaceSpacing = .3f;

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

        private void Update()
        {
            // Interpolate positions and scales during each frame update
            foreach (var (key, targetPosition) in targetPositions)
            {
                key.transform.position = Vector3.Lerp(key.transform.position, targetPosition, Time.deltaTime * InterpolationSpeed);
            }

            foreach (var (key, targetScale) in targetScales)
            {
                key.transform.localScale = Vector3.Lerp(key.transform.localScale, targetScale, Time.deltaTime * InterpolationSpeed);
            }
        }

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
            
            foreach (var row in keyboardKeys)
            {
                originalRowBounds[row] = new Bounds();
                foreach (var key in row)
                {
                    originalPositions[key] = key.transform.position;
                    originalScales[key] = key.transform.localScale;
                    objectRenderers[key] = key.GetComponentsInChildren<Renderer>();
                    foreach (var child in objectRenderers[key])
                    {
                        originalRowBounds[row].Encapsulate(child.bounds);
                    }
                }
            }
            ResetRefinement();
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
            RefineKeyboard();
            foreach (var row in keyboardKeys)
            {
                foreach (var key in row)
                {
                    var keyTransform = key.transform;
                    targetPositions[key] = originalPositions[key];
                    targetScales[key] = originalScales[key];
                    // Comment these out to make the transition smooth, but have issue of disabling the keyboard while moving.
                    keyTransform.position = originalPositions[key]; 
                    keyTransform.localScale = originalScales[key];
                }
            }
        }

        public void RefineLeft()
        {
            refinementLevel = 10 * refinementLevel + (int) Direction.Left;
            RefineKeyboard();
        }

        public void RefineRight()
        {
            refinementLevel = 10 * refinementLevel + (int) Direction.Right;
            RefineKeyboard();
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

       
        private void RefineKeyboard()
        {
            Bounds[] activeBounds = new Bounds[keyboardKeys.Length];
            int[] activeRowCounts = new int[keyboardKeys.Length];
            // Make this read from left to right.
            for (var j = 0; j < keyboardKeys.Length; j++)
            {
                var row = keyboardKeys[j];
                activeBounds[j] = new Bounds();
                float totalZ = 0;
                var reversedSequence = ReverseSequence(refinementLevel);
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
                for (var keyIndex = 0; keyIndex < row.Length; keyIndex++)
                {
                    var key = row[keyIndex];
                    if (keyIndex < lowerBound || keyIndex > upperBound || (keyIndex == upperBound && lowerBound != upperBound))
                    {
                        key.SetActive(false);
                    }
                    else
                    {
                        activeRowCounts[j]++;
                        foreach (var child in objectRenderers[key])
                        {
                            activeBounds[j].Encapsulate(child.bounds);
                        }
                        totalZ += key.transform.position.z;
                        key.SetActive(true);
                        activeRow.Add(key);
                    }
                }

                var spacing = originalRowBounds[row].size.x / activeRowCounts[j];
                var scaling = originalRowBounds[row].size.x / activeBounds[j].size.x;
                var avgZ = totalZ / activeRowCounts[j];
                for (var i = 0; i < activeRow.Count; i++)
                {
                    var key = activeRow[i];
                    var pos = key.transform.position;
                    var rot = key.transform.rotation;
                    var localScale = key.transform.localScale;
                    rot.y /= 2;
                    pos.x = originalRowBounds[row].min.x + spacing * (i + 0.5f);
                    for (var z = 0; z < keyboardKeys[j].Length; z++)
                    {
                        if (key.name != keyboardKeys[j][z].name) continue;
                        if (z + 1 < keyboardKeys[j].Length && keyboardKeys[j][z + 1].name == "Space")
                        {
                            pos.x -= spacing * SpaceSpacing;
                        } 
                        else if (z - 1 >= 0 && keyboardKeys[j][z - 1].name == "Space")
                        {
                            pos.x += spacing * SpaceSpacing;
                        }
                    }
                    pos.y = originalRowBounds[row].max.y;
                    pos.z = avgZ;
                    localScale.x *= scaling;
                    targetPositions[key] = pos;
                    targetScales[key] = localScale;
                    key.transform.rotation = rot;
                    // Replaced in favor of interpolation.
                    // key.transform.localScale = localScale;
                    // key.transform.position = pos;
                }
            }
        }
        
    }
}