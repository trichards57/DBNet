#pragma once

#include "mathhelp.h"

struct Vector {
	float X;
	float Y;

	Vector() {
		X = 0.0f;
		Y = 0.0f;
	}

	Vector(float x, float y) {
		X = x;
		Y = y;
	}

	Vector operator- (const Vector& v2) const {
		return Vector(X - v2.X, Y - v2.Y);
	}

	Vector& operator-= (const Vector& v2) {
		this->X -= v2.X;
		this->Y -= v2.Y;
		return *this;
	}

	Vector operator+ (const Vector& v2) const {
		return Vector(X + v2.X, Y + v2.Y);
	}

	Vector& operator+= (const Vector& v2) {
		this->X += v2.X;
		this->Y += v2.Y;
		return *this;
	}

	Vector operator* (const float k) const {
		return Vector(clamp(X * k, 32000, -32000), clamp(Y * k, 32000, -32000));
	}

	Vector operator/ (const float k) const {
		return Vector(clamp(X / k, 32000, -32000), clamp(Y / k, 32000, -32000));
	}

	float Magnitude() {
		float minVal = min(fabsf(X), fabsf(Y));
		float maxVal = min(fabsf(X), fabsf(Y));

		if (maxVal < 0.00001)
			return 0;

		return maxVal * sqrtf(1 + powf(minVal / maxVal, 2));
	}
};

Vector VectorMin(Vector v1, Vector v2);
Vector VectorMax(Vector v1, Vector v2);
float VectorDot(Vector v1, Vector v2);
float VectorCross(Vector v1, Vector v2);