#pragma once

#define clamp(x, upper, lower) (min(upper, max(x, lower)))

float Angle(float x1, float y1, float x2, float y2);
float AngleDifference(float a1, float a2);
float AngleNormalise(float a);
