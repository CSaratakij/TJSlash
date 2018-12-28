using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    [RequireComponent(typeof(DamageAble))]
    public class Skeleton : MonoBehaviour
    {
        enum SkeletonState
        {
            LookForTarget,
            ChargeToTarget,
            Tired
        }

        [SerializeField]
        bool isMoveTowardRight;

        [SerializeField]
        float walkForce;

        [SerializeField]
        float chargeForce;

        [SerializeField]
        Transform sight;

        [SerializeField]
        Transform leftBound;

        [SerializeField]
        Transform rightBound;

        [SerializeField]
        LayerMask targetMask;


        SkeletonState state;
        bool isBeginMove;

        Vector2 cacheLeftBound;
        Vector2 cacheRightBound;

        RaycastHit2D raycastTargetSight;

        Animator anim;
        Rigidbody2D rigid;

        WaitForSeconds flipWait;

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

        void Start()
        {
            if (!isMoveTowardRight) {
                FlipSprite();
            }
        }

        void FixedUpdate()
        {
            MoveHandler();
        }

        void LateUpdate()
        {
            FlipHandler();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (state == SkeletonState.ChargeToTarget || state == SkeletonState.Tired)
                return;

            if (collision.gameObject.CompareTag("Bullet")) {

                if (raycastTargetSight.collider == null) {
                    isMoveTowardRight = !isMoveTowardRight;
                    FlipSprite();
                }

                isBeginMove = true;
                state = SkeletonState.ChargeToTarget;
            }
        }

        void Initialize()
        {
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();

            cacheLeftBound = leftBound.position;
            cacheRightBound = rightBound.position;

            flipWait = new WaitForSeconds(1.0f);
        }

        void MoveHandler()
        {
            switch (state)
            {
                case SkeletonState.LookForTarget:
                {
                    rigid.velocity = (isMoveTowardRight ? Vector2.right : Vector2.left) * walkForce * Time.fixedDeltaTime;
                    raycastTargetSight = Physics2D.Raycast(sight.position, isMoveTowardRight ? Vector2.right : Vector2.left, 15.0f, targetMask);

                    if (raycastTargetSight.collider != null) {
                        isBeginMove = true;
                        state = SkeletonState.ChargeToTarget;
                    }
                    break;
                }
                case SkeletonState.ChargeToTarget:
                {
                    if (isBeginMove)
                        rigid.velocity = (isMoveTowardRight ? Vector2.right : Vector2.left) * chargeForce * Time.fixedDeltaTime;
                    else
                        rigid.velocity = Vector2.zero;

                    break;
                }
                default:
                    rigid.velocity = Vector2.zero;
                    break;
            }
        }

        void FlipHandler()
        {
            if (isMoveTowardRight && transform.position.x > cacheRightBound.x) {
                isMoveTowardRight = false;
                isBeginMove = false;
                state = SkeletonState.Tired;
                StartCoroutine(WaitCallback());
            }
            else if (!isMoveTowardRight && transform.position.x < cacheLeftBound.x) {
                isMoveTowardRight = true;
                isBeginMove = false;
                state = SkeletonState.Tired;
                StartCoroutine(WaitCallback());
            }
        }

        void FlipSprite()
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1.0f;
            transform.localScale = newScale;
        }

        IEnumerator WaitCallback()
        {
            yield return flipWait;
            FlipSprite();
            state = SkeletonState.LookForTarget;
        }
    }
}
