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
