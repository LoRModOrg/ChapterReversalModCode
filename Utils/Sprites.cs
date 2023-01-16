using System.IO;
using UnityEngine;

namespace ChapterReversalMod.Utils
{
    public static class Sprites
    {
        public static bool TryLoad(string path, out Sprite sprite)
        {
            sprite = null;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return false;
            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(0, 0);
            if (!texture.LoadImage(data))
                return false;
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return true;
        }
    }
}
