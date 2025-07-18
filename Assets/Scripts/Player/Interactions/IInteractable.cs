using UnityEngine;

public interface IInteractable
{
    public void Interact(InteractionContexzt context, Shopper currentShopper);
    public void VisualHintForInteractable();
}
