using Keyboard;
using UnityEngine;

namespace VR.InputSelector
{
    public class InputChoice : InputKey
    {
        [SerializeField] public InputMethod inputMethod;

        private InputSelector inputSelector;

        private new void Start()
        {
            base.Start();
            inputSelector = transform.parent.root.gameObject.GetComponent<InputSelector>();
        }

        protected override void Hit()
        {
            inputSelector.SetInputMethod(inputMethod);
        }
    }
}