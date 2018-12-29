using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]
        AudioClip effects;

        [SerializeField]
        GameObject[] objects;

        bool isPass;
        bool isPlayEffect;

        Color fadeOutColor;
        new Collider2D collider;

        AudioSource audioSource;
        SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
        int total = 0;

        void OnDrawGizmos()
        {
            total = 0;
            Handles.color = Color.red;

            foreach (GameObject obj in objects)
            {
                Handles.DrawDottedLine(obj.transform.position, transform.position, 2.0f);
                total += 1;
            }

            Handles.Label(transform.position, string.Format("Total : {0}", total));
        }
#endif

        void Awake()
        {
            fadeOutColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            collider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
        }

        void LateUpdate()
        {
            if (isPass) {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, fadeOutColor, 0.05f);

                if (spriteRenderer.color.a <= 0.2f && !isPlayEffect) {
                    audioSource.PlayOneShot(effects);
                    StartCoroutine(PlayEffect_Callback());
                    isPlayEffect = true;
                }
            }
            else {
                foreach (GameObject obj in objects)
                {
                    if (obj.activeSelf)
                        return;
                }

                isPass = true;
                collider.enabled = false;
            }
        }

        IEnumerator PlayEffect_Callback()
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
