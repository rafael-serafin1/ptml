#pragma once
#include "./components/Components.hpp"

#ifdef _WIN32
    #define DLL_EXPORT extern "C" __declspec(dllexport)
#else
    #define DLL_EXPORT extern "C"
#endif

DLL_EXPORT class Button;
DLL_EXPORT class Input;