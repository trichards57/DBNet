#pragma once

#include <wtypes.h>
#include "mutationProps.h"

#pragma pack(4)
struct Species {
	short Skin[14];
	BSTR Path;
	BSTR Name;
	short Stnrg;
	VARIANT_BOOL Veg;
	VARIANT_BOOL NoChlr;
	VARIANT_BOOL Fixed;
	int Color;
	short Colind;
	float Postp;
	float Poslf;
	float Posdn;
	float Posrg;
	short Qty;
	BSTR Comment;
	BSTR LeagueFileComment;
	MutationProbs Mutables;
	VARIANT_BOOL CantSee;
	VARIANT_BOOL DisableDNA;
	VARIANT_BOOL DisableMovementSysvars;
	VARIANT_BOOL CantReproduce;
	VARIANT_BOOL VirusImmune;
	short Population;
	short SubSpeciesCounter;
	VARIANT_BOOL Native;
	void* DisplayImage;
	VARIANT_BOOL Kill_MB;
	VARIANT_BOOL Dq_Kill;
};