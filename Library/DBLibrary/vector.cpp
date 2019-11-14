#include "pch.h"
#include "vector.h"

Vector VectorMin(Vector v1, Vector v2) {
	return Vector(min(v1.X, v2.X), min(v1.Y, v2.Y));
}

Vector VectorMax(Vector v1, Vector v2) {
	return Vector(max(v1.X, v2.X), max(v1.Y, v2.Y));
}