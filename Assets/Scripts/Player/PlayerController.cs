using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    [RequireComponent(typeof(Stat))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float moveForce;

        [SerializeField]
        float jumpForce;

        [SerializeField]
        float maxJumpVelocity;

        [SerializeField]
        float onGroundGravity;

        [SerializeField]
        float gravity;

        [SerializeField]
        float verticalTerminalVelocity;

        [SerializeField]
        Transform head;

        [SerializeField]
        Transform ground;

        [SerializeField]
        LayerMask groundLayer;

        [SerializeField]
        LayerMask headButtLayer;

        [SerializeField]
        LayerMask oneWayCollisionLayer;

        [SerializeField]
        Gun gun;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask enemyLayer;

        [SerializeField]
        AudioClip jumpAudioClip;

        [SerializeField]
        AudioClip loseAudioClip;

        [SerializeField]
        AudioClip winAudioClip;

        [SerializeField]
        AudioClip hitAudioClip;

        [SerializeField]
        AudioClip pickUpAudioClip;

        [SerializeField]
        AudioClip collectCointAudioClip;

        int totalJump;
        int totalCoin;

        float newPitch;
        float currentJumpVelocity;

        bool isGrounded;
        bool isFalling;

        bool currentGroundState;
        bool previousGroundState;

        bool currentOneWayCollisionState;
        bool previousOneWayCollsionState;

        bool isPressJump;
        bool isJumped;
        bool isJumpKeyDown;
        bool isJumpKeyUp;

        bool currentPressJumpState;
        bool previousPressJumpState;

        bool isFacingRight = true;
        bool allowPlayJumpAudio;
        bool isInvinsible = false;

        bool isDead;
        bool isBeginOneWayCollision;

        Vector2 velocity;
        Vector2 inputVector;
        Vector2 groundRaycastDirection;
        Vector2 boxCastHeadSize;
        Vector2 boxCastBodySize;

        Vector3 newScale;

        Animator anim;
        AudioSource audioSource;

        Rigidbody2D rigid;

        RaycastHit2D raycastHeadHit;
        RaycastHit2D raycastGroundHit;
        RaycastHit2D raycastFallingCheck;
        RaycastHit2D raycastOneWayCollision;

        SpriteRenderer spriteRenderer;
        Stat stat;

        Color flickeringColor;
        WaitForSeconds flickeringWait;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
        }
#endif

        void Awake()
        {
            Initialize();
            SubscribeEvents();
        }

        void OnDestroy()
        {
            UnSubscribeEvents();
        }

        void Update()
        {
            InputHandler();
            AnimationHandler();
            FlipHandler();
            AudioHandler();
        }

        void FixedUpdate()
        {
            CheckHitEnemy();
            MovementHandler();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isDead && !isInvinsible && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))) {
                stat.health.Remove(1);
                audioSource.PlayOneShot(hitAudioClip);

                StartCoroutine(Flickering_Begin_Callback());
                isInvinsible = true;
            }
            else if ((stat.health.current < stat.health.max) && (collision.gameObject.CompareTag("Potion"))) {
                stat.health.Restore(1);
                collision.gameObject.SetActive(false);
                audioSource.PlayOneShot(pickUpAudioClip);
            }
            else if (collision.gameObject.CompareTag("Coin")) {
                totalCoin += 1;
                collision.gameObject.SetActive(false);
                audioSource.PlayOneShot(collectCointAudioClip);
            }
            else if (collision.gameObject.CompareTag("Flag")) {
                GameController.Instance.GameStop();
                audioSource.PlayOneShot(winAudioClip);
            }
        }

        IEnumerator Flickering_Begin_Callback()
        {
            IEnumerator flickeringCallback = Flickering_Callback();
            StartCoroutine(flickeringCallback);
            yield return flickeringWait;
            StopCoroutine(flickeringCallback);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.3f);
            isInvinsible = false;

            if (stat.health.current <= 0) {
                Debug.Log("Dead...");
                GameController.Instance.GameStop();
                audioSource.PlayOneShot(loseAudioClip);
            }
        }

        IEnumerator Flickering_Callback()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = flickeringColor;
                yield return new WaitForSeconds(0.2f);
                spriteRenderer.color = Color.white;
            }
        }

        void Initialize()
        {
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            stat = GetComponent<Stat>();
            groundRaycastDirection = new Vector3(-1.0f, -1.0f);
            boxCastHeadSize = new Vector2(0.855f, 1.0f);
            boxCastBodySize = new Vector2(0.855f, 0.4f);
            flickeringColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);
            flickeringWait = new WaitForSeconds(0.8f);
        }

        void SubscribeEvents()
        {
            stat.health.OnValueChanged += health_OnValueChanged;
        }
        
        void UnSubscribeEvents()
        {
            stat.health.OnValueChanged -= health_OnValueChanged;
        }

        void health_OnValueChanged(int value)
        {
            if (isDead)
                return;

            if (value <= 0 && !isDead) {
                isDead = true;
                anim.SetTrigger("Dead");
                gameObject.layer = LayerMask.NameToLayer("PlayerDead");
                spriteRenderer.sortingOrder = 1;
            }
        }

        void InputHandler()
        {
            if (isDead || !GameController.IsGameStart) {
                inputVector = Vector2.zero;
                isPressJump = false;
                return;
            }

            inputVector.x = Input.GetAxisRaw("Horizontal");
            inputVector.y = Input.GetAxisRaw("Vertical");

            if (inputVector.x > 0.0f) {
                inputVector.x = 1.0f;
            }
            else if (inputVector.x < 0.0f) {
                inputVector.x = -1.0f;
            }

            if (inputVector.y > 0.0f) {
                inputVector.y = 1.0f;
            }
            else if (inputVector.y < 0.0f) {
                inputVector.y = -1.0f;
            }

            isPressJump = Input.GetButton("Jump");

            previousPressJumpState = currentPressJumpState;
            currentPressJumpState = isPressJump;

            isJumpKeyDown = (!previousPressJumpState && currentPressJumpState);
            isJumpKeyUp = (previousPressJumpState && !currentPressJumpState);

            if (isJumpKeyDown && totalJump < 1) {
                allowPlayJumpAudio = true;
            }

            if (isJumpKeyUp) {
                isJumped = false;
                totalJump += 1;
            }

            if (Input.GetButton("Fire1")) {
                gun.BeginShoot(isFacingRight ? Vector2.right : Vector2.left);
            }
        }

        void CheckHitEnemy()
        {
            if (isDead)
                return;

            if (isInvinsible)
                return;

            Collider2D collider = Physics2D.OverlapBox(transform.position + offset, size, 0.0f, enemyLayer, 0.0f, 0.0f);

            if (collider == null)
                return;

            stat.health.Remove(1);
            audioSource.PlayOneShot(hitAudioClip);

            StartCoroutine(Flickering_Begin_Callback());
            isInvinsible = true;
        }

        void MovementHandler()
        {
            if (!GameController.IsGameStart) {
                velocity.x = 0;
                velocity.y = -onGroundGravity * Time.fixedDeltaTime;
                rigid.velocity = velocity;
                return;
            }

            groundRaycastDirection.x = isFacingRight ? -1.0f : 1.0f;

            raycastGroundHit = Physics2D.BoxCast(ground.position, boxCastBodySize, 0.0f, Vector2.down, 0.03f, groundLayer);
            raycastHeadHit = Physics2D.BoxCast(head.position, boxCastHeadSize, 0.0f, Vector2.up, 0.03f, headButtLayer);
            raycastFallingCheck = Physics2D.BoxCast(ground.position, boxCastBodySize, 0.0f, Vector2.down, 1.2f, groundLayer); 
            raycastOneWayCollision = Physics2D.BoxCast(ground.position, boxCastBodySize, 0.0f, Vector2.down, 0.03f, oneWayCollisionLayer);

            isGrounded = raycastGroundHit.collider != null;

            previousGroundState = currentGroundState;
            currentGroundState = isGrounded;

            if (raycastFallingCheck.collider == null)
                isFalling = true;
            else
                isFalling = velocity.y <= 0.0f;

            previousOneWayCollsionState = currentOneWayCollisionState;
            currentOneWayCollisionState = raycastOneWayCollision.collider != null;

            isBeginOneWayCollision = !previousOneWayCollsionState && currentOneWayCollisionState;

            if (isGrounded) {
                if (velocity.y > 0.0f && raycastOneWayCollision.collider != null && isBeginOneWayCollision) {
                    totalJump = 1;
                    isJumped = true;
                    isBeginOneWayCollision = false;
                }
                else {
                    totalJump = 0;
                }

                isFalling = false;
                allowPlayJumpAudio = true;
                currentJumpVelocity = 0.0f;
                velocity.x = (inputVector.x * moveForce) * Time.fixedDeltaTime;
            }
            else {
                if (previousGroundState && !isPressJump && isFalling && totalJump <= 0) {
                    totalJump = 1;
                }

                velocity.x = (inputVector.x * (moveForce * 0.85f)) * Time.fixedDeltaTime;
            }

            if (isPressJump && !isJumped && totalJump < 1) {
                if (currentJumpVelocity < maxJumpVelocity && raycastHeadHit.collider == null) {
                    velocity.y = jumpForce * Time.fixedDeltaTime;
                    currentJumpVelocity += jumpForce * Time.fixedDeltaTime;

                    if (allowPlayJumpAudio) {
                        audioSource.clip = jumpAudioClip;

                        if (!audioSource.isPlaying)
                            audioSource.Play();

                        allowPlayJumpAudio = false;
                    }
                }
                else {
                    isJumped = true;
                    if (raycastHeadHit.collider != null) {
                        velocity.y = -onGroundGravity * Time.fixedDeltaTime;
                    }
                }
            }
            else {
                if (isGrounded)
                    velocity.y = rigid.velocity.y + (-onGroundGravity * Time.fixedDeltaTime);
                else
                    velocity.y -= ((rigid.velocity.y * rigid.velocity.y) + gravity) * Time.fixedDeltaTime;
            }

            velocity.y = Mathf.Clamp(velocity.y, -verticalTerminalVelocity * Time.fixedDeltaTime, jumpForce);
            rigid.velocity = velocity;
        }

        void AnimationHandler()
        {
            if (Time.timeScale > 0.0f) {
                anim.SetBool("IsWalk", isGrounded && (inputVector.x > 0.0f || inputVector.x < 0.0f));
                anim.SetBool("IsJump", !isGrounded && velocity.y > 0.0f);
            }
            else {
                anim.SetBool("IsWalk", false);
            }
        }

        void FlipHandler()
        {
            if (!GameController.IsGameStart || Time.timeScale <= 0.0f)
                return;

            if (inputVector.x > 0.0f && !isFacingRight)
                FlipSprite();

            else if (inputVector.x < 0.0f && isFacingRight)
                FlipSprite();
        }

        void FlipSprite()
        {
            isFacingRight = !isFacingRight;
            newScale = transform.localScale;
            newScale.x *= -1.0f;
            transform.localScale = newScale;
        }

        void AudioHandler()
        {
            newPitch = ConvertNumberLine(currentJumpVelocity, 0.0f, maxJumpVelocity, 0.8f, 1.14f);
            audioSource.pitch = newPitch;
        }

        float ConvertNumberLine(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            float oldRange = (oldMax - oldMin);
            float newRange = (newMax - newMin);
            return (((value - oldMin) * newRange) / oldRange) + newMin;
        }
    }
}
