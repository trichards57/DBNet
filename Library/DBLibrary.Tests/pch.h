//
// pch.h
// Header for standard system include files.
//

#pragma once

#pragma warning(disable: 26495 26812) // Disables some code analysis warnings generated in the gtest header

#define OLE2ANSI
#define _USE_MATH_DEFINES

// add headers that you want to pre-compile here
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>
#include <cmath>
#include <cstdlib>
#include <ctime>

#include "gtest/gtest.h"
