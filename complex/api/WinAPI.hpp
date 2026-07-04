#pragma once
 
// Em C# foi necessário declarar manualmente (via P/Invoke) as funções e
// estruturas da Win32 API porque o .NET não as expõe nativamente.
// Em C++ isso não é preciso: <windows.h> já traz GetStdHandle,
// GetConsoleMode, SetConsoleMode, ReadConsoleInput, INPUT_RECORD,
// MOUSE_EVENT_RECORD, COORD, etc. Esta classe apenas concentra as
// constantes usadas, mantendo a mesma organização do código original.
#include <windows.h>
 
class WinAPI
{
protected:
    static constexpr DWORD kStdInputHandle          = STD_INPUT_HANDLE;
    static constexpr DWORD kEnableMouseInput         = ENABLE_MOUSE_INPUT;
    static constexpr DWORD kEnableExtendedFlags      = ENABLE_EXTENDED_FLAGS;
    static constexpr DWORD kEnableQuickEditMode      = ENABLE_QUICK_EDIT_MODE;
 
    static constexpr WORD  kMouseEvent               = MOUSE_EVENT;
    static constexpr DWORD kFromLeft1stButtonPressed  = FROM_LEFT_1ST_BUTTON_PRESSED;
};