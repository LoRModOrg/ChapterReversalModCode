using System.Collections;
using UnityEngine;

namespace ChapterReversalMod.Utils
{
    public static class StaticCoroutine
    {
        private static _StaticCoroutine instance;

        public static void Run(IEnumerator routine)
        {
            if (!instance)
                instance = new GameObject("StaticCoroutine").AddComponent<_StaticCoroutine>();
            instance.Run(routine);
        }

        private class _StaticCoroutine : MonoBehaviour
        {
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            public void Run(IEnumerator routine)
            {
                StartCoroutine(routine);
            }
        }
    }
}
