using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnEventTrigger;
    bool _triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) return;
        if (_triggered) return;
        _triggered = true;

        OnEventTrigger?.Invoke();
    }
}
