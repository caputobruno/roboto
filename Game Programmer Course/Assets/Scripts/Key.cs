using UnityEngine;

public class Key : Item
{
    float _userRange = 1;

    public override void Use()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _userRange);
        foreach (var hit in hits)
        {
            var toggleLock = hit.GetComponent<ToggleLock>();
            if (toggleLock)
            {
                toggleLock.Toggle();
                FindObjectOfType<PlayerInventory>().RemoveEquippedItem();
                Destroy(this.gameObject, 1);
                break;
            }
        }
    }
}
