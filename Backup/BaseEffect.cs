using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;

namespace Goo
{
	public abstract class BaseEffect
	{
		private PointF _Position = new PointF(300, 200);
		[Browsable(false)]
		public PointF Position
		{
			get { return _Position; }
			set { _Position = value; }
		}

		private float _Radius = 50;
		[Editor(typeof(SlideEditor), typeof(UITypeEditor)), SliderMinMax(1, 200)]
		public float Radius
		{
			get { return _Radius; }
			set { _Radius = value; }
		}

		private Vector2 _Shift = new Vector2(0, 0);
		[Browsable(false)]
		public Vector2 Shift
		{
			get { return _Shift; }
			set { _Shift = value; }
		}

		public abstract void Apply(bool filter, Bitmap dstBmp, Bitmap srcBmp);
	}
}
