#include "pch.h"
#include "robot.h"
#include "costs.h"
#include "physics.h"

void Robot_ManageFixed(Robot& rob, bool disableFixing) {
	if (!disableFixing)
		rob.Fixed = (rob.Mem[216] > 0);
}

void Robot_Poisons(Robot& rob) {
	if (rob.Corpse || rob.DisableDNA)
		return;

	if (rob.Paralyzed) {
		// The robot has received venom.
		rob.Mem[rob.Vloc] = rob.Vval;
		rob.Paracount--;
		if (rob.Paracount < 1)
		{
			rob.Paralyzed = false;
			rob.Vloc = 0;
			rob.Vval = 0;
		}
	}

	rob.Mem[837] = (short)rob.Paracount;

	if (rob.Poisoned) {
		// The robot has received poison.
		rob.Mem[rob.Ploc] = rob.Pval;
		rob.PoisonCount--;
		if (rob.PoisonCount < 1) {
			rob.Poisoned = false;
			rob.Ploc = 0;
			rob.Pval = 0;
		}
	}

	rob.Mem[838] = (short)rob.PoisonCount;
}

void Robot_Upkeep(Robot& rob, float* costs) {
	float cost;

	if (rob.Corpse)
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
}

void Robot_CalculateMass(Robot& rob) {
	rob.Mass = rob.Body / 1000 + rob.Shell / 200 + rob.Chloroplasts * 31680 / 32000;

	if (rob.Mass < 1)
		rob.Mass = 1;
	if (rob.Mass > 32000)
		rob.Mass = 32000;
}

extern "C" {
	__declspec(dllexport) void __stdcall Robot_RunPreUpdate(Robot& rob, LPSAFEARRAY* costsArray, VARIANT_BOOL* disableFixing,
		int fieldWidth, int fieldHeight) {
		float* costs;
		HRESULT arrayAccess = SafeArrayAccessData(*costsArray, (void**)&costs);

		if (FAILED(arrayAccess))
			return;

		Robot_Upkeep(rob, costs);
		Robot_Poisons(rob);
		Robot_ManageFixed(rob, (*disableFixing) == -1);
		Robot_CalculateMass(rob);

		// Obstacle Collisions should be done here.

		Physics_BorderCollisions(rob, fieldWidth, fieldHeight);

		// Sort out Tie forces here

		SafeArrayUnaccessData(*costsArray);
	}
}