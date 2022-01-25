#pragma once

#include "vector.h"
#include "tie.h"
#include "variable.h"
#include "block.h"
#include "MutationProbs.h"

struct RobotStore
{
	size_t ArraySize;
	bool* Exists;
	unsigned long* AbsNumber;
	std::string* FName;

	void Resize(int newSize) {
		ExpandArray(Exists, newSize);
		ExpandArray(AbsNumber, newSize);
		ExpandArray(FName, newSize);

		for (size_t i = ArraySize; i < newSize; i++)
			Exists[i] = false; // The new robot slots may contain invalid data, so make sure they are non-existent.

		ArraySize = newSize;
	}

	template <typename T> void ExpandArray(T*& inArray, size_t newSize) {
		T* newArray = new T[newSize];
		T* oldArray = inArray;
		inArray = newExists;
		std::copy(oldArray, oldArray + ArraySize, newArray);
		delete[] oldArray;
	}
};
