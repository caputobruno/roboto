using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    private TMP_Text _interactText;
    private PlayerInput _playerInput;
    private List<Door> _doors = new();
    Rigidbody2D _rig;

    private void Awake()
    {
        _interactText = GetComponentInChildren<TMP_Text>();
        _playerInput = GetComponent<PlayerInput>();

        _interactText.gameObject.SetActive(false);
        _playerInput.actions["Interact"].performed += Interact;
    }

    public void FlipText() => _interactText.gameObject.transform.localScale = new Vector3(_interactText.gameObject.transform.localScale.x * -1, _interactText.gameObject.transform.localScale.y, _interactText.gameObject.transform.localScale.z);

    void Interact(InputAction.CallbackContext ctx)
    {
        foreach (var door in _doors)
            door.Interact(this);
    }

    internal void Add(Door door)
    {
        _doors.Add(door);
        _interactText.gameObject.SetActive(true);
    }

    internal void Remove(Door door)
    {
        _doors.Remove(door);
        if(_doors.Count == 0 )
            _interactText.gameObject.SetActive(false);
    }
}
