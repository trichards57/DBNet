#pragma once

#define clamp(x, upper, lower) (min(upper, max(x, lower)))
#define angleToInt(x) ((int)(x*200))
#define intToAngle(x) (((float)x)/200)

float Angle(float x1, float y1, float x2, float y2);
float AngleDifference(float a1, float a2);
float AngleNormalise(float a);
template <typename T> int sgn(T val) {
	return (T(0) < val) - (val < T(0));
}
