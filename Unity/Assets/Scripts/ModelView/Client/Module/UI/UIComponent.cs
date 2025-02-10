using System.Collections.Generic;

namespace ET.Client
{
	/// <summary>
	/// 管理Scene上的UI
	///
	/// 这里仅定义UIComponent，
	/// 具体实现在 HotfixView/Client/Module/UI/UIComponentSystem.cs
	/// </summary>
	[ComponentOf]
	public class UIComponent: Entity, IAwake
	{
		public Dictionary<string, EntityRef<UI>> UIs = new();

		private EntityRef<UIGlobalComponent> uiGlobalComponent;

		public UIGlobalComponent UIGlobalComponent
		{
			get
			{
				return this.uiGlobalComponent;
			}
			set
			{
				this.uiGlobalComponent = value;
			}
		}
	}
}