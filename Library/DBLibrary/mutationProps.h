#pragma once

#include <oaidl.h>

struct MutationProbs {
	VARIANT_BOOL Mutations;
	float MutArray[21];
	float Mean[21];
	float StdDev[21];
	short PointWhatToChange;
	short CopyErrorWhatToChange;
};