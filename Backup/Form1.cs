using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Goo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			_SrcBmp = new Bitmap(_SrcBmpFile);
			_DstBmp = new Bitmap(_SrcBmp);
			pictureBox.Image = _DstBmp;
			_EffectList.DataSource = new BaseEffect[] { new Spikes(), new Smear() };
			_EffectList.SelectedIndexChanged += delegate
			{
				SelectedEffect = _EffectList.SelectedItem as BaseEffect;
			};
			propertyGrid.PropertyValueChanged += delegate { redraw(); };
			propertyGrid.SelectedObjectsChanged += delegate { redraw(); };
			SelectedEffect = (BaseEffect)_EffectList.Items[0];
		}

		readonly string _SrcBmpFile = "goo.bmp";

		Bitmap _SrcBmp;
		Bitmap _DstBmp;

		private BaseEffect _SelectedEffect;

		public BaseEffect SelectedEffect
		{
			get { return _SelectedEffect; }
			set
			{
				_SelectedEffect = value;
				propertyGrid.SelectedObject = value;
			}
		}


		unsafe void redraw()
		{
			if (SelectedEffect != null)
				SelectedEffect.Apply(_ChkFilter.Checked, _DstBmp, _SrcBmp);
			else
				using (Graphics gr = Graphics.FromImage(_DstBmp))
				{
					gr.DrawImage(_SrcBmp, Point.Empty);
				}
			pictureBox.Refresh();
		}

		private void pictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			SelectedEffect.Position = new PointF(e.X, e.Y);
			SelectedEffect.Shift = new Vector2(0, 0);
			redraw();
		}

		private void pictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.None)
			{
				PointF pos = SelectedEffect.Position;
				SelectedEffect.Shift = new Vector2(e.X, e.Y) - new Vector2(pos.X, pos.Y);
				redraw();
			}
		}

		private void _ChkFilter_CheckedChanged(object sender, EventArgs e)
		{
			redraw();
		}
	}
}