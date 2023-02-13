using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace ChapterReversalMod.Utils
{
    public static class Chapters
    {
        public static StageClassInfo AsStageClassInfo(this LorId id)
        {
            return Singleton<StageClassInfoList>.Instance.GetAllWorkshopData()[id.packageId].Find(s => s.id == id);
        }

        /// <param name="books">Id of books needed to do the chapter</param>
        public static StageClassInfo SetRecipe(this StageClassInfo stage, List<LorId> books)
        {
            stage.invitationInfo.combine = StageCombineType.BookRecipe;
            stage.invitationInfo.needsBooks = books;
            return stage;
        }

        /// <param name="level">Minimum library level to do the chapter</param>
        public static StageClassInfo SetNeededLevel(this StageClassInfo stage, int level)
        {
            stage.ChangeExtraCondition();
            stage.extraCondition.needLevel = level;
            return stage;
        }

        /// <param name="stages">Id of stages needed to do the chapter</param>
        public static StageClassInfo SetNeededStages(this StageClassInfo stage, List<LorId> stages)
        {
            stage.ChangeExtraCondition();
            ((StageExtraConditionEx)stage.extraCondition).needClearStageListLorId = stages;
            return stage;
        }

        private static StageClassInfo ChangeExtraCondition(this StageClassInfo stage)
        {
            if (stage.extraCondition is StageExtraConditionEx)
                return stage;
            StageExtraCondition extraCondition = new StageExtraConditionEx();
            extraCondition.needLevel = stage.extraCondition.needLevel;
            extraCondition.needClearStageList = stage.extraCondition.needClearStageList;
            stage.extraCondition = extraCondition;
            return stage;
        }

        /// <param name="stages">Stages  in the chapter.</param>
        /// <param name="icon">Icon of the chapter.</param>
        /// <param name="position">Position of the chapter</param>
        public static void CreateChapter(List<StageClassInfo> stages, Sprite icon, Vector2 position)
        {
            StaticCoroutine.Run(CreateChapterCo(stages, icon, position));
        }

        private static IEnumerator CreateChapterCo(List<StageClassInfo> stages, Sprite icon, Vector2 position)
        {
            yield return new WaitUntil(() => LibraryModel.Instance.ClearInfo != null);
            string id = stages[0].id.ToString();
            stages.ForEach(s => s._storyType = id);
            stages.Where(s => s.extraCondition == null).Select(ChangeExtraCondition);
            UIIconManager.IconSet iconset = new UIIconManager.IconSet();
            iconset.icon = icon;
            iconset.iconGlow = icon;
            UISpriteDataManager.instance.Get<Dictionary<string, UIIconManager.IconSet>>("StoryIconDic")[id] = iconset;

            UIStoryProgressPanel panel = ((UIInvitationPanel)UI.UIController.Instance.GetUIPanel(UIPanelType.Invitation)).InvCenterStoryPanel;
            Transform parent = panel.Get<RectTransform>("posRect").Find("Icons").FindOrAdd("CustomChapters");
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;

            UIStoryProgressIconSlot origin = panel.Get<List<UIStoryProgressIconSlot>>("iconList")[0];
            UIStoryProgressIconSlot copy = Object.Instantiate(origin.gameObject, parent).GetComponent<UIStoryProgressIconSlot>();
            copy.transform.localPosition = position;
            copy.Set("glowOriginalColor", origin.Get("glowOriginalColor"));
            copy.Set("originalcolor", origin.Get("originalcolor"));
            copy.Set("StoryProgressPanel", panel);
            copy.SetSlotData(stages);
        }

        private class StageExtraConditionEx : StageExtraCondition
        {
            public new bool IsUnlocked()
            {
                if (LibraryModel.Instance.GetLibraryLevel() < needLevel)
                {
                    return false;
                }
                foreach (LorId stageId in needClearStageListLorId)
                {
                    if (LibraryModel.Instance.ClearInfo.GetClearCount(stageId) <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }

            public List<LorId> needClearStageListLorId = new List<LorId>();
        }
    }
}
