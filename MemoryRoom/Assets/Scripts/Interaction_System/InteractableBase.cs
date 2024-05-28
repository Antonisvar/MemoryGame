using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class InteractableBase : MonoBehaviour,
    IInteractable
    {
        #region Variables

        public float holdDuration;
        public bool holdInteract;
        public bool multipleUse;
        public bool isInteractable;

        #endregion

        #region Properties
        public float HoldDuration => holdDuration;
        public bool HoldInteract => holdInteract;
        public bool MultipleUse => multipleUse;
        public bool IsInteractable => isInteractable;

        #endregion

        #region Methods
        public void OnInteract()
        {
            Debug.Log("Interacted with: " + gameObject.name);
        }
        #endregion
    }


}

