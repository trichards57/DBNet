#include "pch.h"
#include "mathhelp.h"

float Angle(float x1, float y1, float x2, float y2)
{
	float dx = x2 - x1;
	float dy = y2 - y1;

	return atan2(dy, dx);
}

float AngleDifference(float a1, float a2) {
	return AngleNormalise(a1 - a2);
}

float AngleNormalise(float a) {
	while (a < 0)
		a += (float)M_PI * 2;
	while (a > M_PI * 2)
		a -= (float)M_PI * 2;

	return a;
}
