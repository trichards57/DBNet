#pragma once

#include "robot.h"

void Physics_BorderCollisions(Robot& rob, int fieldWidth, int fieldHeight);
void Physics_NetForces(Robot& rob, float physBrown, float maxVelocity, float physMoving, float moveCost);