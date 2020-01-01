#include "pch.h"
#include "mathhelp.h"

float Angle(float x1, float y1, float x2, float y2)
{
	float dx = x2 - x1;
	float dy = y2 - y1;

	if (dx == 0)
	{
		if (dy >= 0)
			return M_PI_2 * 3;
		return M_PI_2;
	}
	else {
		if (dx < 0)
			return atan(dy / dx);
		return atan(dy / dx) + M_PI;
	}
}