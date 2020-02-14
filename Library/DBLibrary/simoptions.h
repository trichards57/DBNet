#pragma once

#include <wtypes.h>
#include "species.h"

constexpr auto MAX_NATIVE_SPECIES = 20;

#pragma pack(4)
struct SimulationOptions {
	BSTR SimName;
	int TotRunCycle;
	float CycSec;
	int TotRunTime;
	int TotBorn;
	short SpeciesNum;
	Species Species[MAX_NATIVE_SPECIES + 2];
	short FieldSize;
	int FieldWidth;
	int FieldHeight;
	short MaxPopulation;
	short MinVegs;
	VARIANT_BOOL KillDistVegs;
	VARIANT_BOOL BlockedVegs;
	VARIANT_BOOL DisableTies;
	VARIANT_BOOL DisableTypArepro;
	VARIANT_BOOL DisableFixing;
	short PopLimMethod;
	VARIANT_BOOL Toroidal;
	VARIANT_BOOL UpDnConnected;
	VARIANT_BOOL DxSxConnected;
	float MutCurrMult;
	VARIANT_BOOL MutOscill;
	VARIANT_BOOL MuOscillSine;
	int MutCycMax;
	int MutCycMin;
	VARIANT_BOOL DisableMutations;
	VARIANT_BOOL F1;
	VARIANT_BOOL League;
	VARIANT_BOOL Restart;
	int UserSeedNumber;
	int MaxEnergy;
	VARIANT_BOOL ZeroMomentum;
	float CostExecCond;
	float Costs[71];
	VARIANT_BOOL PondMode;
	short LightIntensity;
	float Gradient;
	VARIANT_BOOL DayNight;
	VARIANT_BOOL DayTime;
	short CycleLength;
	VARIANT_BOOL CorpseEnabled;
	float Decay;
	short DecayDelay;
	short DecayType;
	float EnergyProp;
	short EnergyFix;
	VARIANT_BOOL EnergyExType;
	float CoefficientStatic;
	float CoefficientKinetic;
	float Zgravity;
	float Ygravity;
	double Density;
	double Viscosity;
	float PhysBrown;
	float PhysMoving;
	float PhysSwim;
	VARIANT_BOOL PlanetEaters;
	float PlanetEatersG;
	short RepopCooldown;
	short RepopAmount;
	float Diffuse;
	float VegFeedingToBody;
	float MaxVelocity;
	short BadWasteLevel;
	short ChartingInterval;
	float CoefficientElasticity;
	short FluidSolidCustom;
	short CostRadioSetting;
	VARIANT_BOOL NoShotDecay;
	VARIANT_BOOL NoWShotDecay;
	VARIANT_BOOL SunUp;
	int SunUpThreshold;
	VARIANT_BOOL SunDown;
	int SunDownThreshold;
	VARIANT_BOOL DynamicCosts;
	VARIANT_BOOL FixedBotRadii;
	int DayNightCycleCounter;
	short SunThresholdMode;
	VARIANT_BOOL ShapesAreVisible;
	VARIANT_BOOL AllowVerticalShapeDrift;
	VARIANT_BOOL AllowHorizontalShapeDrift;
	VARIANT_BOOL ShapesAreSeeThrough;
	VARIANT_BOOL ShapesAbsorbShots;
	short ShapeDriftRate;
	VARIANT_BOOL MakeAllShapesTransparent;
	VARIANT_BOOL MakeAllShapesBlack;
	VARIANT_BOOL EGridEnabled;
	short EGridWidth;
	float OldCostX;
	int MaxAbsNum;
	int SimGUID;
	VARIANT_BOOL EnableAutoSpeciation;
	short SpeciationGeneticDistance;
	short SpeciationGenerationalDistance;
	short SpeciationMinimumPopulation;
	int SpeciationForkInterval;
	VARIANT_BOOL SunOnRnd;
	short Tides;
	short TidesOf;
	VARIANT_BOOL DeadRobotSnp;
	VARIANT_BOOL SnpExcludeVegs;
};