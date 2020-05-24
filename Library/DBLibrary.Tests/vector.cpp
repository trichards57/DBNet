#include "pch.h"

#include "../DBLibrary/vector.h"

TEST(Vector, HandlesSubtract) {
	Vector a(10, 20);
	Vector b(30, 50);

	Vector c = b - a;

	EXPECT_EQ(c.X, 20);
	EXPECT_EQ(c.Y, 30);
}

TEST(Vector, HandlesSubtractEquals) {
	Vector a(10, 20);
	Vector b(30, 50);

	b -= a;

	EXPECT_EQ(b.X, 20);
	EXPECT_EQ(b.Y, 30);
}

TEST(Vector, HandlesAdd) {
	Vector a(10, 20);
	Vector b(30, 50);

	Vector c = b + a;

	EXPECT_EQ(c.X, 40);
	EXPECT_EQ(c.Y, 70);
}

TEST(Vector, HandlesAddEquals) {
	Vector a(10, 20);
	Vector b(30, 50);

	b += a;

	EXPECT_EQ(b.X, 40);
	EXPECT_EQ(b.Y, 70);
}

TEST(Vector, HandlesMultiply) {
	Vector a(10, 20);

	Vector b = a * 6;

	EXPECT_EQ(b.X, 60);
	EXPECT_EQ(b.Y, 120);
}

TEST(Vector, HandlesMultiplyWithOverrun) {
	Vector a(10, 20);

	Vector b = a * 6000;

	EXPECT_EQ(b.X, 32000);
	EXPECT_EQ(b.Y, 32000);
}

TEST(Vector, HandlesMultiplyWithNegativeOverrun) {
	Vector a(10, 20);

	Vector b = a * -6000;

	EXPECT_EQ(b.X, -32000);
	EXPECT_EQ(b.Y, -32000);
}

TEST(Vector, HandlesDivide) {
	Vector a(10, 20);

	Vector b = a / 10;

	EXPECT_EQ(b.X, 1);
	EXPECT_EQ(b.Y, 2);
}

TEST(Vector, HandlesDivideWithOverrun) {
	Vector a(10, 20);

	Vector b = a / 0.00000001f;

	EXPECT_EQ(b.X, 32000);
	EXPECT_EQ(b.Y, 32000);
}

TEST(Vector, HandlesDivideWithNegativeOverrun) {
	Vector a(10, 20);

	Vector b = a / -0.00000001f;

	EXPECT_EQ(b.X, -32000);
	EXPECT_EQ(b.Y, -32000);
}

TEST(Vector, CalculatesMagnitude) {
	Vector a(3, 4);

	float magnitude = a.Magnitude();

	EXPECT_EQ(magnitude, 5);
}

TEST(Vector, CalculatesMinimum) {
	Vector a(1, 10);
	Vector b(20, 3);

	Vector c = VectorMin(a, b);

	EXPECT_EQ(c.X, 1);
	EXPECT_EQ(c.Y, 3);
}

TEST(Vector, CalculatesMaximum) {
	Vector a(1, 10);
	Vector b(20, 3);

	Vector c = VectorMax(a, b);

	EXPECT_EQ(c.X, 20);
	EXPECT_EQ(c.Y, 10);
}

TEST(Vector, CalculatesDot) {
	Vector a(1, 10);
	Vector b(20, 3);

	float dotProduct = VectorDot(a, b);

	EXPECT_EQ(dotProduct, 50);
}

TEST(Vector, CalculatesCross) {
	Vector a(1, 10);
	Vector b(20, 3);

	float dotProduct = VectorCross(a, b);

	EXPECT_EQ(dotProduct, -197);
}
