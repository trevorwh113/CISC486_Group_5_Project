using UnityEngine;

public interface IInteractable
{
    Transform InteractionTransform { get; }
    bool CanInteract { get; }
    bool IsInteracting { get; }

    void SetSelected(bool selected);
    void Interact(PlayerMovement player);
}
