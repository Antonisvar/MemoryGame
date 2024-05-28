using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    // This scriptable object is used to store the data for the interaction input.
    [CreateAssetMenu(fileName = "InteractionInputData", menuName = "InteractionSystem/InputData")]
    public class InteractionInputData : ScriptableObject
    {
        // Variables to store the interaction input data.
        private bool m_interactedClicked;
        private bool m_interactedRelease;
        // Getters and Setters for the interaction input data.
        public bool InteractedClicked
        {
            get => m_interactedClicked;
            set => m_interactedClicked = value;
        }

        public bool InteractRealease
        {
            get => m_interactedRelease;
            set => m_interactedRelease = value;
        }
        // Reset the input data.
        public void ResetInput()
        {
            m_interactedClicked = false;
            m_interactedRelease = false;
        }

    }
}