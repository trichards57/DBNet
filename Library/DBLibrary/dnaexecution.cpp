#include "pch.h"

#include "robot.h"
#include <stack>
#include "block.h"
#include "costs.h"
#include "MemoryAddresses.h"
#include <random>

constexpr auto MaxValue = 2000000000;

std::mt19937 rng((UINT)time(nullptr));

enum class CurrentConditionFlag {
	NextBody = 1,
	NextElse = 2
};

enum class CurrentFlow {
	Clear = 0,
	Condition = 1,
	Body = 2,
	ElseBody = 3
};

struct State {
	CurrentConditionFlag CurrentCondFlag = CurrentConditionFlag::NextBody;
	CurrentFlow CurrentFlow = CurrentFlow::Clear;
	bool InGene = false;
	int CurrentGene = 0;
	std::stack<int> IntStack = std::stack<int>();
	std::stack<bool> BoolStack = std::stack<bool>();

	int PopInt() {
		if (IntStack.size() == 0)
			return 0;
		int val = IntStack.top();
		IntStack.pop();
		return val;
	}

	bool PopBool() {
		if (BoolStack.size() == 0)
			return true;
		bool val = BoolStack.top();
		BoolStack.pop();
		return val;
	}
};

int LocalMod(int value, int max) {
	int newValue = value % max;

	if (newValue == 0) {
		if (value > 0) {
			return max;
		}
		else if (value < 0) {
			return -max;
		}
	}
	return newValue;
}

void DNA_ExecuteBasicCommand(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	int a, b, c;
	std::uniform_int_distribution<int> gen;

	rob.Nrg -= simOpts.Costs[COST_BASIC_COMMAND] * simOpts.Costs[COST_MULTIPLIER];

	switch (value) {
	case 1: // add
		a = state.PopInt();
		b = state.PopInt();

		a %= MaxValue;
		b %= MaxValue;

		c = a + b;

		if (abs(c) > MaxValue)
			c -= sgn(c) * MaxValue;
		state.IntStack.push(c);
		break;
	case 2: // add
		a = state.PopInt();
		b = state.PopInt();

		a %= MaxValue;
		b %= MaxValue;

		c = a - b;

		if (abs(c) > MaxValue)
			c -= sgn(c) * MaxValue;
		state.IntStack.push(c);
		break;
	case 3: // mult
		a = state.PopInt();
		b = state.PopInt();
		c = a * b;
		if (abs(c) > MaxValue)
			c = (int)copysign(MaxValue, c);
		state.IntStack.push(c);
		break;
	case 4: // div
		a = state.PopInt();
		b = state.PopInt();
		if (b != 0)
			state.IntStack.push(a / b);
		else
			state.IntStack.push(0);
		break;
	case 5: // rnd
		gen = std::uniform_int_distribution<int>(0, state.PopInt());
		state.IntStack.push(gen(rng));
		break;
	case 6: // dereference (*)
		b = state.PopInt();
		b = LocalMod(abs(b), ROBOT_MAX_MEM);
		state.IntStack.push(rob.Mem[b]);
		break;
	case 7: // mod
		b = state.PopInt();
		if (b == 0)
		{
			state.PopInt();
			state.IntStack.push(0);
		}
		else
			state.IntStack.push(state.PopInt() % b);
		break;
	case 8: // sgn
		state.IntStack.push(sgn(state.PopInt()));
		break;
	case 9: // absolute value
		state.IntStack.push(abs(state.PopInt()));
		break;
	case 10: // dup or dupint
		b = state.PopInt();
		state.IntStack.push(b);
		state.IntStack.push(b);
		break;
	case 11: // dropint
		state.PopInt();
		break;
	case 12: // clearint
		state.IntStack = std::stack<int>();
		break;
	case 13: // swapint
		if (state.IntStack.size() > 1) {
			a = state.PopInt();
			b = state.PopInt();
			state.IntStack.push(a);
			state.IntStack.push(b);
		}
		break;
	case 14: // overint
		if (state.IntStack.size() == 1) {
			state.IntStack.push(0);
		}
		else if (state.IntStack.size() > 1) {
			b = state.PopInt();
			a = state.PopInt();
			state.IntStack.push(a);
			state.IntStack.push(b);
			state.IntStack.push(a);
		}
		break;
	}
}

float getScale(int dimension) {
	if (dimension > 32000)
		return (float)dimension / 32000;
	return 1;
}

void DNA_FindAngle(Robot& rob, SimulationOptions& simOpts, State& state) {
	int x1, y1;

	y1 = state.PopInt();
	x1 = state.PopInt();

	float x2 = rob.Pos.X / getScale(simOpts.FieldWidth);
	float y2 = rob.Pos.Y / getScale(simOpts.FieldHeight);

	int e = angleToInt(AngleNormalise(Angle(x2, y2, (float)x1, (float)y1)));

	state.IntStack.push(e);
}

void DNA_FindDistance(Robot& rob, SimulationOptions& simOpts, State& state) {
	Vector v1((float)state.PopInt() / getScale(simOpts.FieldHeight), (float)state.PopInt() / getScale(simOpts.FieldWidth));

	Vector vectorDistance = v1 - rob.Pos;
	float length = min(vectorDistance.Magnitude(), MaxValue);

	state.IntStack.push((int)length);
}

void DNA_ExecuteAdvancedCommand(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	rob.Nrg -= simOpts.Costs[COST_ADVANCED_COMMAND] * simOpts.Costs[COST_MULTIPLIER];

	int a, b;
	float c;

	switch (value) {
	case 1: // findang
		DNA_FindAngle(rob, simOpts, state);
		break;
	case 2: // finddist
		DNA_FindDistance(rob, simOpts, state);
		break;
	case 3: // ceil
		b = state.PopInt();
		a = state.PopInt();
		state.IntStack.push(min(a, b));
		break;
	case 4: // floor
		b = state.PopInt();
		a = state.PopInt();
		state.IntStack.push(max(a, b));
		break;
	case 5: // sqr
		a = state.PopInt();
		if (a > 0)
			state.IntStack.push((int)sqrt(a));
		else
			state.IntStack.push(0);
		break;
	case 6: // power
		b = state.PopInt();
		a = state.PopInt();

		if (abs(b) > 10)
			b = (int)copysign(10, b);

		if (a == 0)
			state.IntStack.push(0);
		else {
			c = pow((float)a, (float)b);
			if (c > MaxValue)
				c = copysignf(MaxValue, c);
			state.IntStack.push((int)c);
		}
		break;
	case 7: // pyth
		a = state.PopInt();
		b = state.PopInt();
		c = Vector((float)a, (float)b).Magnitude();
		if (c > MaxValue)
			c = copysignf(MaxValue, c);
		state.IntStack.push((int)c);
		break;
	case 8: // angle compare
		b = state.PopInt();
		a = state.PopInt();

		b %= 1256;
		if (b < 0) b += 1256;

		a %= 1256;
		if (a < 0) a += 1256;

		state.IntStack.push(angleToInt((AngleDifference(intToAngle(a), intToAngle(b)))));
		break;
	case 9: // root
		b = abs(state.PopInt());
		a = abs(state.PopInt());

		if (b == 0)
			state.IntStack.push(0);
		else
			state.IntStack.push((int)pow(a, 1 / b));
		break;
	case 10: // logx
		b = abs(state.PopInt());
		a = abs(state.PopInt());

		if (b < 2 || a == 0)
			state.IntStack.push(0);
		else
			state.IntStack.push((int)(log(a) / log(b)));
		break;
	case 11: // sin
		a = state.PopInt();
		state.IntStack.push((int)(sin(intToAngle(a)) * 32000));
		break;
	case 12: // cos
		a = state.PopInt();
		state.IntStack.push((int)(cos(intToAngle(a)) * 32000));
		break;
	}
}

void DNA_ExecuteBitwiseCommand(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	rob.Nrg -= simOpts.Costs[COST_BITWISE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];

	int a, b;

	switch (value) {
	case 1: // Compliment
		a = state.PopInt();
		a = ~a;
		state.IntStack.push(a);
		break;
	case 2: // AND
		b = state.PopInt();
		a = state.PopInt();
		state.IntStack.push(a & b);
		break;
	case 3: // OR
		b = state.PopInt();
		a = state.PopInt();
		state.IntStack.push(a | b);
		break;
	case 4: // XOR
		b = state.PopInt();
		a = state.PopInt();
		state.IntStack.push(a ^ b);
		break;
	case 5: // ++
		a = state.PopInt();
		a++;
		state.IntStack.push(a);
		break;
	case 6: // --
		a = state.PopInt();
		a--;
		state.IntStack.push(a);
		break;
	case 7: // negate
		a = state.PopInt();
		a = -a;
		state.IntStack.push(a);
		break;
	case 8: // <<
		a = state.PopInt();
		a = a << 1;
		state.IntStack.push(a);
		break;
	case 9: // >>
		a = state.PopInt();
		a = a >> 1;
		state.IntStack.push(a);
		break;
	}
}

void DNA_ExecuteConditions(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	rob.Nrg -= simOpts.Costs[COST_CONDITION_COMMAND] * simOpts.Costs[COST_MULTIPLIER];

	int a, b, d;
	float tolerance;

	switch (value)
	{
	case 1: // <
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a < b);
		break;
	case 2: // >
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a > b);
		break;
	case 3: // ==
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a == b);
		break;
	case 4: // !=
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a != b);
		break;
	case 5: // percent equal
		b = state.PopInt();
		a = state.PopInt();
		tolerance = (float)a / 10;
		state.BoolStack.push((a - tolerance <= b) && (a + tolerance >= b));
		break;
	case 6: // not percent equal
		b = state.PopInt();
		a = state.PopInt();
		tolerance = (float)a / 10;
		state.BoolStack.push(!((a - tolerance <= b) && (a + tolerance >= b)));
		break;
	case 7: // roughly equal
		d = state.PopInt();
		b = state.PopInt();
		a = state.PopInt();
		tolerance = (float)a / 100 * d;
		state.BoolStack.push((a - tolerance <= b) && (a + tolerance >= b));
	case 8: // not roughly equal
		d = state.PopInt();
		b = state.PopInt();
		a = state.PopInt();
		tolerance = (float)a / 100 * d;
		state.BoolStack.push(!((a - tolerance <= b) && (a + tolerance >= b)));
	case 9: // >=
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a >= b);
		break;
	case 10: // <=
		b = state.PopInt();
		a = state.PopInt();

		state.BoolStack.push(a <= b);
		break;
	}
}

void DNA_ExecuteLogic(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	rob.Nrg -= simOpts.Costs[COST_LOGIC_COMMAND] * simOpts.Costs[COST_MULTIPLIER];

	bool a, b;

	switch (value) {
	case 1: // and
		b = state.PopBool();
		a = state.PopBool();
		state.BoolStack.push(a && b);
		break;
	case 2: // or
		b = state.PopBool();
		a = state.PopBool();
		state.BoolStack.push(a || b);
		break;
	case 3: // xor
		b = state.PopBool();
		a = state.PopBool();
		state.BoolStack.push(a != b);
		break;
	case 4: // not
		b = state.PopBool();
		state.BoolStack.push(!b);
		break;
	case 5: // true
		state.BoolStack.push(true);
		break;
	case 6: // false
		state.BoolStack.push(false);
		break;
	case 7: // dropbool
		state.PopBool();
		break;
	case 8: // clearbool
		state.BoolStack = std::stack<bool>();
		break;
	case 9: // dupbool
		if (!state.BoolStack.empty())
			state.BoolStack.push(state.BoolStack.top());
		break;
	case 10: // swapbool
		if (state.BoolStack.size() > 1) {
			a = state.PopBool();
			b = state.PopBool();

			state.BoolStack.push(a);
			state.BoolStack.push(b);
		}
		break;
	case 11: // overbool
		if (state.BoolStack.size() > 1) {
			a = state.PopBool();
			b = state.PopBool();

			state.BoolStack.push(a);
			state.BoolStack.push(b);
			state.BoolStack.push(a);
		}
		else if (state.BoolStack.size() == 1) {
			state.BoolStack.push(true);
		}
		break;
	}
}

void DNA_ExecuteStores(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	int a, b, c;

	switch (value) {
	case 1: // store
		b = state.PopInt(); // The memory location to store to
		if (b != 0) { // Only stores to non-zero addresses actually do anything
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			a = state.PopInt();

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = LocalMod(a, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 2: // inc
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			rob.Mem[b] = LocalMod(rob.Mem[b] + 1, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 3: // dec
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			rob.Mem[b] = LocalMod(rob.Mem[b] - 1, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 4: // add and store
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			a = rob.Mem[b] + state.PopInt();

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = LocalMod(a, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 5: // subtract and store
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			a = rob.Mem[b] - state.PopInt();

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = LocalMod(a, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 6: // multiply and store
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			a = rob.Mem[b] * LocalMod(state.PopInt(), 32000);

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = LocalMod(a, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 7: // divide and store
		b = state.PopInt();
		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			int c = state.PopInt();
			if (c == 0)
				a = 0;
			else
				a = rob.Mem[b] / c;

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = LocalMod(a, 32000);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 8: // ceiling and store
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			c = state.PopInt();

			a = LocalMod(min(rob.Mem[b], c), 32000);

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = a;
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 9: // floor and store
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);
			c = state.PopInt();

			a = LocalMod(max(rob.Mem[b], c), 32000);

			for (int k = 0; k < 4; k++) {
				if (b == 480 + k) rob.TieAngOverwrite[k] = VARIANT_TRUE;
				if (b == 484 + k) rob.TieLenOverwrite[k] = VARIANT_TRUE;
			}

			rob.Mem[b] = a;
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 10: // store random
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);

			std::uniform_int_distribution<int> gen(min(0, rob.Mem[b]), max(0, rob.Mem[b]));
			rob.Mem[b] = gen(rng);
			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
		break;
	case 11: // store sign
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);

			rob.Mem[b] = sgn(rob.Mem[b]);

			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
	case 12: // store abs
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);

			rob.Mem[b] = abs(rob.Mem[b]);

			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
	case 13: // store square root
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);

			if (rob.Mem[b] > 0)
				rob.Mem[b] = (short)sqrt(rob.Mem[b]);
			else
				rob.Mem[b] = 0;

			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
	case 14: // store negation
		b = state.PopInt();

		if (b != 0) {
			b = LocalMod(abs(b), ROBOT_MAX_MEM);

			rob.Mem[b] = -rob.Mem[b];

			rob.Nrg -= simOpts.Costs[COST_STORE_COMMAND] * simOpts.Costs[COST_MULTIPLIER];
		}
	}
}

CurrentConditionFlag AddUpConditions(State& state) {
	bool res = true;

	while (!state.BoolStack.empty()) {
		res = res && state.PopBool();
	}

	return res ? CurrentConditionFlag::NextBody : CurrentConditionFlag::NextElse;
}

bool DNA_ExecuteFlowCommand(Robot& rob, SimulationOptions& simOpts, State& state, short value) {
	if (value == 1) {
		state.CurrentFlow = CurrentFlow::Condition;
		state.CurrentGene++;
		state.BoolStack = std::stack<bool>();
		state.InGene = true;
		return false;
	}
	else if (value == 2 /* start */ || value == 3 /* else */ || value == 4 /* stop */) {
		// start by assuming inferring a stop command if there isn't already one there
		if (state.CurrentFlow == CurrentFlow::Condition) state.CurrentCondFlag = AddUpConditions(state);
		if (!state.InGene) state.CurrentCondFlag = CurrentConditionFlag::NextBody;

		state.CurrentFlow = CurrentFlow::Clear;

		switch (value) {
		case 2: // start
			if (!state.InGene) state.CurrentGene++;
			state.InGene = false;
			if (state.CurrentCondFlag == CurrentConditionFlag::NextBody)
				state.CurrentFlow = CurrentFlow::Body;
			break;
		case 3: // else
			if (state.CurrentCondFlag == CurrentConditionFlag::NextElse)
				state.CurrentFlow = CurrentFlow::ElseBody;
			if (!state.InGene) state.CurrentGene++;
			state.InGene = false;
			break;
		case 4: // stop
			state.InGene = false;
			state.CurrentFlow = CurrentFlow::Clear;
			break;
		}
		return true;
	}
	return false;
}

bool DNA_ConditionState(State& state) {
	return state.BoolStack.empty() || state.BoolStack.top();
}

void __stdcall DNA_Execute(Robot& rob, SimulationOptions& simOpts) {
	State state = State();
	// TODO : consider implementing gene activation flags.

	rob.Condnum = 0; // Resets the condition statement counter

	Block* dnaBlocks;
	HRESULT res = SafeArrayAccessData(rob.Dna, reinterpret_cast<void**>(&dnaBlocks));

	int currentBlock = 1;

	if (SUCCEEDED(res)) {
		LONG upperBound;

		res = SafeArrayGetUBound(rob.Dna, 1, &upperBound);

		if (SUCCEEDED(res)) {
			while (!(dnaBlocks[currentBlock].Tipo == 10 && dnaBlocks[currentBlock].Value == 1) && currentBlock <= 32000 && currentBlock < upperBound) {
				switch (dnaBlocks[currentBlock].Tipo) {
				case 0: // number
					if (state.CurrentFlow != CurrentFlow::Clear) {
						state.IntStack.push(dnaBlocks[currentBlock].Value);
						rob.Nrg -= simOpts.Costs[COST_NUMBER] * simOpts.Costs[COST_MULTIPLIER];
					}
					break;
				case 1: // *number
					if (state.CurrentFlow != CurrentFlow::Clear) {
						short b = dnaBlocks[currentBlock].Value;

						if (b > ROBOT_MAX_MEM || b < 1) {
							b = LocalMod(abs(dnaBlocks[currentBlock].Value), ROBOT_MAX_MEM);
						}

						state.IntStack.push(rob.Mem[b]);

						rob.Nrg -= simOpts.Costs[COST_DOT_NUMBER] * simOpts.Costs[COST_MULTIPLIER];
					}
					break;
				case 2: // commands (add, sub, etc.)
					if (state.CurrentFlow != CurrentFlow::Clear)
						DNA_ExecuteBasicCommand(rob, simOpts, state, dnaBlocks[currentBlock].Value);
					break;
				case 4: // advanced commands
					if (state.CurrentFlow != CurrentFlow::Clear)
						DNA_ExecuteAdvancedCommand(rob, simOpts, state, dnaBlocks[currentBlock].Value);
					break;
				case 5: // bitwise commands
					if (state.CurrentFlow != CurrentFlow::Clear)
						DNA_ExecuteBitwiseCommand(rob, simOpts, state, dnaBlocks[currentBlock].Value);
					break;
				case 6: // logic commands
					if (state.CurrentFlow == CurrentFlow::Condition || state.CurrentFlow == CurrentFlow::Body || state.CurrentFlow == CurrentFlow::ElseBody)
						DNA_ExecuteLogic(rob, simOpts, state, dnaBlocks[currentBlock].Value);
					break;
				case 7: // store commands
					if ((state.CurrentFlow == CurrentFlow::Body || state.CurrentFlow == CurrentFlow::ElseBody) && DNA_ConditionState(state))
						DNA_ExecuteStores(rob, simOpts, state, dnaBlocks[currentBlock].Value);
					break;
				case 9: // flow commands
					if (!DNA_ExecuteFlowCommand(rob, simOpts, state, dnaBlocks[currentBlock].Value))
						rob.Condnum++;
					rob.Mem[thisgene] = state.CurrentGene;
				}

				currentBlock++;
			}
		}

		SafeArrayUnaccessData(rob.Dna);
	}
}
