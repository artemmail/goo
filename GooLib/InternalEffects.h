#pragma once

#include "grTypes.h"

using namespace Gradients;

namespace InternalEffects
{
	void Spikes(bool filter, grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R, int k);
	void Smear(bool filter, grColor* pDst, const grColor* pSrc, int width, int height, const grVector2f& pos, const grVector2f& shift, GR_FLOAT R);
}