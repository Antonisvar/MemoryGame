using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class DestroyInteractable : InteractableBase
    {
        public override void OnInteract()
        {
            base.OnInteract();

            Destroy(gameObject);
        }
    }
}
