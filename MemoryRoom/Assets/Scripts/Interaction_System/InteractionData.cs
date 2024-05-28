using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    [CreateAssetMenu(fileName = "InteractionData", menuName = "InteractionSystem/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        private InteractableBase m_interactable;

        public InteractableBase Interactable
        {
            get => m_interactable;
            set => m_interactable = value;
        }

        public void Interact()
        {
            m_interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase interactable) => m_interactable == interactable;
        public bool IsEmpty() => m_interactable == null;
        public void ResetData() => m_interactable = null;

    }
}