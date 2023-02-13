using ChapterReversalMod.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ChapterReversalMod
{
    public class Main : ModInitializer
    {
        public static string Id { get; } = "ChapterReversalMod";
        public static Harmony Harmony { get; private set; }
        public static string ModPath { get; } = Directory.GetParent(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path))).FullName;
        public static Sprite Icon { get; } = Sprites.TryLoad(Path.Combine(ModPath, "icon.png"), out Sprite sprite) ? sprite : null;

        public override void OnInitializeMod()
        {
            Harmony = new Harmony(Id);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
