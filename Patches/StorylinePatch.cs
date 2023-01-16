using ChapterReversalMod.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace ChapterReversalMod.Patches
{
    public static class StorylinePatch
    {
        private const string TOGGLE_NAME = "ChapterReversal";

        [HarmonyPatch(typeof(UIStoryGradeFilter), "Init")]
        public static class InitPatch1
        {
            public static void Prefix(UIStoryGradeFilter __instance, List<UIStoryGradeFilterSlot> ___gradeSlots)
            {
                if (!__instance.transform.GetParents().Any(transform => transform.name.Contains("Invitation_Panel"))
                    || !__instance.transform.GetParents().Any(transform => transform.name.Contains("StoryProgress"))
                    || ___gradeSlots.Any(slot => slot.transform.name == TOGGLE_NAME))
                    return;
                GameObject origin = ___gradeSlots[0].gameObject;
                GameObject copy = Object.Instantiate(origin, origin.transform.parent.parent);
                copy.transform.name = TOGGLE_NAME;
                copy.transform.position = origin.transform.parent.position;
                copy.transform.localPosition = new Vector3(-405, 17, 0);
                copy.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                UIStoryGradeFilterSlot newSlot = copy.GetComponent<UIStoryGradeFilterSlot>();
                newSlot.Set("gradeIdx", Grade.grade7);
                newSlot.Get<UICustomSelectableToggle>("SelectableToggle").IsOn = false;
                newSlot.Get<Image>("img_icon").sprite = Main.Icon;
                newSlot.Get<Image>("img_icon").color = UIColorManager.Manager.GetUIColor(UIColor.Default);
                newSlot.Get<Image>("img_iconGlow").gameObject.SetActive(false);
                ___gradeSlots.Add(newSlot);
            }
        }

        [HarmonyPatch(typeof(UIStoryGradeFilter), "Activate")]
        public static class ActivatePatch
        {
            public static void Postfix(UIStoryGradeFilter __instance, List<UIStoryGradeFilterSlot> ___gradeSlots)
            {
                if (!__instance.transform.GetParents().Any(transform => transform.name.Contains("Invitation_Panel")))
                    return;
                if (LibraryModel.Instance.GetChapter() == 7)
                {
                    ___gradeSlots.Find(slot => slot.transform.name == TOGGLE_NAME)?.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(UIStoryGradeFilterSlot), "OnClickToggle")]
        public static class ClickPatch
        {
            public static bool Prefix(UIStoryGradeFilterSlot __instance)
            {
                if (__instance.transform.name != TOGGLE_NAME)
                    return true;
                UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);
                __instance.SetToggleOn(false);
                // TODO: 클릭시 이벤트 구현
                return false;
            }
        }
    }
}
