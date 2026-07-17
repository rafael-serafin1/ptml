#pragma once
#include "../api/WinAPI.hpp"
#include "../api/Terminal.hpp"
#include <functional>
#include <string>
 
#define PLACEHOLDER "[ Clique Aqui ]\n"
#define Coordinates Couple<int, int>

class Button : protected WinAPI {
public:
    enum class ButtonTypes {
        Common  = 0,
        Scanner = 1,
        Radio   = 2
    };
    
    void SetButton(std::function<void()> handler = nullptr, const std::string& placeholder = std::string(PLACEHOLDER));
    
    Button() = default;
    ~Button() {
        delete T;
    }
    
    void SetPosition(int X, int Y) {
        this->X = X;
        this->Y = Y;
    }

    Coordinates GetPosition() {
        Coordinates C = Coordinates();
        C.first = this->X;
        C.second = this->Y;
        return C;
    }

private:
    Terminal *T = new Terminal();
    int counter = 0;
    
    /// @fn Handler Básico
    void BasicHandler();     
    
    // coordinates
    int X = 0;
    int Y = 0;

    // helpers
    void GetCursorPos(int& x, int& y) const;                // ? Get current cursor position 
    void SetCursorPos(int x, int y) const;                  // ? Set cursor position to coordinates
    void SetCursorVisible(bool visible) const;              // ? Set cursor visibility
};