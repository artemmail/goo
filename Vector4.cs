using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Goo
{
	public struct Vector4
	{
		public double X, Y, Z, W;

		public Vector4(double x, double y, double z, double w)
		{
			X = x; Y = y; Z = z; W = w;
		}
		public Vector4(uint color)
		{
			Z = (color & 0xff) / 255.0;
			Y = ((color >> 8) & 0xff) / 255.0;
			X = ((color >> 16) & 0xff) / 255.0;
			W = (color >> 24) / 255.0;
		}

		public uint ToColor()
		{
			return (uint)(Z * 255) | ((uint)(Y * 255) << 8) | ((uint)(X * 255) << 16) | ((uint)(W * 255) << 24);
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}
		public static Vector4 operator *(Vector4 a, double b)
		{
			return new Vector4(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}
	}
}
