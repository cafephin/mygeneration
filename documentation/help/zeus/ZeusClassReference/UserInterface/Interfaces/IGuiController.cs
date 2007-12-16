using System;
using System.Collections;

namespace Zeus.UserInterface
{
	public interface IGuiController : IGuiControl, IZeusGuiControl, IEnumerable
	{
		GuiCheckBoxList AddCheckBoxList(string id, string tooltip);
		GuiGrid AddGrid(string id, string tooltip);
	}
}