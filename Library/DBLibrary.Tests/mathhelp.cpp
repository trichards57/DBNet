#include "pch.h"
#include "../DBLibrary/mathhelp.h"

TEST(Angle, HandlesAnglesQuadrant1) {
	float angle = Angle(1, 1, 2, 2);

	EXPECT_NEAR(angle, M_PI_4, 0.001);
}

TEST(Angle, HandlesAnglesQuadrant2) {
	float angle = Angle(1, 1, 2, 0);

	EXPECT_NEAR(angle, -M_PI_4, 0.001);
}

TEST(Angle, HandlesAnglesQuadrant3) {
	float angle = Angle(1, 1, 0, 2);

	EXPECT_NEAR(angle, M_PI_2 + M_PI_4, 0.001);
}

TEST(Angle, HandlesAnglesQuadrant4) {
	float angle = Angle(1, 1, 0, 0);

	EXPECT_NEAR(angle, -(M_PI_2 + M_PI_4), 0.001);
}

TEST(AngleDifference, HandlesSmallAngles) {
	float angle = AngleDifference(0.1, 0.05);

	EXPECT_NEAR(angle, 0.05, 0.001);
}

TEST(AngleDifference, HandlesLargeAngles) {
	float angle = AngleDifference(6 * M_PI, M_PI_4);

	EXPECT_NEAR(angle, (2 * M_PI) - M_PI_4, 0.001);
}

TEST(AngleNormalise, HandlesUnchanged) {
	float angle = AngleNormalise(0.1);

	EXPECT_NEAR(angle, 0.1, 0.001);
}

TEST(AngleNormalise, HandlesLargePositive) {
	float testAngle = 0.14;
	float angle = AngleNormalise(testAngle + M_PI * 8);

	EXPECT_NEAR(angle, testAngle, 0.001);
}

TEST(AngleNormalise, HandlesLargeNegative) {
	float testAngle = 0.13;
	float angle = AngleNormalise(testAngle - M_PI * 8);

	EXPECT_NEAR(angle, testAngle, 0.001);
}

TEST(sgn, NegativeGivesMinusOne) {
	float x = -42.5;
	auto sign = sgn(x);

	EXPECT_EQ(sign, -1);
}

TEST(sgn, PositiveGivesPlusOne) {
	float x = 51.6;
	auto sign = sgn(x);

	EXPECT_EQ(sign, 1);
}

TEST(sgn, ZeroGivesPlusOne) {
	float x = 0;
	auto sign = sgn(x);

	EXPECT_EQ(sign, 0);
}

TEST(clamp, TooPositiveGetsCapped) {
	float x = clamp(50000, 32000, -32000);

	EXPECT_EQ(x, 32000);
}

TEST(clamp, TooNegativeGetsCapped) {
	float x = clamp(-50000, 32000, -32000);

	EXPECT_EQ(x, -32000);
}

TEST(clamp, GoodSizeUnchanged) {
	float x = clamp(42, 32000, -32000);

	EXPECT_EQ(x, 42);
}
