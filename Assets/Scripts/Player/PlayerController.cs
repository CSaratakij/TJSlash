using UnityEngine;

namespace TJ
{
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
        AudioClip jumpAudioClip;

        int totalJump;

        float oldRange;
        float newRange;

        float newPitch;
        float currentJumpVelocity;

        bool isGrounded;

        bool isPressJump;
        bool isJumped;
        bool isJumpKeyDown;
        bool isJumpKeyUp;

        bool currentPressJumpState;
        bool previousPressJumpState;

        bool isFacingRight = true;
        bool allowPlayJumpAudio;

        Vector2 velocity;
        Vector2 inputVector;
        Vector2 groundRaycastDirection;
        Vector2 boxCastHeadSize;

        Vector3 newScale;

        Animator anim;
        AudioSource audioSource;

        Rigidbody2D rigid;

        RaycastHit2D raycastHeadHit;
        RaycastHit2D raycastGroundHit;

        void Awake()
        {
            Initialize();
        }

        void Update()
        {
            InputHandler();
            AnimationHandler();
            FlipHandler();
        }

        void FixedUpdate()
        {
            MovementHandler();
        }

        void Initialize()
        {
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            rigid = GetComponent<Rigidbody2D>();
            groundRaycastDirection = new Vector3(-1.0f, -1.0f);
            boxCastHeadSize = new Vector2(0.8f, 1.0f);
        }

        void InputHandler()
        {
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

            float oldRange = (maxJumpVelocity - 0.0f);
            float newRange = (1.14f - 0.8f);
            float newPitch = (((currentJumpVelocity - 0.0f) * newRange) / oldRange) + 0.8f;

            audioSource.pitch = newPitch;
        }

        void MovementHandler()
        {
            groundRaycastDirection.x = isFacingRight ? -1.0f : 1.0f;

            raycastGroundHit = Physics2D.Raycast(ground.position, groundRaycastDirection, 0.65f, groundLayer, 0.0f, 0.0f);
            raycastHeadHit = Physics2D.BoxCast(head.position, boxCastHeadSize, 0.0f, Vector2.up, 0.03f, groundLayer);

            isGrounded = (raycastGroundHit.collider == null) ? false : true;

            if (isGrounded) {
                totalJump = 0;
                allowPlayJumpAudio = true;
                currentJumpVelocity = 0.0f;
                velocity.x = (inputVector.x * moveForce) * Time.fixedDeltaTime;
            }
            else {
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
            anim.SetBool("IsWalk", isGrounded && (inputVector.x > 0.0f || inputVector.x < 0.0f));
            anim.SetBool("IsJump", !isGrounded && velocity.y > 0.0f);
        }

        void FlipHandler()
        {
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
    }
}
