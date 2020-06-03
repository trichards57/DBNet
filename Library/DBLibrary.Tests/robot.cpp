#include "pch.h"
#include "../DBLibrary/robot.h"
#include "../DBLibrary/MemoryAddresses.h"

TEST(Robot_ManageFixed, SetsFixedWhenAddressIsSet) {
	SimulationOptions* opts = new SimulationOptions();
	Robot* rob = new Robot();

	rob->Mem[Fixed] = 1;
	opts->DisableFixing = VARIANT_FALSE;

	Robot_ManageFixed(*rob, *opts);

	EXPECT_EQ(rob->Fixed, VARIANT_TRUE);

	delete opts, rob;
}

TEST(Robot_ManageFixed, SetsNotFixedWhenAddressIsZero) {
	SimulationOptions* opts = new SimulationOptions();
	Robot* rob = new Robot();

	rob->Mem[Fixed] = 0;
	opts->DisableFixing = VARIANT_FALSE;

	Robot_ManageFixed(*rob, *opts);

	EXPECT_EQ(rob->Fixed, VARIANT_FALSE);

	delete opts, rob;
}

TEST(Robot_ManageFixed, IgnoresAddressFixWhenDisableFixing) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	rob->Mem[Fixed] = 1;
	opts->DisableFixing = VARIANT_TRUE;

	Robot_ManageFixed(*rob, *opts);

	EXPECT_EQ(rob->Fixed, VARIANT_FALSE);
}

TEST(Robot_ManageFixed, IgnoresAddressFreeWhenDisableFixing) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	rob->Mem[Fixed] = 0;
	opts->DisableFixing = VARIANT_TRUE;

	Robot_ManageFixed(*rob, *opts);

	EXPECT_EQ(rob->Fixed, VARIANT_FALSE);
}

TEST(Robot_CalculateMass, SetsMassFromBody) {
	auto rob = std::make_unique<Robot>();

	rob->Body = 1258;

	Robot_CalculateMass(*rob);

	EXPECT_EQ(rob->Mass, rob->Body / 1000);
}

TEST(Robot_CalculateMass, SetsMassFromShell) {
	auto rob = std::make_unique<Robot>();

	rob->Shell = 456;

	Robot_CalculateMass(*rob);

	EXPECT_EQ(rob->Mass, rob->Shell / 200);
}

TEST(Robot_CalculateMass, SetsMassFromChloroplasts) {
	auto rob = std::make_unique<Robot>();

	rob->Chloroplasts = 32000;

	Robot_CalculateMass(*rob);

	EXPECT_EQ(rob->Mass, 31680);
}

TEST(Robot_CalculateMass, SetsMinimumMass) {
	auto rob = std::make_unique<Robot>();

	rob->Body = 25;

	Robot_CalculateMass(*rob);

	EXPECT_EQ(rob->Mass, 1);
}

TEST(Robot_CalculateMass, SetsMaximumMass) {
	auto rob = std::make_unique<Robot>();

	rob->Body = 32000 * 1042;

	Robot_CalculateMass(*rob);

	EXPECT_EQ(rob->Mass, 32000);
}

TEST(Robot_FindRadius, HandlesFixedSize) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_TRUE;

	float radius = Robot_FindRadius(*rob, *opts, 1);

	EXPECT_EQ(radius, ROBOT_SIZE_HALF);
}

TEST(Robot_FindRadius, HandlesBody) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_FALSE;
	rob->Body = 1000;

	float radius = Robot_FindRadius(*rob, *opts, 1);

	EXPECT_NEAR(radius, 114.2, 0.1);
}

TEST(Robot_FindRadius, HandlesMinimumRadius) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_FALSE;

	float radius = Robot_FindRadius(*rob, *opts, 1);

	EXPECT_EQ(radius, 1);
}

TEST(Robot_FindRadius, HandlesChloroplastsBody) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_FALSE;
	rob->Chloroplasts = 1000;

	float radius = Robot_FindRadius(*rob, *opts, 1);

	EXPECT_NEAR(radius, 12.9, 0.1);
}

TEST(Robot_FindRadius, HandlesTotalBody) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_FALSE;
	rob->Body = 1000;
	rob->Chloroplasts = 1000;

	float radius = Robot_FindRadius(*rob, *opts, 1);

	EXPECT_NEAR(radius, 114.2 + 9.4, 0.1);
}

TEST(Robot_FindRadius, HandlesDefaultCase) {
	auto opts = std::make_unique<SimulationOptions>();
	auto rob = std::make_unique<Robot>();

	opts->FixedBotRadii = VARIANT_FALSE;
	rob->Body = 32000;

	float radius = Robot_FindRadius(*rob, *opts, 1);
	float defaultRadius = Robot_FindRadius(*rob, *opts, -1);

	EXPECT_NEAR(defaultRadius, radius, 0.1);
}

TEST(Robot_Poisons, HandlesDeadRobot) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_TRUE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = 20;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = 20;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, 20);
	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, 20);
}

TEST(Robot_Poisons, HandlesDisabledDNA) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_TRUE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = 20;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = 20;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, 20);
	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, 20);
}

TEST(Robot_Poisons, HandlesOngoingParalysis) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = 20;
	rob->Poisoned = VARIANT_FALSE;
	rob->PoisonCount = 0;
	rob->Vloc = Fixed;
	rob->Vval = 10;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, 19);
	EXPECT_EQ(rob->Mem[837], 19);
	EXPECT_EQ(rob->Mem[Fixed], 10);
}

TEST(Robot_Poisons, HandlesOngoingPoison) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_FALSE;
	rob->Paracount = 0;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = 20;
	rob->Ploc = Fixed + 1;
	rob->Pval = 10;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, 19);
	EXPECT_EQ(rob->Mem[838], 19);
	EXPECT_EQ(rob->Mem[Fixed + 1], 10);
}

TEST(Robot_Poisons, HandlesEndingParalysis) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = 1;
	rob->Poisoned = VARIANT_FALSE;
	rob->PoisonCount = 0;
	rob->Vloc = Fixed;
	rob->Vval = 10;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_FALSE);
	EXPECT_EQ(rob->Paracount, 0);
	EXPECT_EQ(rob->Mem[837], 0);
	EXPECT_EQ(rob->Mem[Fixed], 10);
}

TEST(Robot_Poisons, HandlesEndingPoison) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_FALSE;
	rob->Paracount = 0;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = 1;
	rob->Ploc = Fixed + 1;
	rob->Pval = 10;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Poisoned, VARIANT_FALSE);
	EXPECT_EQ(rob->PoisonCount, 0);
	EXPECT_EQ(rob->Mem[838], 0);
	EXPECT_EQ(rob->Mem[Fixed + 1], 10);
}

TEST(Robot_Poisons, HandlesNoPoisonNoParalysis) {
	auto rob = std::make_unique<Robot>();

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_FALSE;
	rob->Paracount = 0;
	rob->Poisoned = VARIANT_FALSE;
	rob->PoisonCount = 0;
	rob->Vloc = Fixed + 1;
	rob->Vval = 10;
	rob->Ploc = Fixed + 1;
	rob->Pval = 11;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Poisoned, VARIANT_FALSE);
	EXPECT_EQ(rob->PoisonCount, 0);
	EXPECT_EQ(rob->Mem[838], 0);
	EXPECT_EQ(rob->Mem[Fixed], 0);
	EXPECT_EQ(rob->Mem[Fixed + 1], 0);
}

TEST(Robot_FeedBody, HandlesStandardCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 1000;
	rob->Body = 200;
	rob->Mem[fdbody] = 10;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 1010);
	EXPECT_EQ(rob->Body, 199);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_FeedBody, ClampsVeryHighCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 1000;
	rob->Body = 200;
	rob->Mem[fdbody] = 1000;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 1100);
	EXPECT_EQ(rob->Body, 190);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_FeedBody, ClampsVeryHighEnergy) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 32000;
	rob->Body = 200;
	rob->Mem[fdbody] = 10;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 32000);
	EXPECT_EQ(rob->Body, 199);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_StoreBody, HandlesStandardCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 1000;
	rob->Body = 200;
	rob->Mem[strbody] = 10;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 990);
	EXPECT_EQ(rob->Body, 201);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_StoreBody, ClampsVeryHighCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 1000;
	rob->Body = 200;
	rob->Mem[strbody] = 1000;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 900);
	EXPECT_EQ(rob->Body, 210);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_StoreBody, ClampsVeryHighBody) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	rob->Nrg = 32000;
	rob->Body = 32000;
	rob->Mem[strbody] = 10;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 31990);
	EXPECT_EQ(rob->Body, 32000);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}
