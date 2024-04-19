using UnityEngine;

public class CutsceneSignalEmitter : MonoBehaviour
{
    public void ToggleCinematic(bool cinematicPlaying) => GameManager.Instance.ToggleCinematic(cinematicPlaying);
}
