using UnityEngine;

namespace TJ
{
    [RequireComponent(typeof(DamageAble))]
    public class Drone : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        float upDownSpeed;

        [SerializeField]
        float upDownLength;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float upDownScale;

        [SerializeField]
        Gun gun;

        [SerializeField]
        Transform[] possiblePlaces;

        int currentPickIndex;

        bool isExpose;
        bool isShooted;

        Vector3[] cachePossiblePlaces;
        WaitForSeconds exposeDelay;

        Color fadeOutColor;
        SpriteRenderer spriteRenderer;

        Timer timer;
        Collider2D collider;


        void Awake()
        {
            Initialize();
            SubscribeEvent();
        }

        void OnDestroy()
        {
            UnSubscribeEvent();
        }

        void Update()
        {
            if (target == null || gun == null)
                return;

            if (isExpose)
            {
                if (isShooted) {
                    collider.enabled = false;
                    spriteRenderer.color = Color.Lerp(spriteRenderer.color, fadeOutColor, 0.05f);

                    if (spriteRenderer.color.a <= 0.1f) {
                        isExpose = false;
                        ChangePos();
                    }
                }
                else
                {
                    timer.Countdown();

                    var direction = (target.position - transform.position).normalized;
                    gun.BeginShoot(direction);
                }
            }
        }

        void LateUpdate()
        {
            var newPos = transform.position;
            newPos.y += Mathf.Sin(Time.time * upDownSpeed) * upDownLength * upDownScale * Time.deltaTime;
            transform.position = newPos;
        }

        void Initialize()
        {
            currentPickIndex = 0;
            isExpose = true;

            cachePossiblePlaces = new Vector3[possiblePlaces.Length];

            for (int i = 0; i < cachePossiblePlaces.Length; ++i)
            {
                cachePossiblePlaces[i] = possiblePlaces[i].position;
            }

            fadeOutColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
            timer = GetComponent<Timer>();
        }

        void SubscribeEvent()
        {
            timer.OnTimerStop += timer_OnStop;
        }

        void UnSubscribeEvent()
        {
            timer.OnTimerStop -= timer_OnStop;
        }

        void ChangePos()
        {
            currentPickIndex = (currentPickIndex + 1) >= cachePossiblePlaces.Length ? 0 : (currentPickIndex + 1);
            transform.position = cachePossiblePlaces[currentPickIndex];

            isExpose = true;
            isShooted = false;

            collider.enabled = true;
            spriteRenderer.color = Color.white;
        }

        void timer_OnStop()
        {
            isShooted = true;
        }
    }
}
