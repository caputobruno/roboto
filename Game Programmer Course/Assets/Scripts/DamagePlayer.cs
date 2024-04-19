using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] bool _ignoreFromTop;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_ignoreFromTop && Vector2.Dot(collision.contacts[0].normal, Vector2.down) > 0.5f)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage(collision.contacts[0].normal);
            }
        }
    }
}
