using ChapterReversalMod.Utils;
using HarmonyLib;
using Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace ChapterReversalMod.Patches
{
    public static class StorylinePatch
    {
        private const string TOGGLE_NAME = "ChapterReversal";
        private static readonly Sprite toggleIcon = Sprites.TryLoad(Path.Combine(Main.ModPath, "Resource", "Sprites", "toggleIcon.png"), out Sprite spr) ? spr : null;
        private static readonly Sprite toggleIconGlow = Sprites.TryLoad(Path.Combine(Main.ModPath, "Resource", "Sprites", "toggleIconGlow.png"), out Sprite spr) ? spr : null;

        [HarmonyPatch(typeof(UIStoryGradeFilter), "Init")]
        public static class InitPatch1
        {
            public static void Prefix(UIStoryGradeFilter __instance, List<UIStoryGradeFilterSlot> ___gradeSlots)
            {
                if (!__instance.transform.GetParents().All(transform => transform.name != "[Panel]Invitation_Panel(Clone)" && transform.name != "[Script]StoryProgress")
                    || ___gradeSlots.Any(slot => slot.transform.name == TOGGLE_NAME))
                    return;
                GameObject origin = ___gradeSlots[0].gameObject;
                GameObject copy = UnityEngine.Object.Instantiate(origin, origin.transform.parent.parent);
                copy.transform.name = TOGGLE_NAME;
                copy.transform.position = origin.transform.parent.position;
                copy.transform.localPosition = new Vector3(-405, origin.transform.parent.localPosition.y, 0);
                UIStoryGradeFilterSlot newSlot = copy.GetComponent<UIStoryGradeFilterSlot>();
                newSlot.Set("gradeIdx", Grade.grade7);
                newSlot.Get<UICustomSelectableToggle>("SelectableToggle").IsOn = false;
                newSlot.Get<Image>("img_icon").sprite = toggleIcon;
                newSlot.Get<Image>("img_icon").color = UIColorManager.Manager.GetUIColor(UIColor.Default);
                newSlot.Get<Image>("img_iconGlow").sprite = toggleIconGlow;
                newSlot.Get<Image>("img_iconGlow").gameObject.SetActive(false);
                ___gradeSlots.Add(newSlot);
            }
        }

        [HarmonyPatch(typeof(UIStoryProgressPanel), "SetStoryLine")]
        public static class SetStoryLinePatch
        {
            public static void Postfix(UIStoryProgressPanel __instance)
            {
                ScrollRect scrollRect = __instance.Get<ScrollRect>("scroll_viewPort");
                if (!scrollRect.GetComponent<CanvasGroup>())
                    scrollRect.gameObject.AddComponent<CanvasGroup>();
                ClickPatch.canvasGroup = scrollRect.GetComponent<CanvasGroup>();
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
            public static CanvasGroup canvasGroup;
            public static bool lockToggle = false;

            public static bool Prefix(UIStoryGradeFilterSlot __instance, UIStoryGradeFilter ___storyGradeFilter)
            {
                if (__instance.transform.name != TOGGLE_NAME)
                    return true;

                if (!lockToggle)
                {
                    UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);

                    // TODO: 클릭시 이벤트 구현
                    if (__instance.IsOn)
                    {
                        Alarms.ShowAlarm("이전의 여정에 새로운 가능성을 부여하시겠습니까?", e =>
                        {
                            if (e)
                            {
                                SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Creature/KnightOfDespair_Groggy", false, 1, null);
                                Transform camera = __instance.transform.root.GetChildsByName(name => name.Contains("Camera")).First();
                                StaticCoroutine.Run(ChangeMapCo(camera.GetComponent<Camera>(), true));
                            }
                            else
                            {
                                __instance.SetToggleOn(false);
                            }
                        });
                    }
                    else
                    {
                        SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Creature/KnightOfDespair_Groggy", false, 1, null);
                        Transform camera = __instance.transform.root.GetChildsByName(name => name.Contains("Camera")).First();
                        StaticCoroutine.Run(ChangeMapCo(camera.GetComponent<Camera>(), false));
                    }
                }
                return false;
            }

            public static IEnumerator ChangeMapCo(Camera camera, bool flag)
            {
                CameraFilterPack_Broken_Screen filter1 = camera.gameObject.AddComponent<CameraFilterPack_Broken_Screen>();
                CameraFilterPack_Broken_Screen filter2 = camera.gameObject.AddComponent<CameraFilterPack_Broken_Screen>();

                lockToggle = true;
                filter1.Fade = 0.7f;
                filter2.Fade = 0.7f;
                yield return new WaitForSeconds(2);

                for (float time = 0; time <= 1; time += Time.deltaTime)
                {
                    canvasGroup.alpha = Mathf.Clamp01((flag ? 0 : 1) + (flag ? 1 : -1) * (1 - time) * (1 - time));
                    filter1.Fade = filter2.Fade = Mathf.Clamp01(0.7f * (1 - time * time));
                    yield return null;
                }
                UnityEngine.Object.Destroy(filter1);
                UnityEngine.Object.Destroy(filter2);
                lockToggle = false;
            }
        }
    }
}
