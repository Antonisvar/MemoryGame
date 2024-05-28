using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class InputHandler : MonoBehaviour
    {

        #region Data
        // Reference to the interaction input data.
        public InteractionInputData interactionInputData;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            interactionInputData.ResetInput();
        }

        // Update is called once per frame
        void Update()
        {
            GetInteractionInputData();
        }

        #region Custom Methods

        void GetInteractionInputData()
        {
            interactionInputData.InteractedClicked = Input.GetKeyDown(KeyCode.E);
            interactionInputData.InteractRealease = Input.GetKeyUp(KeyCode.E);
        }

        #endregion
    }
}