using MLAPI;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInteraction : NetworkedBehaviour
{
    [Header("Interaction")]
    public Camera MainCamera;
    public float InteractionDistance = 20.0f;
    public Interactable InteractableInRange;

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        // Interaction
        if (!InventoryManager.Singleton.InventoryUI.mainInventoryOpen)
        {
            // TODO: Seperate raycast from object detection.
            var lookDirection = MainCamera.transform.forward.normalized;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(MainCamera.transform.position, lookDirection, out RaycastHit hit, InteractionDistance))
            {
                Debug.DrawRay(MainCamera.transform.position, lookDirection * hit.distance, Color.yellow);

                var interactable = hit.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    InteractableInRange = interactable;
                } else
                {
                    InteractableInRange = null;
                }
            }
            else
            {
                Debug.DrawRay(MainCamera.transform.position, lookDirection, Color.blue);

                InteractableInRange = null;
            }
        }
    }

    /// <summary>
    /// Player presses Interact key.
    /// Interacts with interactable in the world.
    /// </summary>
    /// <param name="input"></param>
    public void OnInteract(InputValue _)
    {
        if (InteractableInRange != null)
        {
            InteractableInRange.Interact(this);
        }
    }


    // Rendering
    private void LateUpdate()
    {
        if (InteractableInRange != null)
        {
            InteractionManager.Singleton.InteractionText.text = InteractableInRange.DisplayName + " [Press F to interact]";
        }
        else
        {
            InteractionManager.Singleton.InteractionText.text = "";
        }
    }
}