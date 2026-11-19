using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteraction : MonoBehaviour
{
    private readonly List<IInteractable> nearbyInteractables = new();

    private PlayerMovement player;
    private IInteractable selectedInteractable;
    private IInteractable lockedInteraction;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (lockedInteraction != null)
        {
            if (lockedInteraction.IsInteracting)
            {
                return;
            }

            lockedInteraction = null;
        }

        SelectClosestInteractable();

        if (selectedInteractable != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            IInteractable interaction = selectedInteractable;
            interaction.Interact(player);

            if (interaction.IsInteracting)
            {
                lockedInteraction = interaction;
            }
        }
    }

    public void Register(IInteractable interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    public void Unregister(IInteractable interactable, bool hidePrompt = true)
    {
        nearbyInteractables.Remove(interactable);

        if (selectedInteractable == interactable)
        {
            if (hidePrompt)
            {
                selectedInteractable.SetSelected(false);
            }

            selectedInteractable = null;
        }
    }

    private void SelectClosestInteractable()
    {
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        foreach (IInteractable interactable in nearbyInteractables)
        {
            if (!interactable.CanInteract)
            {
                continue;
            }

            float distance = Vector3.SqrMagnitude(
                transform.position - interactable.InteractionTransform.position
            );

            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        if (closest == selectedInteractable)
        {
            return;
        }

        selectedInteractable?.SetSelected(false);
        selectedInteractable = closest;
        selectedInteractable?.SetSelected(true);
    }
}
