#ifndef __grTypes__
#define __grTypes__

#include "math.h"

#define GR_FORCEINLINE __forceinline
#define GR_INLINE __inline
#define GR_FLOAT float
#define GR_USHORT unsigned short
#define GR_BYTE unsigned char

#define GR_PI ((GR_FLOAT)(3.14159265359f))
#define GrToDegrees(x) ((GR_FLOAT)(x) / GR_PI * 180)
#define GrToRadians(x) ((GR_FLOAT)(x) / 180 * GR_PI)

namespace Gradients
{
	typedef unsigned int grColor;

	template<typename T>
	GR_FORCEINLINE T grMin(T a, T b)
	{
		return a < b ? a : b;
	}
	template<typename T>
	GR_FORCEINLINE T grMax(T a, T b)
	{
		return a > b ? a : b;
	}
	template<typename T>
	GR_FORCEINLINE T grAbs(T x)
	{
		return x > 0 ? x : -x;
	}
	template<typename T>
	GR_FORCEINLINE T grSign(T x)
	{
		if (x >= 0) return 1;
		else return -1;
	}
	template<typename T>
	GR_FORCEINLINE T grSaturate(T x)
	{
		if (x <= 0) return 0;
		else if (x >= 1) return 1;
		return x;
	}

	#define grFloor(x) floorf(x)
	#define grCeil(x) ceilf(x)
	#define grSqrt(x) sqrtf(x)
	#define grSin(x) sinf(x)
	#define grCos(x) cosf(x)
	#define grAtan2(y, x) atan2f(y, x)
	#define grExp(x) expf(x)
	#define grLog(x) logf(x)

	struct grVector4f
	{
		GR_FLOAT x, y, z, w;

		GR_FORCEINLINE grVector4f() { }
		GR_FORCEINLINE grVector4f(GR_FLOAT c) : x(c), y(c), z(c), w(c) { }
		GR_FORCEINLINE grVector4f(GR_FLOAT _x, GR_FLOAT _y, GR_FLOAT _z, GR_FLOAT _w) : x(_x), y(_y), z(_z), w(_w) { }
		GR_FORCEINLINE grVector4f(grColor col)
			: x((col & 0xff) / 255.0f), y(((col >> 8) & 0xff) / 255.0f)
			, z(((col >> 16) & 0xff) / 255.0f), w((col >> 24) / 255.0f)
		{
		}
		GR_FORCEINLINE grColor ToArgb()
		{
			return (int)(x * 255) | ((int)(y * 255) << 8) | ((int)(z * 255) << 16) | ((int)(w * 255) << 24);
		}
		GR_FORCEINLINE const grVector4f operator+=(const grVector4f& v)
		{
			x += v.x; y += v.y; z += v.z; w += v.w;
			return *this;
		}
		GR_FORCEINLINE grVector4f operator+(const grVector4f& v) const
		{
			return grVector4f(x + v.x, y + v.y, z + v.z, w + v.w);
		}
		GR_FORCEINLINE grVector4f operator-(const grVector4f& v) const
		{
			return grVector4f(x - v.x, y - v.y, z - v.z, w - v.w);
		}
		GR_FORCEINLINE grVector4f operator*(const grVector4f& v) const
		{
			return grVector4f(x * v.x, y * v.y, z * v.z, w * v.w);
		}
		GR_FORCEINLINE grVector4f operator/(const grVector4f& v) const
		{
			return grVector4f(x / v.x, y / v.y, z / v.z, w / v.w);
		}
		GR_FORCEINLINE grVector4f operator*(GR_FLOAT c) const
		{
			return grVector4f(c * x, c * y, c * z, c * w);
		}
		GR_FORCEINLINE grVector4f operator-() const
		{
			return grVector4f(-x, -y, -z, -w);
		}
		GR_FORCEINLINE GR_FLOAT& operator[] (int i)
		{
			return ((GR_FLOAT*)this)[i];
		}
		GR_FORCEINLINE GR_FLOAT operator[] (int i) const
		{
			return ((GR_FLOAT*)this)[i];
		}
		GR_FORCEINLINE grVector4f GetMin(const grVector4f& v) const
		{
			return grVector4f(grMin(x, v.x), grMin(y, v.y), grMin(z, v.z), grMin(w, v.w));
		}
		GR_FORCEINLINE grVector4f GetMax(const grVector4f& v) const
		{
			return grVector4f(grMax(x, v.x), grMax(y, v.y), grMax(z, v.z), grMax(w, v.w));
		}
		GR_FORCEINLINE grVector4f GetInterpolated(const grVector4f& toV, GR_FLOAT t) const
		{
			return *this * (1 - t) + toV * t;
		}
		GR_FORCEINLINE grVector4f GetAbs() const
		{
			return grVector4f(grAbs(x), grAbs(y), grAbs(z), grAbs(w));
		}
		GR_FORCEINLINE grVector4f GetSign() const
		{
			return grVector4f(grSign(x), grSign(y), grSign(z), grSign(w));
		}
		GR_FORCEINLINE void Saturate()
		{
			x = grSaturate(x);
			y = grSaturate(y);
			z = grSaturate(z);
			w = grSaturate(w);
		}

		static grVector4f one;
		static grVector4f half;
	};
	struct grVector2f
	{
		GR_FLOAT x, y;

		grVector2f() { }
		GR_FORCEINLINE grVector2f(GR_FLOAT _x, GR_FLOAT _y) : x(_x), y(_y) { }

		GR_FORCEINLINE grVector2f operator+(const grVector2f& v) const
		{
			return grVector2f(x + v.x, y + v.y);
		}
		GR_FORCEINLINE grVector2f operator-(const grVector2f& v) const
		{
			return grVector2f(x - v.x, y - v.y);
		}
		GR_FORCEINLINE grVector2f operator*(GR_FLOAT c) const
		{
			return grVector2f(c * x, c * y);
		}
		GR_FORCEINLINE grVector2f operator/(GR_FLOAT c) const
		{
			return grVector2f(x / c, y / c);
		}
		GR_FORCEINLINE const grVector2f& operator+=(const grVector2f v)
		{
			x += v.x;
			y += v.y;
			return *this;
		}
		GR_FORCEINLINE const grVector2f& operator*=(GR_FLOAT c)
		{
			x *= c;
			y *= c;
			return *this;
		}
		GR_FORCEINLINE grVector2f operator*(const grVector2f& v) const
		{
			return grVector2f(x * v.x, y * v.y);
		}
		GR_FORCEINLINE grVector2f operator/(const grVector2f& v) const
		{
			return grVector2f(x / v.x, y / v.y);
		}
		GR_FORCEINLINE bool operator!=(const grVector2f& v) const
		{
			return x != v.x || y != v.y;
		}
		GR_FORCEINLINE grVector2f operator-() const
		{
			return grVector2f(-x, -y);
		}
		GR_FORCEINLINE GR_FLOAT GetLength() const
		{
			return sqrtf(x * x + y * y);
		}
		GR_FORCEINLINE grVector2f GetUnit() const
		{
			return *this * (1.0f / GetLength());
		}
		GR_FORCEINLINE GR_FLOAT GetDot(const grVector2f& v) const
		{
			return x * v.x + y * v.y;
		}
		GR_FORCEINLINE GR_FLOAT GetCross(const grVector2f& v) const
		{
			return x * v.y - y * v.x;
		}
		GR_FORCEINLINE grVector2f GetInterpolated(const grVector2f& toV, GR_FLOAT t) const
		{
			return *this * (1 - t) + toV * t;
		}
		GR_FORCEINLINE grVector2f GetRotated(GR_FLOAT angle) const
		{
			return grVector2f(x * grCos(angle) - y * grSin(angle), x * grSin(angle) + y * grCos(angle));
		}
	};

	struct grMatrix3f
	{
			GR_FLOAT
				m00, m01, m02,
				m10, m11, m12,
				m20, m21, m22;

		static grMatrix3f identity;

		grMatrix3f() { }
		grMatrix3f(GR_FLOAT _m00, GR_FLOAT _m01, GR_FLOAT _m02,
			GR_FLOAT _m10, GR_FLOAT _m11, GR_FLOAT _m12,
			GR_FLOAT _m20, GR_FLOAT _m21, GR_FLOAT _m22)
			: m00(_m00), m01(_m01), m02(_m02), m10(_m10), m11(_m11), m12(_m12), m20(_m20), m21(_m21), m22(_m22)
		{
		}

		static grMatrix3f CreateScale(GR_FLOAT sx, GR_FLOAT sy)
		{
			grMatrix3f res = identity;
			res.m00 = sx; res.m11 = sy;
			return res;
		}
		static grMatrix3f CreateTranslate(GR_FLOAT dx, GR_FLOAT dy)
		{
			grMatrix3f res = identity;
			res.m20 = dx; res.m21 = dy;
			return res;
		}
		static grMatrix3f CreateRotate(GR_FLOAT angle)
		{
			grMatrix3f res = identity;
			GR_FLOAT sin_a = grSin(angle);
			GR_FLOAT cos_a = grCos(angle);
			res.m00 = res.m11 = cos_a;
			res.m01 = sin_a;
			res.m10 = -sin_a;
			return res;
		}

		grMatrix3f operator*(const grMatrix3f& m) const
		{
			grMatrix3f res;
			res.m00 = m00 * m.m00 + m01 * m.m10 + m02 * m.m20;
			res.m01 = m00 * m.m01 + m01 * m.m11 + m02 * m.m21;
			res.m02 = m00 * m.m02 + m01 * m.m12 + m02 * m.m22;
			res.m10 = m10 * m.m00 + m11 * m.m10 + m12 * m.m20;
			res.m11 = m10 * m.m01 + m11 * m.m11 + m12 * m.m21;
			res.m12 = m10 * m.m02 + m11 * m.m12 + m12 * m.m22;
			res.m20 = m20 * m.m00 + m21 * m.m10 + m22 * m.m20;
			res.m21 = m20 * m.m01 + m21 * m.m11 + m22 * m.m21;
			res.m22 = m20 * m.m02 + m21 * m.m12 + m22 * m.m22;
			return res;
		}
	};

	__inline grVector2f operator*(const grVector2f v, const grMatrix3f& m)
	{
		grVector2f res;
		GR_FLOAT w = 1.0f / (v.x * m.m02 + v.y * m.m12 + m.m22);
		res.x = (v.x * m.m00 + v.y * m.m10 + m.m20) * w;
		res.y = (v.x * m.m01 + v.y * m.m11 + m.m21) * w;
		return res;
	}

	struct grRect
	{
		GR_FLOAT x, y;
		GR_FLOAT width, height;

		grRect() { }
		grRect(GR_FLOAT _x, GR_FLOAT _y, GR_FLOAT _width, GR_FLOAT _height)
			: x(_x), y(_y), width(_width), height(_height)
		{
		}
		grRect(const grVector2f& _pos, const grVector2f& _size)
		{
			pos() = _pos;
			size() = _size;
		}
		GR_FORCEINLINE const grVector2f& pos() const { return *(const grVector2f*)&x; }
		GR_FORCEINLINE grVector2f& pos() { return *(grVector2f*)&x; }
		GR_FORCEINLINE grVector2f other_pos() const { return pos() + size(); }
		GR_FORCEINLINE grVector2f center() const { return pos() + size() * 0.5f; }
		GR_FORCEINLINE const grVector2f& size() const { return *(const grVector2f*)&width; }
		GR_FORCEINLINE grVector2f& size() { return *(grVector2f*)&width; }
		GR_FORCEINLINE grRect GetClipped(const grRect& view) const
		{
			grVector2f other = other_pos();
			grVector2f view_other = view.other_pos();
			grVector2f p = grVector2f(grMax(x, view.x), grMax(y, view.y));
			grVector2f s;
			if (view_other.x < other.x) s.x = view_other.x - p.x;
			else s.x = other.x - p.x;
			if (view_other.y < other.y) s.y = view_other.y - p.y;
			else s.y = other.y - p.y;
			return grRect(p, s);
		}
		GR_FORCEINLINE void ToInt()
		{
			x = (float)(int)x;
			y = (float)(int)y;
			width = (float)(int)width;
			height = (float)(int)height;
		}
		GR_FORCEINLINE bool IsEmpty() const
		{
			return width <= 0 || height <= 0;
		}
		GR_FORCEINLINE grRect GetShifted(const grVector2f& shift) const
		{
			return grRect(pos() + shift, size());
		}
	};

	template<typename T>
	GR_FORCEINLINE T* grShiftPtr(T* ptr, int shift) { return reinterpret_cast<T*>(reinterpret_cast<unsigned char*>(ptr) + shift); }
	template<typename T>
	GR_FORCEINLINE const T* grShiftCPtr(const T* ptr, int shift) { return reinterpret_cast<const T*>(reinterpret_cast<const unsigned char*>(ptr) + shift); }
}

#endif // __grTypes__