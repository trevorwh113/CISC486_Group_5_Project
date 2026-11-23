using UnityEngine;

public interface IInteractable
{
    Transform InteractionTransform { get; }
    bool CanInteract { get; }
    bool IsInteracting { get; }
    /** Higher values are preferred when multiple interactables are in range. */
    int InteractionPriority { get; }

    void SetSelected(bool selected);
    void Interact(PlayerMovement player);
}
