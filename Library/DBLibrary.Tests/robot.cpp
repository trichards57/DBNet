#include "pch.h"
#include "../DBLibrary/robot.h"
#include "../DBLibrary/MemoryAddresses.h"
#include "../DBLibrary/costs.h"

constexpr auto MAX_VALUE = 32000;

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

	EXPECT_EQ(rob->Mass, MAX_VALUE);
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
	const float initialCount = 20;

	rob->Corpse = VARIANT_TRUE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = initialCount;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = initialCount;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, initialCount);
	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, initialCount);
}

TEST(Robot_Poisons, HandlesDisabledDNA) {
	auto rob = std::make_unique<Robot>();
	const float initialCount = 20;

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_TRUE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = initialCount;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = initialCount;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, initialCount);
	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, initialCount);
}

TEST(Robot_Poisons, HandlesOngoingParalysis) {
	auto rob = std::make_unique<Robot>();
	const float initialCount = 20;
	const int memAddress = Fixed;
	const int memValue = 10;

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = initialCount;
	rob->Poisoned = VARIANT_FALSE;
	rob->PoisonCount = 0;
	rob->Vloc = memAddress;
	rob->Vval = memValue;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_TRUE);
	EXPECT_EQ(rob->Paracount, initialCount - 1);
	EXPECT_EQ(rob->Mem[837], initialCount - 1);
	EXPECT_EQ(rob->Mem[memAddress], memValue);
}

TEST(Robot_Poisons, HandlesOngoingPoison) {
	auto rob = std::make_unique<Robot>();
	const float initialCount = 20;
	const int memAddress = Fixed + 1;
	const int memValue = 10;

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_FALSE;
	rob->Paracount = 0;
	rob->Poisoned = VARIANT_TRUE;
	rob->PoisonCount = initialCount;
	rob->Ploc = memAddress;
	rob->Pval = memValue;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Poisoned, VARIANT_TRUE);
	EXPECT_EQ(rob->PoisonCount, initialCount - 1);
	EXPECT_EQ(rob->Mem[838], initialCount - 1);
	EXPECT_EQ(rob->Mem[memAddress], memValue);
}

TEST(Robot_Poisons, HandlesEndingParalysis) {
	auto rob = std::make_unique<Robot>();
	const float initialCount = 1;
	const int memAddress = Fixed + 1;
	const int memValue = 10;

	rob->Corpse = VARIANT_FALSE;
	rob->DisableDNA = VARIANT_FALSE;

	rob->Paralyzed = VARIANT_TRUE;
	rob->Paracount = initialCount;
	rob->Poisoned = VARIANT_FALSE;
	rob->PoisonCount = 0;
	rob->Vloc = memAddress;
	rob->Vval = memValue;

	Robot_Poisons(*rob);

	EXPECT_EQ(rob->Paralyzed, VARIANT_FALSE);
	EXPECT_EQ(rob->Paracount, initialCount - 1);
	EXPECT_EQ(rob->Mem[837], initialCount - 1);
	EXPECT_EQ(rob->Mem[memAddress], memValue);
}

TEST(Robot_Poisons, HandlesEndingPoison) {
	auto rob = std::make_unique<Robot>();
	const float initialCount = 1;
	const int memAddress = Fixed + 1;
	const int memValue = 10;

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
	EXPECT_EQ(rob->PoisonCount, initialCount - 1);
	EXPECT_EQ(rob->Mem[838], initialCount - 1);
	EXPECT_EQ(rob->Mem[memAddress], memValue);
}

TEST(Robot_Poisons, HandlesNoPoisonNoParalysis) {
	auto rob = std::make_unique<Robot>();

	const int poisonAddress = Fixed;
	const int paralysisAddress = Fixed + 1;
	const int memValue = 10;

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
	EXPECT_EQ(rob->Paralyzed, VARIANT_FALSE);
	EXPECT_EQ(rob->PoisonCount, 0);
	EXPECT_EQ(rob->Paracount, 0);
	EXPECT_EQ(rob->Mem[837], 0);
	EXPECT_EQ(rob->Mem[838], 0);
	EXPECT_NE(rob->Mem[poisonAddress], memValue);
	EXPECT_NE(rob->Mem[paralysisAddress], memValue);
}

TEST(Robot_FeedBody, HandlesStandardCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = 1000;
	const int startBody = 200;
	const int feedBodyAmount = 10;

	rob->Nrg = startNrg;
	rob->Body = startBody;
	rob->Mem[fdbody] = feedBodyAmount;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, startNrg + feedBodyAmount);
	EXPECT_EQ(rob->Body, startBody - feedBodyAmount / 10);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_FeedBody, ClampsVeryHighCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = 1000;
	const int startBody = 200;
	const int feedBodyAmount = 1000;
	const int maxAllowedFeed = 100;

	rob->Nrg = startNrg;
	rob->Body = startBody;
	rob->Mem[fdbody] = feedBodyAmount;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, startNrg + maxAllowedFeed);
	EXPECT_EQ(rob->Body, startBody - maxAllowedFeed / 10);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_FeedBody, ClampsVeryHighEnergy) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = MAX_VALUE;
	const int startBody = 200;
	const int feedBodyAmount = 10;

	rob->Nrg = startNrg;
	rob->Body = startBody;
	rob->Mem[fdbody] = feedBodyAmount;

	Robot_FeedBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, MAX_VALUE);
	EXPECT_EQ(rob->Body, startBody - feedBodyAmount / 10);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[fdbody], 0);
}

TEST(Robot_StoreBody, HandlesStandardCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = 1000;
	const int startBody = 200;
	const int storeBodyAmount = 10;

	rob->Nrg = startNrg;
	rob->Body = startBody;
	rob->Mem[strbody] = storeBodyAmount;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, startNrg - storeBodyAmount);
	EXPECT_EQ(rob->Body, startBody + storeBodyAmount / 10);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[strbody], 0);
}

TEST(Robot_StoreBody, ClampsVeryHighCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = 1000;
	const int startBody = 200;
	const int storeBodyAmount = 1000;
	const int maxAllowedStore = 100;

	rob->Nrg = startNrg;
	rob->Body = startBody;
	rob->Mem[strbody] = storeBodyAmount;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, startNrg - maxAllowedStore);
	EXPECT_EQ(rob->Body, startBody + maxAllowedStore / 10);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[strbody], 0);
}

TEST(Robot_StoreBody, ClampsVeryHighBody) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	const int startNrg = MAX_VALUE;
	const int startBody = MAX_VALUE;
	const int storeBodyAmount = 10;

	rob->Nrg = 32000;
	rob->Body = 32000;
	rob->Mem[strbody] = 10;

	Robot_StoreBody(*rob, *opts);

	EXPECT_EQ(rob->Nrg, startNrg - storeBodyAmount);
	EXPECT_EQ(rob->Body, MAX_VALUE);
	EXPECT_EQ(rob->Radius, Robot_FindRadius(*rob, *opts, 1));
	EXPECT_EQ(rob->Mem[strbody], 0);
}

TEST(Robot_StoreVenom, HandlesStandardCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	opts->Costs[VENOM_COST] = 2.0;
	opts->Costs[COST_MULTIPLIER] = 3.0;

	rob->Nrg = 1000;
	rob->Venom = 10;
	rob->Mem[storevenom] = 20;
	rob->Waste = 900;

	Robot_StoreVenom(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 860);
	EXPECT_EQ(rob->Venom, 30);
	EXPECT_EQ(rob->Waste, 1020);
	EXPECT_EQ(rob->Mem[storevenom], 0);
	EXPECT_EQ(rob->Mem[venom], 30);
}

TEST(Robot_StoreVenom, ClampsVeryHighCommand) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	opts->Costs[VENOM_COST] = 1.0;
	opts->Costs[COST_MULTIPLIER] = 1.0;

	rob->Nrg = 32000;
	rob->Venom = 0;
	rob->Mem[storevenom] = 32767;

	Robot_StoreVenom(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 31800);
	EXPECT_EQ(rob->Venom, 100);
	EXPECT_EQ(rob->Waste, 100);
	EXPECT_EQ(rob->Mem[storevenom], 0);
	EXPECT_EQ(rob->Mem[venom], 100);
}

TEST(Robot_StoreVenom, ClampsVeryHighVenom) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	opts->Costs[VENOM_COST] = 1.0;
	opts->Costs[COST_MULTIPLIER] = 1.0;

	rob->Nrg = 32000;
	rob->Venom = 31990;
	rob->Mem[storevenom] = 100;

	Robot_StoreVenom(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 31980);
	EXPECT_EQ(rob->Venom, 32000);
	EXPECT_EQ(rob->Waste, 10);
	EXPECT_EQ(rob->Mem[storevenom], 0);
	EXPECT_EQ(rob->Mem[venom], 32000);
}

TEST(Robot_StoreVenom, ClampsLowEnergy) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	opts->Costs[VENOM_COST] = 1.0;
	opts->Costs[COST_MULTIPLIER] = 1.0;

	rob->Nrg = 10;
	rob->Venom = 0;
	rob->Mem[storevenom] = 100;

	Robot_StoreVenom(*rob, *opts);

	EXPECT_EQ(rob->Nrg, -10);
	EXPECT_EQ(rob->Venom, 10);
	EXPECT_EQ(rob->Waste, 10);
	EXPECT_EQ(rob->Mem[storevenom], 0);
	EXPECT_EQ(rob->Mem[venom], 10);
}

TEST(Robot_StoreVenom, ClampsLowVenom) {
	auto rob = std::make_unique<Robot>();
	auto opts = std::make_unique <SimulationOptions>();

	opts->Costs[VENOM_COST] = 1.0;
	opts->Costs[COST_MULTIPLIER] = 1.0;

	rob->Nrg = 1000;
	rob->Venom = 10;
	rob->Mem[storevenom] = -100;

	Robot_StoreVenom(*rob, *opts);

	EXPECT_EQ(rob->Nrg, 980);
	EXPECT_EQ(rob->Venom, 0);
	EXPECT_EQ(rob->Waste, 10);
	EXPECT_EQ(rob->Mem[storevenom], 0);
	EXPECT_EQ(rob->Mem[venom], 0);
}
