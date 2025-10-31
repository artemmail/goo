// GooLib.h

#pragma once

#pragma unmanaged

#include "InternalEffects.h"

#pragma managed


using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;

namespace GooLib {

	public ref class Effects
	{
		ref class Locker
		{
		private:
			BitmapData^ _srcData;
			BitmapData^ _dstData;
			Bitmap^ _dstBmp;
			Bitmap^ _srcBmp;
		public:
			Locker(Bitmap^ dstBmp, Bitmap^ srcBmp)
				: _dstBmp(dstBmp), _srcBmp(srcBmp)
			{
				_srcData = srcBmp->LockBits(Rectangle(0, 0, srcBmp->Width, srcBmp->Height), Imaging::ImageLockMode::ReadOnly, Imaging::PixelFormat::Format32bppRgb);
				_dstData = dstBmp->LockBits(Rectangle(0, 0, srcBmp->Width, srcBmp->Height), Imaging::ImageLockMode::WriteOnly, Imaging::PixelFormat::Format32bppRgb);
				pSrc = (grColor*)_srcData->Scan0.ToPointer();
				pDst = (grColor*)_dstData->Scan0.ToPointer();
			}
			~Locker()
			{
				_dstBmp->UnlockBits(_dstData);
				_srcBmp->UnlockBits(_srcData);
			}
			grColor* pDst;
			grColor* pSrc;
		};
	public:
		static void Skipes(bool filter, Bitmap^ dstBmp, Bitmap^ srcBmp, PointF pos, PointF shift, GR_FLOAT R, int k)
		{
			//BitmapData^ srcData = srcBmp->LockBits(Rectangle(0, 0, srcBmp->Width, srcBmp->Height), Imaging::ImageLockMode::ReadOnly, Imaging::PixelFormat::Format32bppRgb);
			//BitmapData^ dstData = dstBmp->LockBits(Rectangle(0, 0, srcBmp->Width, srcBmp->Height), Imaging::ImageLockMode::WriteOnly, Imaging::PixelFormat::Format32bppRgb);
			//UInt32* pSrc = (UInt32*)srcData->Scan0.ToPointer();
			//UInt32* pDst = (UInt32*)dstData->Scan0.ToPointer();
			{
				Locker lck(dstBmp, srcBmp);
				InternalEffects::Spikes(filter, lck.pDst, lck.pSrc, srcBmp->Width, srcBmp->Height, grVector2f((GR_FLOAT)pos.X, (GR_FLOAT)pos.Y),
					grVector2f((GR_FLOAT)shift.X, (GR_FLOAT)shift.Y), R, k);
			}
			//dstBmp->UnlockBits(dstData);
			//srcBmp->UnlockBits(srcData);
		}
		static void Smear(bool filter, Bitmap^ dstBmp, Bitmap^ srcBmp, PointF pos, PointF shift, GR_FLOAT R)
		{
			Locker lck(dstBmp, srcBmp);
			InternalEffects::Smear(filter, lck.pDst, lck.pSrc, srcBmp->Width, srcBmp->Height, grVector2f((GR_FLOAT)pos.X, (GR_FLOAT)pos.Y),
				grVector2f((GR_FLOAT)shift.X, (GR_FLOAT)shift.Y), R);
		}
	};
}
