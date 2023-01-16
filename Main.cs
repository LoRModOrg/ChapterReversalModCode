using HarmonyLib;
using System.Reflection;

namespace ChapterReversalMod
{
    public class Main : ModInitializer
    {
        public static string Id { get; } = "ChapterReversalMod";
        public static Harmony Harmony { get; private set; }

        public override void OnInitializeMod()
        {
            Harmony = new Harmony(Id);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
