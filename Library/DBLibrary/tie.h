#pragma once

#include <oaidl.h>
#include <wtypes.h>

#pragma pack(4)
struct Robot;

#pragma pack(4)
struct Tie {
	short Port;
	short Pnt;
	short Ptt;
	float Ang;
	float Bend;
	VARIANT_BOOL AngReg;
	int Ln;
	int Shrink;
	VARIANT_BOOL Stat;
	short Last;
	short Mem;
	VARIANT_BOOL Back;
	VARIANT_BOOL NrgUsed;
	VARIANT_BOOL Infused;
	VARIANT_BOOL Sharing;
	float Fx;
	float Fy;
	float NaturalLength;
	float K;
	float B;
	signed char Type;
};

constexpr auto MAX_TIES = 10;

void Tie_HookeForces(Robot* robots, int idx);
void Tie_Torque(Robot* robots, int idx);