using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ANV
{
    public class InteractionController : MonoBehaviour
    {
        #region Variables
        [Header("Data")]
        public InteractionInputData interactionInputData;
        public InteractionData interactionData;

        [Space]
        [Header("UI")]
        [SerializeField] private InteractionUIPanel uiPanel;


        [Space]
        [Header("Ray Settings")]

        public float rayDistance;
        public float raySphereRadius;
        public LayerMask interactableLayer;

        #endregion

        #region Private Variables
        private Camera m_cam;
        private bool m_interacting;
        private float m_holder = 0f;

        #endregion

        #region Built in Methods
        void Awake()
        {
            m_cam = FindObjectOfType<Camera>();
        }

        void Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }
        #endregion

        #region Custom Methods
        void CheckForInteractable()
        {
            RaycastHit _hit;
            // Create a ray from the camera to the forward direction.
            Ray _ray = new Ray(m_cam.transform.position, m_cam.transform.forward);
            // Check if the ray hits any collider on the interactable layer.
            if (Physics.SphereCast(_ray, raySphereRadius, out _hit, rayDistance, interactableLayer))
            {
                // Get the interactable component from the hit collider.
                InteractableBase _interactable = _hit.collider.GetComponent<InteractableBase>();

                if (_interactable != null)

                    if (interactionData.IsEmpty())
                    {
                        interactionData.Interactable = _interactable;
                        uiPanel.SetTooltip(_interactable.TooltipMessage);
                    }
                    else if (!interactionData.IsSameInteractable(_interactable))
                    {
                        interactionData.Interactable = _interactable;
                        uiPanel.SetTooltip(_interactable.TooltipMessage);
                    }
            }
            else
            {
                uiPanel.ResetUI();
                interactionData.ResetData();
            }

            Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.red);
        }

        void CheckForInteractableInput()
        {
            if (interactionData.IsEmpty())
            {
                return;
            }

            if (interactionInputData.InteractedClicked)
            {
                m_interacting = true;
                m_holder = 0f;
            }

            if (interactionInputData.InteractRealease)
            {
                m_interacting = false;
                m_holder = 0f;
                uiPanel.UpdateProgressBar(0);
            }

            if (m_interacting)
            {
                if (!interactionData.Interactable.IsInteractable)
                {
                    return;
                }

                if (interactionData.Interactable.HoldInteract)
                {
                    m_holder += Time.deltaTime;

                    float heldPercent = m_holder / interactionData.Interactable.HoldDuration;
                    uiPanel.UpdateProgressBar(heldPercent);

                    if (heldPercent > 1)
                    {
                        interactionData.Interact();
                        m_interacting = false;
                    }
                }
                else
                {
                    interactionData.Interact();
                    m_interacting = false;
                }
            }
        }
        #endregion
    }
}