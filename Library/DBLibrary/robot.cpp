#include "pch.h"
#include "robot.h"
#include "costs.h"
#include "physics.h"

void Robot_ManageFixed(Robot& rob, const SimulationOptions options) {
	if (options.DisableFixing == VARIANT_FALSE)
		rob.Fixed = (rob.Mem[216] > 0);
}

void Robot_Poisons(Robot& rob) {
	if (rob.Corpse == VARIANT_TRUE || rob.DisableDNA == VARIANT_TRUE)
		return;

	if (rob.Paralyzed == VARIANT_TRUE) {
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

	if (rob.Poisoned == VARIANT_TRUE) {
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

void Robot_Upkeep(Robot& rob, const SimulationOptions options) {
	float cost;

	if (rob.Corpse == VARIANT_TRUE)
		return;

	// Age Cost
	float ageDelta = rob.Age - options.Costs[AGE_COST_START];

	if (ageDelta > 0 && rob.Age > 0) {
		if (options.Costs[AGE_COST_MAKE_LOG] == 1)
			cost = options.Costs[AGE_COST] * logf(ageDelta);
		else if (options.Costs[AGE_COST_MAKE_LINEAR] == 1)
			cost = options.Costs[AGE_COST] + (ageDelta * options.Costs[AGE_COST_LINEAR_FRACTION]);
		else
			cost = options.Costs[AGE_COST];
		rob.Nrg -= cost * options.Costs[COST_MULTIPLIER];
	}

	// Body Upkeep
	cost = rob.Body * options.Costs[BODY_UPKEEP] * options.Costs[COST_MULTIPLIER];
	rob.Nrg -= cost;

	// DNA Upkeep
	cost = (rob.DnaLen - 1) * options.Costs[DNA_CYC_COST] * options.Costs[COST_MULTIPLIER];
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

void Robot_RunPreUpdate(Robot* robots, int idx, SimulationOptions& options) {
	Robot_Upkeep(robots[idx], options);
	Robot_Poisons(robots[idx]);
	Robot_ManageFixed(robots[idx], options);
	Robot_CalculateMass(robots[idx]);

	// Obstacle Collisions should be done here.

	Physics_BorderCollisions(robots[idx], options);

	Tie_HookeForces(robots, idx);
	if (robots[idx].Corpse == VARIANT_FALSE && robots[idx].DisableDNA == VARIANT_FALSE)
		Tie_Torque(robots, idx);

	Physics_NetForces(robots[idx], options);
}

void __stdcall Robot_RunPreUpdates(LPSAFEARRAY& robs, short maxRobots, SimulationOptions& options) {
	Robot* robots;
	HRESULT res = SafeArrayAccessData(robs, reinterpret_cast<void**>(&robots));

	if (SUCCEEDED(res)) {
		for (int i = 0; i <= maxRobots; i++) {
			if (robots[i].Exist == VARIANT_TRUE) {
				Robot_RunPreUpdate(robots, i, options);
			}
		}
		SafeArrayUnaccessData(robs);
	}
}

// Returns false if the robot does not exist
// NOTE : This is opposite behaviour to the original function
bool Robot_CheckRobot(LPSAFEARRAY robs, int idx)
{
	long upperBound = 0;
	Robot rob = Robot();

	HRESULT res = SafeArrayGetUBound(robs, 1, &upperBound);

	if (SUCCEEDED(res)) {
		if (idx > upperBound)
			return false;

		if (idx == 0)
			return true;

		long i[] = { idx };

		res = SafeArrayGetElement(robs, i, &rob);

		if (SUCCEEDED(res)) {
			return rob.Exist == VARIANT_TRUE;
		}
	}

	return false;
}