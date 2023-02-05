using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace ChapterReversalMod.Utils
{
	public static class Alarms
	{
		public static void ShowAlarm(string text, ConfirmEvent evnt = null)
		{
			UIAlarmPopup popup = UIAlarmPopup.instance;
			if (popup.IsOpened())
			{
				popup.Close();
			}
			if (popup.Get<GameObject>("ob_blue").activeSelf)
			{
				popup.Get<GameObject>("ob_blue").SetActive(false);
			}
			if (!popup.Get<GameObject>("ob_normal").activeSelf)
			{
				popup.Get<GameObject>("ob_normal").gameObject.SetActive(true);
			}
			if (popup.Get<GameObject>("ob_Reward").activeSelf)
			{
				popup.Get<GameObject>("ob_Reward").SetActive(false);
			}
			popup.Set("currentAnimState", Reflections.GetType("UI.UIAlarmPopup.UIAlarmAnimState").Get("Normal"));
			popup.Set("currentmode", AnimatorUpdateMode.UnscaledTime);
			foreach (GameObject gameObject in popup.Get<List<GameObject>>("ButtonRoots"))
			{
				gameObject.gameObject.SetActive(false);
			}
			if (popup.Get<GameObject>("ob_BlackBg").activeSelf)
			{
				popup.Get<GameObject>("ob_BlackBg").SetActive(false);
			}
			popup.Set("currentAlarmType", UIAlarmType.Default);
			popup.Get<TextMeshProUGUI>("txt_alarm").text = text;
			popup.Set("_confirmEvent", evnt);
			popup.Get<List<GameObject>>("ButtonRoots")[(int)UIAlarmButtonType.YesNo].gameObject.SetActive(true);
			popup.Open();
			UIControlManager.Instance.SelectSelectableForcely(popup.Get<UICustomSelectable>("yesButton"), false);
		}
	}
}
