#include "pch.h"
#include "robot.h"

RobotStore robots;
unsigned long maxAbsNumber = 0;

int GetRobot() {
	for (size_t i = 0; i < robots.ArraySize; i++) {
		if (!robots.Exists[i])
		{
			return i;
		}
	}

	size_t newIndex = robots.ArraySize;

	robots.Resize(robots.ArraySize + 100);

	maxAbsNumber++;
	robots.AbsNumber[newIndex] = maxAbsNumber;

	return newIndex;
}

bool RobotExists(int id) {
	if (id < robots.ArraySize)
		return robots.Exists[id];
	return false;
}

BSTR GetFName(int id) {
	if (id < robots.ArraySize)
		return SysAllocStringByteLen(robots.FName[id].c_str(), robots.FName[id].size());
	return SysAllocString(L"");
}
