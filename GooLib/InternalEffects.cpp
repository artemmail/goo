#include "StdAfx.h"
#include "InternalEffects.h"

namespace InternalEffects
{
	class NoFilter
	{
	public:
		static GR_FORCEINLINE void Filter(grColor* pDst, const grColor* pSrc, int width, int height, GR_FLOAT x, GR_FLOAT y)
		{
			int si = (int)grFloor(y + GR_FLOAT(0.5));
			int sj = (int)grFloor(x + GR_FLOAT(0.5));
			if (si < 0) si = 0;
			else if (si >= height) si = height - 1;
			if (sj < 0) sj = 0;
			else if (sj >= width) sj = width - 1;
			const grColor* pS = pSrc + si * width;
			*pDst = pS[sj];
		}
	};

	class LinearFilter
	{
	public:
		static GR_FORCEINLINE void Filter(grColor* pDst, const grColor* pSrc, int width, int height, GR_FLOAT x, GR_FLOAT y)
		{
			GR_FLOAT floorX = grFloor(x);
			GR_FLOAT floorY = grFloor(y);
			GR_FLOAT w00, w01, w10, w11;
			w10 = w11 = x - floorX;
			w00 = w01 = 1 - w10;
			GR_FLOAT rem = y - floorY;
			w01 *= rem; w11 *= rem;
			rem = 1 - rem;
			w00 *= rem; w10 *= rem;
			int si = (int)floorY;
			int sj = (int)floorX;
			int si1 = si + 1;
			int sj1 = sj + 1;
			if (si < 0) si = 0;
			else if (si >=  height) si = height - 1;
			if (sj < 0) sj = 0;
			else if (sj >=  width) sj = width - 1;
			if (si1 < 0) si1 = 0;
			else if (si1 >=  height) si1 = height - 1;
			if (sj1 < 0) sj1 = 0;
			else if (sj >=  width) sj = width - 1;
			grVector4f v =
				grVector4f(pSrc[si * width + sj]) * w00 +
				grVector4f(pSrc[si * width + sj1]) * w10 +
				grVector4f(pSrc[si1 * width + sj]) * w01 +
				grVector4f(pSrc[si1 * width + sj1]) * w11;
			*pDst = v.ToArgb();
		}
	};

	template<class TFilter>
	void SpikesT(grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R, int k)
	{
		GR_FLOAT maxAmp = R * GR_FLOAT(0.5);
		GR_FLOAT Amp = shift.GetLength() / R;
		grVector2f dirY = Amp != 0 ? shift.GetUnit() : grVector2f(0, 1);
		grVector2f dirX(dirY.y, -dirY.x);
		GR_FLOAT scaleY = Amp > maxAmp ? Amp / maxAmp : 1;
		for (int i = 0; i < height; i++, pDst = pDst + width)
		{
			grColor* pD = pDst;
			for (int j = 0; j < width; j++, pD++)
			{
				GR_FLOAT x = (GR_FLOAT)j;
				GR_FLOAT y = (GR_FLOAT)i;

				GR_FLOAT dx = x - pos.x;
				GR_FLOAT dy = y - pos.y;
				GR_FLOAT a = grAtan2(dy, dx);
				GR_FLOAT r = grSqrt(dx * dx + dy * dy);

				// Global
				//r /= 1 - Amp * grSin(k * a);

				// Local
				if (r < R)
				{
					GR_FLOAT S = GR_FLOAT(1.0001) - Amp * grSin(k * a);
					r = R * (grSqrt(S * S + 4 * r / R * (1 - S)) - S) / (2 * (1 - S));
				}

				x = r * grCos(a) + pos.x;
				y = r * grSin(a) + pos.y;

				TFilter::Filter(pD, pSrc, width, height, x, y);
			}
		}
	}
	void Spikes(bool filter, grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R, int k)
	{
		if (filter)
			SpikesT<LinearFilter>(pDst, pSrc, width, height, pos, shift, R, k);
		else
			SpikesT<NoFilter>(pDst, pSrc, width, height, pos, shift, R, k);
	}

	template<class TFilter>
	void SmearT(grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R)
	{
		GR_FLOAT maxAmp = R * GR_FLOAT(0.5);
		GR_FLOAT Amp = shift.GetLength();
		grVector2f dirY = Amp != 0 ? shift.GetUnit() : grVector2f(0, 1);
		grVector2f dirX(dirY.y, -dirY.x);
		GR_FLOAT scaleY = Amp > maxAmp ? Amp / maxAmp : 1;
		for (int i = 0; i < height; i++, pDst = pDst + width)
		{
			grColor* pD = pDst;
			for (int j = 0; j < width; j++, pD++)
			{
				GR_FLOAT x = (GR_FLOAT)j;
				GR_FLOAT y = (GR_FLOAT)i;

				GR_FLOAT dx = x - pos.x;
				GR_FLOAT dy = y - pos.y;
				GR_FLOAT xx = dx * dirX.x + dy * dirX.y;
				GR_FLOAT yy = dx * dirY.x + dy * dirY.y;
				if (grAbs(xx) < R && grAbs(yy) < scaleY * R)
				{
					GR_FLOAT ae = 3 * xx / R;
					ae = scaleY * Amp / scaleY * grExp(-ae * ae);
					GR_FLOAT signY = yy >= ae ? GR_FLOAT(1.0) : GR_FLOAT(-1.0);
					yy = (yy - ae) / (1 - signY * ae / (scaleY * R));
					grVector2f v = dirX * xx + dirY * yy;
					x = pos.x + v.x;
					y = pos.y + v.y;
				}

				TFilter::Filter(pD, pSrc, width, height, x, y);
			}
		}
	}

	void Smear(bool filter, grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R)
	{
		if (filter)
			SmearT<LinearFilter>(pDst, pSrc, width, height, pos, shift, R);
		else
			SmearT<NoFilter>(pDst, pSrc, width, height, pos, shift, R);
	}
}