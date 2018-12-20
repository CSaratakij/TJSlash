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

        float newPitch;
        float currentJumpVelocity;

        bool isGrounded;
        bool isFalling;

        bool currentGroundState;
        bool previousGroundState;

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
        Vector2 boxCastBodySize;

        Vector3 newScale;

        Animator anim;
        AudioSource audioSource;

        Rigidbody2D rigid;

        RaycastHit2D raycastHeadHit;
        RaycastHit2D raycastGroundHit;
        RaycastHit2D raycastFallingCheck;

        void Awake()
        {
            Initialize();
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
            MovementHandler();
        }

        void Initialize()
        {
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            rigid = GetComponent<Rigidbody2D>();
            groundRaycastDirection = new Vector3(-1.0f, -1.0f);
            boxCastHeadSize = new Vector2(0.855f, 1.0f);
            boxCastBodySize = new Vector2(0.855f, 0.4f);
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
        }

        void MovementHandler()
        {
            groundRaycastDirection.x = isFacingRight ? -1.0f : 1.0f;

            raycastGroundHit = Physics2D.BoxCast(ground.position, boxCastBodySize, 0.0f, Vector2.down, 0.03f, groundLayer);
            raycastHeadHit = Physics2D.BoxCast(head.position, boxCastHeadSize, 0.0f, Vector2.up, 0.03f, groundLayer);
            raycastFallingCheck = Physics2D.BoxCast(ground.position, boxCastBodySize, 0.0f, Vector2.down, 1.2f, groundLayer); 

            isGrounded = raycastGroundHit.collider != null;

            previousGroundState = currentGroundState;
            currentGroundState = isGrounded;

            if (raycastFallingCheck.collider == null)
                isFalling = true;
            else
                isFalling = velocity.y <= 0.0f;

            if (isGrounded) {
                totalJump = 0;
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
