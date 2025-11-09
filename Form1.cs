using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Goo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

                        _SrcBmp = LoadSourceBitmap();
                        _SrcBmp = EnsureBitmapFormat(_SrcBmp);
                        _DstBmp = CreateDestinationBitmap(_SrcBmp);
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

                private Bitmap LoadSourceBitmap()
                {
                        try
                        {
                                if (File.Exists(_SrcBmpFile))
                                {
                                        return new Bitmap(_SrcBmpFile);
                                }

                                MessageBox.Show(
                                        $"Файл '{_SrcBmpFile}' не найден. Будет создано временное изображение.",
                                        Text,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                                MessageBox.Show(
                                        $"Не удалось загрузить '{_SrcBmpFile}'.\nПричина: {ex.Message}\nБудет создано временное изображение.",
                                        Text,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        }

                        return CreateFallbackBitmap(pictureBox.Width, pictureBox.Height);
                }

                private static Bitmap CreateFallbackBitmap(int width, int height)
                {
                        width = Math.Max(1, width);
                        height = Math.Max(1, height);

                        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                Rectangle rect = new Rectangle(0, 0, width, height);

                                using (LinearGradientBrush brush = new LinearGradientBrush(
                                        rect,
                                        Color.FromArgb(64, 130, 202),
                                        Color.FromArgb(31, 31, 31),
                                        LinearGradientMode.ForwardDiagonal))
                                {
                                        g.FillRectangle(brush, rect);
                                }

                                using (Pen pen = new Pen(Color.FromArgb(180, Color.White), 3))
                                {
                                        g.DrawRectangle(pen, 10, 10, width - 20, height - 20);
                                }

                                string text = "goo.bmp отсутствует";
                                using (Font font = new Font(FontFamily.GenericSansSerif, Math.Max(12, Math.Min(width, height) / 15f), FontStyle.Bold))
                                {
                                        SizeF textSize = g.MeasureString(text, font);
                                        PointF location = new PointF((width - textSize.Width) / 2, (height - textSize.Height) / 2);
                                        using (Brush textBrush = new SolidBrush(Color.WhiteSmoke))
                                        {
                                                g.DrawString(text, font, textBrush, location);
                                        }
                                }
                        }

                        return bmp;
                }

                private static Bitmap EnsureBitmapFormat(Bitmap bitmap)
                {
                        if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                                return bitmap;
                        }

                        Bitmap converted = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(converted))
                        {
                                g.DrawImage(bitmap, Point.Empty);
                        }

                        bitmap.Dispose();

                        return converted;
                }

                private static Bitmap CreateDestinationBitmap(Bitmap source)
                {
                        Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(destination))
                        {
                                g.DrawImage(source, Point.Empty);
                        }

                        return destination;
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