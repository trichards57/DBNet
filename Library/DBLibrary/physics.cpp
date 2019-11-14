#include "pch.h"
#include "physics.h"
#include "vector.h"

void Physics_BorderCollisions(Robot& rob, int fieldWidth, int fieldHeight) {
	const float b = 0.05f;
	const float SmudgeFactor = 50.0f;

	if (rob.Pos.X > rob.Radius&& rob.Pos.X < (fieldWidth - rob.Radius) && rob.Pos.Y > rob.Radius&& rob.Pos.Y < (fieldHeight - rob.Radius))
		return;

	rob.Mem[214] = 0;

	float smudge = rob.Radius + SmudgeFactor;

	Vector dif = VectorMin(VectorMax(rob.Pos, Vector(smudge, smudge)), Vector(fieldWidth - smudge, fieldHeight - smudge));
	Vector dist = dif - rob.Pos;

	if (dist.X != 0) {
		// TODO : This needs to handle left right being connected
		rob.Mem[214] = 1;

		if ((rob.Pos.X - rob.Radius) < 0)
			rob.Pos.X = rob.Radius;
		if ((rob.Pos.X + rob.Radius) > fieldWidth)
			rob.Pos.X = (float)fieldWidth - rob.Radius;
		rob.ImpulseRes.X += rob.Vel.X * b;
	}

	if (dist.Y != 0) {
		// TODO : This needs to handle up down being connected
		rob.Mem[214] = 1;

		if ((rob.Pos.Y - rob.Radius) < 0)
			rob.Pos.Y = rob.Radius;
		if ((rob.Pos.Y + rob.Radius) > fieldHeight)
			rob.Pos.Y = (float)fieldHeight - rob.Radius;
		rob.ImpulseRes.Y += rob.Vel.Y * b;
	}
}