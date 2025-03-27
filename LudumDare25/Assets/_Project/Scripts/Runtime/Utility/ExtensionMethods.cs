using System;
using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Runtime.Utility
{
    public static class ExtensionMethods
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static void DelayedInvoke(this MonoBehaviour mb, Action f, float delay)
        {
            mb.StartCoroutine(InvokeRoutine(f, delay));
        }

        private static IEnumerator InvokeRoutine(System.Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }

        public static Vector3 RandomPointInBounds(this Bounds bounds)
        {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static void Random(this ref Vector3 myVector, Vector3 min, Vector3 max)
        {
            myVector = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y),
                UnityEngine.Random.Range(min.z, max.z));
        }
        
        public static float GetFrustumWidthAtDistance(this Camera c,float distance)
        {
            if (c == null) return 0;

            float height = 2f * distance * Mathf.Tan(c.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return height * c.aspect;
        }
    }
}