using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] bool _onlyFromTop;
    [SerializeField] float _bounciness;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_onlyFromTop && Vector2.Dot(collision.contacts[0].normal, Vector2.down) < 0.5f)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                player.Bounce(collision.contacts[0].normal, _bounciness);
            }
        }
    }
}