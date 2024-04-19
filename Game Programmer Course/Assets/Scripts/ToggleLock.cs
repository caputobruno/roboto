using UnityEngine;
using UnityEngine.Events;

public class ToggleLock : MonoBehaviour
{
    [SerializeField] UnityEvent OnUnlocked;

    bool _locked;

    private void Awake()
    {
        _locked = true;
    }

    [ContextMenu(nameof(Toggle))]
    public void Toggle()
    {
        _locked = !_locked;
        if(!_locked)
            OnUnlocked?.Invoke();
    }
}
