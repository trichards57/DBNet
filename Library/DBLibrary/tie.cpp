#include "pch.h"
#include "tie.h"
#include "MemoryAddresses.h"
#include "robot.h"
#include "vector.h"

void Tie_Delete(Robot* robots, int idx1, int idx2) {
	Robot* rob1 = &robots[idx1];
	Robot* rob2 = &robots[idx2];

	if (!rob1->Exist || !rob2->Exist)
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
		rob1->Mem[numties] = rob1->NumTies;

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
		rob2->Mem[numties] = rob2->NumTies;

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

	rob1->Multibot = true;
	rob1->Mem[multi] = 1;
	rob1->Ties[tieIdx].B = 0.1;
	rob1->Ties[tieIdx].K = 0.05;
	rob1->Ties[tieIdx].Type = 3;

	float ang1 = Angle(rob1->Pos.X, rob1->Pos.Y, rob2->Pos.X, rob2->Pos.Y);
	float dist = sqrt(pow(rob1->Pos.X - rob2->Pos.X, 2) + pow(rob1->Pos.Y - rob2->Pos.Y, 2));

	if (!rob1->Ties[tieIdx].Back)
	{
		rob1->Ties[tieIdx].Ang =
	}
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