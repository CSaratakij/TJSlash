using UnityEngine;

namespace TJ
{
    public class FallingSpike : MonoBehaviour
    {
        [SerializeField]
        float fallForce;

        [SerializeField]
        LayerMask triggerFallingLayer;

        bool isFalling;

        RaycastHit2D hitResult;
        Rigidbody2D rigid;


        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            hitResult = Physics2D.Raycast(transform.position, Vector2.down, 100.0f, triggerFallingLayer);

            if (hitResult.collider != null) {
                isFalling = true;
            }

            MoveHandler();
        }

        void MoveHandler()
        {
            if (isFalling) {
                rigid.velocity = (Vector2.down * fallForce) * Time.fixedDeltaTime;
            }
            else {
                rigid.velocity = Vector2.zero;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            gameObject.SetActive(false);
        }
    }
}
