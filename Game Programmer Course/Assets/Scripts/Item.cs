using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public bool UseOnlyOnce;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerInventory = collision.gameObject.GetComponent<PlayerInventory>();
        if (playerInventory)
        {
            playerInventory.Pickup(this, true);
        }
    }

    public abstract void Use();
}