#pragma once
struct MutationProbs
{
	bool MutationsEnabled;
	float MutationArray[20];
	float Mean[20];
	float StandardDeviation[20];
	int PointWhatToChange;
	int CopyErrorWhatToChange;
};
