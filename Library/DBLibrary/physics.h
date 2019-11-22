#pragma once

#include "robot.h"
#include "simoptions.h"

void Physics_BorderCollisions(Robot& rob, const SimulationOptions options);
void Physics_NetForces(Robot& rob, const SimulationOptions options);