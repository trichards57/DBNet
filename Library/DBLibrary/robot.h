#pragma once

#include <oaidl.h>
#include <wtypes.h>
#include "vector.h"
#include "tie.h"
#include "mutationProps.h"
#include "var.h"
#include "simoptions.h"

#pragma warning(push)
#pragma warning(disable: 26495) // Disables the warning about not initialising fields.  Not a problem: the C++ currently doesn't ever create a Robot.
#pragma pack(4)
struct Robot {
	VARIANT_BOOL Exist;
	float Radius;
	VARIANT_BOOL Veg;
	VARIANT_BOOL NoChlr;
	VARIANT_BOOL Wall;
	VARIANT_BOOL Corpse;
	VARIANT_BOOL Fixed;
	VARIANT_BOOL View;
	VARIANT_BOOL NewMove;
	Vector Pos;
	Vector BucketPos;
	Vector Vel;
	Vector ActVel;
	Vector Opos;
	Vector ImpulseInd;
	Vector ImpulseRes;
	float ImpulseStatic;
	float AddedMass;
	float Aim;
	Vector AimVector;
	float OAim;
	float Ma;
	float Mt;
	Tie Ties[11];
	short Order;
	short Occurr[21];
	float Nrg;
	float Onrg;
	float Chloroplasts;
	float Body;
	float Obody;
	float Vbody;
	float Mass;
	float Shell;
	float Slime;
	float Waste;
	float Pwaste;
	float Poison;
	float Venom;
	VARIANT_BOOL Paralyzed;
	float Paracount;
	float NumTies;
	VARIANT_BOOL Multibot;
	VARIANT_BOOL TieAngOverwrite[4];
	VARIANT_BOOL TieLenOverwrite[4];
	VARIANT_BOOL Poisoned;
	float PoisonCount;
	float Bouyancy;
	short DecayTimer;
	int Kills;
	VARIANT_BOOL Dead;
	short Ploc;
	short Pval;
	short Vloc;
	short Vval;
	int Vtimer;
	Var Vars[1001];
	short Vnum;
	short MaxUsedVars;
	short UsedVars[1001];
	short EpiMem[15];
	short Mem[1001];
	LPSAFEARRAY Dna;
	int LastOpp;
	short LastOppType;
	Vector LastOppPos;
	int LastTch;
	int AbsNum;
	int Sim;
	MutationProbs Mutables;
	int PointMutCycle;
	int PointMutBp;
	int Point2MutCycle;
	short Condnum;
	void* Console;
	short SonNumber;
	int Mutations;
	int OldMutations;
	float GenMut;
	float OldGD;
	int LastMut;
	double MutEpiReset;
	int Parent;
	int Age;
	int NewAge;
	int BirthCycle;
	short GeneNum;
	short Generation;
	BSTR LastOwner;
	BSTR FName;
	short DnaLen;
	BSTR LastMutDetail;
	short Skin[14];
	short OSkin[14];
	int Color;
	VARIANT_BOOL Highlight;
	short Flash;
	short LastUp;
	short LastDown;
	short LastLeft;
	short LastRight;
	int VirsuShot;
	LPSAFEARRAY Ga;
	short OldBotNum;
	short ReproTimer;
	VARIANT_BOOL CantSee;
	VARIANT_BOOL DisableDNA;
	VARIANT_BOOL DisableMovementsSysvars;
	VARIANT_BOOL CantReproduce;
	VARIANT_BOOL VirusImmune;
	short SubSpecies;
	short Fertilized;
	LPSAFEARRAY SpermDNA;
	short SpermDNALen;
	BSTR Tag;
	short Monitor_R;
	short Monitor_G;
	short Monitor_B;
	unsigned char Multibot_Time;
	unsigned char Chlr_Share_Delay;
	unsigned char Dq;
	BSTR DbgString;
};
#pragma warning(pop)

void Robot_CalculateMass(Robot& rob);
bool Robot_CheckRobot(LPSAFEARRAY robots, int idx);
void Robot_ManageFixed(Robot& rob, const SimulationOptions options);
float __stdcall Robot_FindRadius(Robot& rob, SimulationOptions& options, float multiplier);
void Robot_Poisons(Robot& rob);
void Robot_FeedBody(Robot& rob, SimulationOptions& options);
void Robot_StoreBody(Robot& rob, SimulationOptions& options);
void Robot_StoreVenom(Robot& rob, SimulationOptions& options);

constexpr auto ROBOT_SIZE = 120;
constexpr auto ROBOT_SIZE_HALF = ROBOT_SIZE / 2;
constexpr auto ROBOT_CUBIC_TWIP_PER_BODY = 905;

constexpr auto ROBOT_MAX_MEM = 1000;
