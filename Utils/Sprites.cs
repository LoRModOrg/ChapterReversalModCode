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

        public static Texture2D ToReadable(this Texture2D texture)
        {
            if (texture.isReadable)
                return texture;
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                        texture.width,
                        texture.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, renderTexture);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D readable = new Texture2D(texture.width, texture.height);
            readable.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readable.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
            return readable;
        }
    }
}
