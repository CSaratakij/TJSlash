using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]
        GameObject[] objects;

        bool isPass;
        Color fadeOutColor;
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
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (isPass) {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, fadeOutColor, 0.05f);

                if (spriteRenderer.color.a <= 0.1f)
                    gameObject.SetActive(false);
            }
            else
            {
                foreach (GameObject obj in objects)
                {
                    if (obj.activeSelf)
                        return;
                }

                isPass = true;
            }
        }
    }
}
