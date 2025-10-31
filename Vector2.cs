using System;
using System.Collections.Generic;
using System.Text;

namespace Goo
{
	public struct Vector2
	{
		public double X, Y;

		public Vector2(double x, double y)
		{
			X = x; Y = y;
		}

		public double Dot(Vector2 b)
		{
			return X * b.X + Y * b.Y;
		}

		public double LengthSquared()
		{
			return Dot(this);
		}

		public double Length()
		{
			return Math.Sqrt(LengthSquared());
		}

		public Vector2 GetUnit()
		{
			double len = Length();
			return new Vector2(X / len, Y / len);
		}

		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}
		public static Vector2 operator *(Vector2 a, double b)
		{
			return new Vector2(a.X * b, a.Y * b);
		}
	}
}
