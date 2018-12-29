using UnityEngine;

namespace TJ
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        bool isLimitRange;

        [SerializeField]
        float maximumRange;
        
        float force;

        Vector2 maximumPos;
        Vector2 direction;

        Rigidbody2D rigid;


        void OnEnable()
        {
            maximumPos = transform.position + (new Vector3(direction.x, direction.y, 0.0f) * maximumRange);
        }

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void LateUpdate()
        {
            if (!isLimitRange)
                return;

            if (Vector2.Distance(maximumPos, transform.position) <= 0.1f) {
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            rigid.velocity = (direction * force) * Time.fixedDeltaTime;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Bullet") || collider.gameObject.CompareTag("Potion") || collider.gameObject.CompareTag("Coin") || collider.gameObject.CompareTag("EnemyBullet"))
                return;

            gameObject.SetActive(false);
        }

        public void Move(Vector2 direction, float force)
        {
            this.direction = direction;
            this.force = force;
        }
    }
}
