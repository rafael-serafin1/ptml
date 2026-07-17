#pragma once
#include "../components/Button.hpp"

inline Terminal __basic_setings() {
    std::cout << std::endl;
    Terminal::ClearLine(1, 1);
    Terminal T = Terminal();
    T.SetCursorPos(0, 1);
    return T;
}