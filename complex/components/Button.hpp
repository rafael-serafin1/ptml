#pragma once
#include "../api/WinAPI.hpp"
#include "../api/Terminal.hpp"
#include <functional>
#include <string>
 
class Button : protected WinAPI {
public:
    enum class ButtonTypes {
        Common  = 0,
        Scanner = 1,
        Radio   = 2
    };
    Terminal *T = new Terminal();
    
    void SetButton(std::function<void()> handler = nullptr, const std::string& placeholder = std::string("[ Clique Aqui ]\n"));

    ~Button() {
        delete this->T;
    }

private:
    int counter = 0;
    
    /// @fn Handler Básico
    void BasicHandler();                                   
 
    // helpers
    void GetCursorPos(int& x, int& y) const;                // ? Get current cursor position 
    void SetCursorPos(int x, int y) const;                  // ? Set cursor position to coordinates
    void SetCursorVisible(bool visible) const;              // ? Set cursor visibility
};