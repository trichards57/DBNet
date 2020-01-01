#include "pch.h"
#include "physics.h"
#include "vector.h"
#include "MemoryAddresses.h"
#include "simoptions.h"
#include "costs.h"

float Physics_RandomFloat() {
	return((float)(rand())) / ((float)RAND_MAX);
}

void Physics_BrownianForces(Robot& rob, const SimulationOptions options) {
	if (options.PhysBrown == 0)
		return;

	float impulse = options.PhysBrown * 0.5f * Physics_RandomFloat();
	float angle = Physics_RandomFloat() * 2 * (float)M_PI;

	rob.ImpulseInd = rob.ImpulseInd + Vector(cosf(angle) * impulse, sinf(angle) * impulse);
	rob.Ma += (impulse / 100) * (Physics_RandomFloat() - 0.5);
}

void Physics_VoluntaryForces(Robot& rob, const SimulationOptions options) {
	float multiplier = 1;

	if (rob.Corpse || rob.DisableMovementsSysvars || rob.DisableDNA || !rob.Exist
		|| (rob.Mem[dirup] == 0 && rob.Mem[dirdn] == 0 && rob.Mem[dirsx] == 0 && rob.Mem[dirdx] == 0))
		return;

	if (!rob.NewMove)
		multiplier = rob.Mass;

	Vector dir = Vector(rob.Mem[dirup] - rob.Mem[dirdn], rob.Mem[dirsx] - rob.Mem[dirdx]) * multiplier;

	Vector newAccel = Vector(VectorDot(rob.AimVector, dir), VectorCross(rob.AimVector, dir));

	float magnitude = newAccel.Magnitude();

	if (magnitude > options.MaxVelocity)
		newAccel = newAccel * (options.MaxVelocity / magnitude);

	rob.ImpulseInd = rob.ImpulseInd + newAccel * options.PhysMoving;

	float energyCost = newAccel.Magnitude() * options.Costs[MOVE_COST] * options.Costs[COST_MULTIPLIER];

	if (energyCost > rob.Nrg)
		energyCost = rob.Nrg;

	if (energyCost < -1000)
		energyCost = -1000;

	rob.Nrg -= energyCost;
}

void Physics_NetForces(Robot& rob, const SimulationOptions options) {
	if (rob.Fixed)
		return;

	// Clip the velocity to zero to avoid overflows later
	if (rob.Vel.X < 0.0000001)
		rob.Vel.X = 0;
	if (rob.Vel.Y < 0.0000001)
		rob.Vel.Y = 0;

	// Deal with the planet eaters routine here (this is robot to robot gravity)
	// Deal with friction against the environment here
	// Deal with sphere drag forces here
	Physics_BrownianForces(rob, options);
	// Deal with gravity forces here
	Physics_VoluntaryForces(rob, options);
}

void Physics_BorderCollisions(Robot& rob, const SimulationOptions options) {
	const float b = 0.05f;
	const float SmudgeFactor = 50.0f;

	if (rob.Pos.X > rob.Radius&& rob.Pos.X < (options.FieldWidth - rob.Radius) && rob.Pos.Y > rob.Radius&& rob.Pos.Y < (options.FieldHeight - rob.Radius))
		return;

	rob.Mem[214] = 0;

	float smudge = rob.Radius + SmudgeFactor;

	Vector dif = VectorMin(VectorMax(rob.Pos, Vector(smudge, smudge)), Vector(options.FieldWidth - smudge, options.FieldHeight - smudge));
	Vector dist = dif - rob.Pos;

	if (dist.X != 0) {
		// TODO : This needs to handle left right being connected
		rob.Mem[214] = 1;

		if ((rob.Pos.X - rob.Radius) < 0)
			rob.Pos.X = rob.Radius;
		if ((rob.Pos.X + rob.Radius) > options.FieldWidth)
			rob.Pos.X = (float)options.FieldWidth - rob.Radius;
		rob.ImpulseRes.X += rob.Vel.X * b;
	}

	if (dist.Y != 0) {
		// TODO : This needs to handle up down being connected
		rob.Mem[214] = 1;

		if ((rob.Pos.Y - rob.Radius) < 0)
			rob.Pos.Y = rob.Radius;
		if ((rob.Pos.Y + rob.Radius) > options.FieldHeight)
			rob.Pos.Y = (float)options.FieldHeight - rob.Radius;
		rob.ImpulseRes.Y += rob.Vel.Y * b;
	}
}