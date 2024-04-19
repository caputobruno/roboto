using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FluidFlowAnimation : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 1;
    SpriteRenderer _sr;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float x = Mathf.Repeat(Time.time * _scrollSpeed, 1);
        Vector2 offset = new Vector2(x, 0);
        _sr.material.SetTextureOffset("_MainTex", offset);
    }
}