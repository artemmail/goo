using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Goo
{
	public class Spikes : BaseEffect
	{
		private int _Count = 16;
		[Editor(typeof(SlideEditor), typeof(UITypeEditor)), SliderMinMax(1, 20)]
		public int Count
		{
			get { return _Count; }
			set { _Count = value; }
		}

		public override void Apply(bool filter, System.Drawing.Bitmap dstBmp, System.Drawing.Bitmap srcBmp)
		{
			GooLib.Effects.Skipes(filter, dstBmp, srcBmp, Position, new System.Drawing.PointF((float)Shift.X, (float)Shift.Y), Radius, Count);
		}
	}
}
