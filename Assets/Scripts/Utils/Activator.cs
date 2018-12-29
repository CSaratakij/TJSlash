using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TJ
{
    public class Activator : MonoBehaviour
    {
        [SerializeField]
        GameObject[] objects;

        [SerializeField]
        string tagName;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            foreach (GameObject obj in objects)
            {
                if (obj == null)
                    continue;

                Handles.color = Color.blue;
                Handles.DrawLine(obj.transform.position, transform.position);
            }
        }
#endif

        void Awake()
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(tagName)) {
                Activate();
            }
        }

        void Activate()
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(true);
            }
        }
    }
}
