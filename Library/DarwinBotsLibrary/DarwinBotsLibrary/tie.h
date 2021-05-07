#pragma once

#include "tie.h"
#include "vector.h"

struct Robot;

struct Tie
{
	int Port;
	Robot* Point;
	Tie* BackTie;
	float Angle;
	float Bend;
	bool IsAngleFixed;
	int Length;
	int Shrink;
	bool Stat;
	int Last;
	int Memory;
	bool IsBackTie;
	bool EnergyUsed;
	bool InformationUsed;
	bool Sharing;
	Vector Force;
	float NaturalLength;
	float K;
	float B;
	char Type;
};
