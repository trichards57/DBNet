#include "pch.h"
#include "tie.h"
#include "MemoryAddresses.h"
#include "robot.h"
#include "vector.h"

constexpr float AngleSlack = 5 * 2 * (float)M_PI / 360; // 5 degrees

void Tie_Torque(Robot* robots, int idx) {
	int j = 1;
	float mt = 0;
	int numTorqueTies = 0;
	Robot* rob1 = &robots[idx];

	if (rob1->NumTies > 0) {
		if (rob1->Ties[1].Pnt > 0) {
			float dlo;
			while (rob1->Ties[j].Pnt > 0) {
				if (rob1->Ties[j].AngReg == VARIANT_TRUE) { // Angle is fixed
					Robot* rob2 = &robots[rob1->Ties[j].Pnt];
					float an1 = Angle(rob1->Pos.X, rob1->Pos.Y, rob2->Pos.X, rob2->Pos.Y); // Angle of the tie
					dlo = AngleDifference(an1, rob1->Aim); // The difference between the tie angle and the robot aim
					float mm = AngleDifference(dlo, rob1->Ties[j].Ang + rob1->Ties[j].Bend); // Difference between actual and requested angle

					rob1->Ties[j].Bend = 0; // Reset the bend command

					if (fabs(mm) > AngleSlack) {
						numTorqueTies++;
						mm = (fabs(mm) - AngleSlack) * sgn(mm);
						float m = mm * 0.1f;
						float dx = rob2->Pos.X - rob1->Pos.X;
						float dy = rob1->Pos.Y - rob2->Pos.Y;
						float dist = sqrtf(dx * dx + dy * dy);
						float nax = clamp(-sin(an1) * m * dist / 10, 100, -100);
						float nay = clamp(-cos(an1) * m * dist / 10, 100, -100);

						Vector torqueVector = Vector(nax, nay);

						rob2->ImpulseInd -= torqueVector;
						rob1->ImpulseInd += torqueVector;
						mt += mm;
					}
				}
				j++;
			}
			if (mt != 0)
			{
				if (fabs(mt) > 2 * M_PI)
					rob1->Ties[j].Ang = dlo;
				else {
					if (fabs(mt) < M_PI_4)
						rob1->Ma = mt;
					else
						rob1->Ma = (float)M_PI_4 * sgn(mt);
				}
			}
		}
	}
}

void Tie_Delete(Robot* robots, int idx1, int idx2) {
	Robot* rob1 = &robots[idx1];
	Robot* rob2 = &robots[idx2];

	if (rob1->Exist == VARIANT_FALSE || rob2->Exist == VARIANT_FALSE)
		return;
	if (rob1->NumTies == 0 || rob2->NumTies == 0)
		return;

	int k = 1;
	int j = 1;

	while (rob1->Ties[k].Pnt != idx2 && k < MAX_TIES)
		k++;
	while (rob2->Ties[j].Pnt != idx1 && j < MAX_TIES)
		j++;

	if (k < MAX_TIES)
	{
		rob1->NumTies--;
		rob1->Mem[numties] = (short)rob1->NumTies;

		if (rob1->Mem[tiepres] == rob1->Ties[k].Port) // This is the last tie created, so tiepres needs updating
		{
			if (k > 1)
				rob1->Mem[tiepres] = rob1->Ties[k - 1].Port;
			else
				rob1->Mem[tiepres] = 0; // There are no more ties
		}
	}

	if (j < MAX_TIES)
	{
		rob2->NumTies--;
		rob2->Mem[numties] = (short)rob2->NumTies;

		if (rob2->Mem[tiepres] == rob2->Ties[j].Port) // This is the last tie created, so tiepres needs updating
		{
			if (j > 1)
				rob2->Mem[tiepres] = rob2->Ties[j - 1].Port;
			else
				rob2->Mem[tiepres] = 0; // There are no more ties
		}
	}

	for (int i = k; i < MAX_TIES; i++) {
		rob1->Ties[i] = rob1->Ties[i + 1];
	}
	for (int i = j; i < MAX_TIES; i++) {
		rob2->Ties[i] = rob2->Ties[i + 1];
	}
}

void Tie_Regang(Robot* robots, int idx, int tieIdx) {
	Robot* rob1 = &robots[idx];
	Robot* rob2 = &robots[rob1->Ties[tieIdx].Pnt];

	rob1->Multibot = VARIANT_TRUE;
	rob1->Mem[multi] = 1;
	rob1->Ties[tieIdx].B = 0.1f;
	rob1->Ties[tieIdx].K = 0.05f;
	rob1->Ties[tieIdx].Type = 3;

	float ang1 = Angle(rob1->Pos.X, rob1->Pos.Y, rob2->Pos.X, rob2->Pos.Y);
	float dist = sqrt(pow(rob1->Pos.X - rob2->Pos.X, 2) + pow(rob1->Pos.Y - rob2->Pos.Y, 2));

	if (rob1->Ties[tieIdx].Back == VARIANT_FALSE)
	{
		rob1->Ties[tieIdx].Ang = AngleDifference(AngleNormalise(ang1), rob2->Aim);
		rob1->Ties[tieIdx].AngReg = VARIANT_TRUE;
	}
	rob1->Ties[tieIdx].NaturalLength = dist;
}

void Tie_HookeForces(Robot* robots, int idx) {
	if (robots[idx].NumTies > 0) {
		// This is how much the tie can stretch or contract without creating a force
		float deformation = 20.0f;

		for (int k = 1; k <= MAX_TIES; k++) {
			if (robots[idx].Ties[k].Pnt == 0)
				break;

			// Add check to make sure the target robot actually exists (related to teleportation)

			Vector tieVector = robots[idx].Pos - robots[robots[idx].Ties[k].Pnt].Pos;
			auto length = tieVector.Magnitude();

			if (length - robots[idx].Radius - robots[robots[idx].Ties[k].Pnt].Radius > 1000) {
				Tie_Delete(robots, idx, robots[idx].Ties[k].Pnt);
			}
			else {
				if (robots[idx].Ties[k].Last > 1) robots[idx].Ties[k].Last--;
				if (robots[idx].Ties[k].Last < 0) robots[idx].Ties[k].Last++;

				if (robots[idx].Ties[k].Last == 1) {
					Tie_Delete(robots, idx, robots[idx].Ties[k].Pnt);
				}
				else
				{
					if (robots[idx].Ties[k].Last == -1)
						Tie_Regang(robots, idx, k);

					if (length != 0) {
						tieVector = tieVector / length;
						float displacement = robots[idx].Ties[k].NaturalLength - length;

						if (fabs(displacement) > deformation) {
							displacement = copysign(fabs(displacement) - deformation, displacement);
							float impulse = robots[idx].Ties[k].K * displacement;
							robots[idx].ImpulseInd = robots[idx].ImpulseInd + tieVector * impulse;

							Vector velocity = robots[idx].Vel - robots[robots[idx].Ties[k].Pnt].Vel;
							impulse = VectorDot(velocity, tieVector) * -robots[idx].Ties[k].B;
							robots[idx].ImpulseInd = robots[idx].ImpulseInd + tieVector * impulse;
						}
					}
				}
			}
		}
	}
}