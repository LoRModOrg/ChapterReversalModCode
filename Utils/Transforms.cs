using System.Collections.Generic;
using UnityEngine;

namespace ChapterReversalMod.Utils
{
    public static class Transforms
    {
        public static IEnumerable<Transform> GetParents(this Transform transform)
        {
            Transform parent = transform.parent;
            while (parent) {
                yield return parent;
                parent = parent.parent;
            }
        }
    }
}
