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
