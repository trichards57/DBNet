#include "pch.h"
#include "robot.h"
#include "costs.h"
#include "physics.h"
#include "MemoryAddresses.h"

void Robot_ManageFixed(Robot& rob, const SimulationOptions options) {
	if (options.DisableFixing == VARIANT_FALSE)
		rob.Fixed = (rob.Mem[Fixed] > 0) ? VARIANT_TRUE : VARIANT_FALSE;
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

float __stdcall Robot_FindRadius(Robot& rob, SimulationOptions& options, float multiplier) {
	float bodyPoints;
	float chloroplasts;

	if (options.FixedBotRadii)
		return ROBOT_SIZE_HALF;

	if (multiplier == -1) {
		bodyPoints = 32000;
		chloroplasts = 0;
	}
	else {
		bodyPoints = rob.Body * multiplier;
		chloroplasts = rob.Chloroplasts * multiplier;
	}

	bodyPoints = max(1, bodyPoints);

	float result = powf(logf(bodyPoints) * bodyPoints * ROBOT_CUBIC_TWIP_PER_BODY * 3 * 0.25f / (float)M_PI, 1.0f / 3);
	result += (415 - result) * chloroplasts / 32000;

	return max(1, result);
}

void Robot_FeedBody(Robot& rob, SimulationOptions& options) {
	float feed = min(100.0f, rob.Mem[fdbody]);

	rob.Nrg += feed;
	rob.Body -= feed / 10;

	if (rob.Nrg > 32000)
		rob.Nrg = 32000;

	rob.Radius = Robot_FindRadius(rob, options, 1);

	rob.Mem[fdbody] = 0;
}

void Robot_StoreBody(Robot& rob, SimulationOptions& options) {
	float change = min(100.0f, rob.Mem[strbody]);

	rob.Nrg -= change;
	rob.Body += change / 10;

	if (rob.Body > 32000)
		rob.Body = 32000;

	rob.Radius = Robot_FindRadius(rob, options, 1);

	rob.Mem[strbody] = 0;
}

void __stdcall Robot_ManageBody(Robot& rob, SimulationOptions& options) {
	if (rob.Mem[strbody] > 0)
		Robot_StoreBody(rob, options);
	if (rob.Mem[fdbody] > 0)
		Robot_FeedBody(rob, options);

	rob.Body = clamp(rob.Body, 32000, 0);
	rob.Mem[body] = (int)rob.Body;
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

constexpr float VenomEnergyConversionRate = 1;

void __stdcall Robot_StoreVenom(Robot& rob, SimulationOptions& options) {
	if (rob.Nrg <= 0)
		return;

	float delta = (float)clamp(rob.Mem[824], 32000, -32000);

	if (delta > rob.Nrg / VenomEnergyConversionRate)
		delta = copysign(rob.Nrg / VenomEnergyConversionRate, delta);

	if (fabs(delta) > 100)
		delta = copysignf(100, delta);

	if (rob.Venom + delta > 32000)
		delta = 32000 - rob.Venom;
	if (rob.Venom + delta < 0)
		delta = -rob.Venom;

	rob.Venom += delta;
	rob.Nrg -= fabs(delta) * VenomEnergyConversionRate;

	float cost = fabs(delta) * options.Costs[VENOM_COST] * options.Costs[COST_MULTIPLIER];
	rob.Nrg -= cost;
	rob.Waste += cost;

	rob.Mem[824] = 0;
	rob.Mem[825] = (short)(rob.Venom);
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
