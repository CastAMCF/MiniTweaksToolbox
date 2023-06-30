using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
	internal class ChecklistWindowMenuItem : MenuItemVertical, IPointerEnterHandler, IEventSystemHandler
	{
		public override void Mark(bool isMarked)
		{
			if (!this.Interactable)
			{
				return;
			}
			if (this.canPlayHoverSound && isMarked)
			{
				SoundManager.Get().PlaySFX("ButtonHover");
			}
			else
			{
				this.canPlayHoverSound = true;
			}
			if (isMarked)
			{
				base.transform.GetChild(0).GetComponent<Image>().color = GlobalData.CMS_ORANGE;
				return;
			}
			base.transform.GetChild(0).GetComponent<Image>().color = this.original;
		}

		public override bool CanMark()
		{
			return this.Interactable;
		}

		public void OnPointerEnter(PointerEventData data)
		{
			if (!this.Interactable)
			{
				return;
			}
			this.ButtonMenu.GetComponent<ButtonsWindowMenu>().SelectItem(base.gameObject);
		}

		public void SetDisabled()
		{
			this.Interactable = false;
			base.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
		}

		public void SetEnabled()
		{
			this.Interactable = true;
			base.transform.GetChild(0).GetComponent<Image>().color = this.original;
		}

		public void SetParent(ButtonsWindowMenu menu)
		{
			this.ButtonMenu = menu;
		}

		private readonly Color original = new Color(0f, 0f, 0f, 0.776f);

		public bool Interactable = true;

		public ButtonsWindowMenu ButtonMenu;

		private bool canPlayHoverSound;
	}
}
