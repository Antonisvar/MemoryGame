using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class InteractableBase : MonoBehaviour,
    IInteractable
    {
        #region Variables

        [SerializeField] private float holdDuration = 1f;
        [SerializeField] private bool holdInteract = true;
        [SerializeField] private bool multipleUse = false;
        [SerializeField] private bool isInteractable = true;

        [SerializeField] private string tooltipMessage = "Interact";

        #endregion

        #region Properties
        public float HoldDuration => holdDuration;
        public bool HoldInteract => holdInteract;
        public bool MultipleUse => multipleUse;
        public bool IsInteractable => isInteractable;

        public string TooltipMessage => tooltipMessage;

        #endregion

        #region Methods
        public virtual void OnInteract()
        {
            Debug.Log("Interacted with: " + gameObject.name);
        }
        #endregion
    }


}

