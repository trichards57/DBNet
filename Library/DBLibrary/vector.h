#pragma once

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
};

Vector VectorMin(Vector v1, Vector v2);
Vector VectorMax(Vector v1, Vector v2);