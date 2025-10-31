using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System;

namespace Goo
{
	public class SliderMinMax : System.Attribute
	{
		public SliderMinMax(int minValue, int maxValue)
		{
			m_MinValue = minValue;
			m_MaxValue = maxValue;
		}
		private int m_MinValue;
		public int MinValue
		{
			get { return m_MinValue; }
		}
		private int m_MaxValue;
		public int MaxValue
		{
			get { return m_MaxValue; }
		}
	}

	public class SlideEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService wfes = provider.GetService(
								typeof(IWindowsFormsEditorService)) as
								IWindowsFormsEditorService;

			if (wfes != null)
			{
				TrackBar trackBar = new TrackBar();
				SliderMinMax minMax = null;
				trackBar.Minimum = 0;
				trackBar.Maximum = 100;
				foreach (System.Attribute attr in context.PropertyDescriptor.Attributes)
				{
					minMax = attr as SliderMinMax;
					if (minMax != null)
					{
						trackBar.Minimum = minMax.MinValue;
						trackBar.Maximum = minMax.MaxValue;
						break;
					}
				}
				trackBar.Width = 150;
				trackBar.TickFrequency = 10;
				trackBar.Value = Math.Max(Math.Min(System.Convert.ToInt32(value), trackBar.Maximum), trackBar.Minimum);
				wfes.DropDownControl(trackBar);
				return trackBar.Value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
