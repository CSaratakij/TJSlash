using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    [RequireComponent(typeof(DamageAble))]
    public class Slime : MonoBehaviour
    {
        [SerializeField]
        bool isMoveTowardRight;

        [SerializeField]
        float moveForce;

        [SerializeField]
        Transform leftBound;

        [SerializeField]
        Transform rightBound;


        Vector2 cacheLeftBound;
        Vector2 cacheRightBound;

        Animator anim;
        Rigidbody2D rigid;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (leftBound == null || rightBound == null)
                return;

            Handles.color = Color.yellow;
            Handles.DrawDottedLine(leftBound.position, rightBound.position, 2.0f);
            Handles.Label(leftBound.position, "Left Bound");
            Handles.Label(rightBound.position, "Right Bound");
        }
#endif

        void Awake()
        {
            Initialize();
        }

        void FixedUpdate()
        {
            MoveHandler();
        }

        void Update()
        {
            AnimationHandler();
        }

        void LateUpdate()
        {
            FlipHandler();
        }

        void Initialize()
        {
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();

            cacheLeftBound = leftBound.position;
            cacheRightBound = rightBound.position;
        }

        void MoveHandler()
        {
            rigid.velocity = (isMoveTowardRight ? Vector2.right : Vector2.left) * moveForce * Time.fixedDeltaTime;
        }

        void AnimationHandler()
        {
            anim.SetBool("IsWalk", true);
        }

        void FlipHandler()
        {
            if (isMoveTowardRight && transform.position.x > cacheRightBound.x) {
                isMoveTowardRight = false;
                FlipSprite();
            }
            else if (!isMoveTowardRight && transform.position.x < cacheLeftBound.x) {
                isMoveTowardRight = true;
                FlipSprite();
            }
        }

        void FlipSprite()
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1.0f;
            transform.localScale = newScale;
        }
    }
}
