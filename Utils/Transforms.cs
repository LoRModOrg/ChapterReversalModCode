using System;
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

        public static IEnumerable<Transform> GetChilds(this Transform transform, Predicate<Transform> filter = null)
        {
            foreach (Transform child in transform)
                if (filter == null || filter(child))
                    yield return child;
        }

        public static IEnumerable<Transform> GetChildsByName(this Transform transform, Predicate<string> nameFilter)
        {
            return transform.GetChilds(child => nameFilter(child.name));
        }
    }
}
