using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Goo
{
	public class Smear : BaseEffect
	{
		public override void Apply(bool filter, Bitmap dstBmp, Bitmap srcBmp)
		{
			GooLib.Effects.Smear(filter, dstBmp, srcBmp, Position, new PointF((float)Shift.X, (float)Shift.Y), (float)Radius);
		}
	}
}
