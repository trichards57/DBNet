#include "pch.h"
#include "robot.h"
#include "costs.h"

extern "C" {
	__declspec(dllexport) void __stdcall TestRobot(Robot& rob) {
	}

	__declspec(dllexport) void __stdcall Robot_Upkeep(Robot& rob, LPSAFEARRAY* costsArray) {
		float* costs;
		float cost;

		if (rob.Corpse)
			return;

		HRESULT arrayAccess = SafeArrayAccessData(*costsArray, (void**)&costs);

		if (FAILED(arrayAccess))
			return;

		// Age Cost
		float ageDelta = rob.Age - costs[AGE_COST_START];

		if (ageDelta > 0 && rob.Age > 0) {
			if (costs[AGE_COST_MAKE_LOG] == 1)
				cost = costs[AGE_COST] * logf(ageDelta);
			else if (costs[AGE_COST_MAKE_LINEAR] == 1)
				cost = costs[AGE_COST] + (ageDelta * costs[AGE_COST_LINEAR_FRACTION]);
			else
				cost = costs[AGE_COST];
			rob.Nrg -= cost * costs[COST_MULTIPLIER];
		}

		// Body Upkeep
		cost = rob.Body * costs[BODY_UPKEEP] * costs[COST_MULTIPLIER];
		rob.Nrg -= cost;

		// DNA Upkeep
		cost = (rob.DnaLen - 1) * costs[DNA_CYC_COST] * costs[COST_MULTIPLIER];
		rob.Nrg -= cost;

		// Degrade Slime
		rob.Slime *= 0.98f;
		if (rob.Slime < 0.5)
			rob.Slime = 0;
		rob.Mem[821] = (short)rob.Slime;

		rob.Poison *= 0.98f;
		if (rob.Poison < 0.5)
			rob.Poison = 0;
		rob.Mem[827] = (short)rob.Poison;

		SafeArrayUnaccessData(*costsArray);
	}
}