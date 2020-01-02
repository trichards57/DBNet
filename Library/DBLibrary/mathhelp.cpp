#include "pch.h"
#include "mathhelp.h"

float Angle(float x1, float y1, float x2, float y2)
{
	float dx = x2 - x1;
	float dy = y2 - y1;

	if (dx == 0)
	{
		if (dy >= 0)
			return (float)M_PI_2 * 3;
		return (float)M_PI_2;
	}
	else {
		if (dx < 0)
			return atan(dy / dx);
		return atan(dy / dx) + (float)M_PI;
	}
}

float AngleDifference(float a1, float a2) {
	float diff = a1 - a2;

	if (diff > M_PI)
		diff = (-2 * (float)M_PI - diff);
	if (diff < -M_PI)
		diff += 2 * (float)M_PI;

	return diff;
}

float AngleNormalise(float a) {
	while (a < 0)
		a += (float)M_PI * 2;
	while (a > M_PI * 2)
		a -= (float)M_PI * 2;

	return a;
}