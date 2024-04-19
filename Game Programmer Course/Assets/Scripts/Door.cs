using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject _close1;
    [SerializeField] GameObject _close2;
    [SerializeField] GameObject _open1;
    [SerializeField] GameObject _open2;
    [SerializeField] Vector2 _offsetDestination;

    [ContextMenu("Open")]
    public void Open()
    {
        _close1.SetActive(false);
        _close2.SetActive(false);
        _open1.SetActive(true);
        _open2.SetActive(true);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        _close1.SetActive(true);
        _close2.SetActive(true);
        _open1.SetActive(false);
        _open2.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if(playerInteractionController != null)
        {
            playerInteractionController.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if (playerInteractionController != null)
        {
            playerInteractionController.Remove(this);
        }
    }

    public void Interact(PlayerInteractionController playerInteractionController)
    {
        var destination = Vector2.Distance(playerInteractionController.transform.position, _open1.transform.position) > 
            Vector2.Distance(playerInteractionController.transform.position, _open2.transform.position) 
            ? _open1 : _open2;

        playerInteractionController.gameObject.transform.position = destination.transform.position + (Vector3)_offsetDestination;
    }
}
