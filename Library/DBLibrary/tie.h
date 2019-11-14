#pragma once

#define MAX_TIES

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