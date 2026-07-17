#pragma once
#include <iostream>
#include "WinAPI.hpp"

#define CLEAR_ESCAPE "\x1B["

template<typename T, typename Y> struct Couple {
    T first;
    Y second;
};

class Terminal {
private:
    // terminal size
    Couple<int, int> *_terminal_size;

    // cursor positions
    Couple<int, int> *_cursor_pos; 

    // cursor visibility
    bool isCursorVisible;

    /// @brief set cursor position info into pointer _cursor_pos
    /// @param X position
    /// @param Y position
    void set_cursor_pos(int X, int Y) {
        _cursor_pos->first = X;
        _cursor_pos->second = Y;
    }

    Couple<int, int> get_cursor_pos() const {
        return *_cursor_pos;
    }

    void set_cursor_visibility(bool state) {
        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        CONSOLE_CURSOR_INFO cursorInfo;
        GetConsoleCursorInfo(hOut, &cursorInfo);
        cursorInfo.bVisible = state ? TRUE : FALSE;
        SetConsoleCursorInfo(hOut, &cursorInfo);

        this->isCursorVisible = state;
    }

    bool get_cursor_visibility() const {
        return this->isCursorVisible;
    }
    
public:

    /// @brief Constructor
    Terminal() {
        _cursor_pos = new Couple<int, int>; 
        _terminal_size = new Couple<int, int>; 
    }

    /// @brief Destructor
    ~Terminal() {
        delete _cursor_pos, _terminal_size;
    }

    /// @brief Cursor position
    /// @return couple of type int, int
    Couple<int, int> GetCursorPos() const {
        CONSOLE_SCREEN_BUFFER_INFO csbi;
        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleScreenBufferInfo(hOut, &csbi);

        Terminal t = Terminal();
        t.set_cursor_pos(csbi.dwCursorPosition.X, csbi.dwCursorPosition.Y);
        return t.get_cursor_pos();
    }

    /// @brief Set cursor to a specific position
    /// @param X x coordinates
    /// @param Y y coordinates
    void SetCursorPos(int X, int Y) {
        if (X < 0 || Y < 0) throw std::invalid_argument("Invalid argument type: \'negative number\'.");

        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        COORD coord;
        coord.X = static_cast<SHORT>(X);
        coord.Y = static_cast<SHORT>(Y);
        SetConsoleCursorPosition(hOut, coord);

        this->set_cursor_pos(X, Y);
    }

    /// @brief set cursor visibility 
    /// @param to boolean state value
    void SetCursorVisibility(bool to) {
        this->set_cursor_visibility(to);
    }

    /// @brief toggles cursor's visibility
    void ToggleCursorVisibility() {
        bool actualState = this->get_cursor_visibility();

        if (!actualState) this->set_cursor_visibility(true);
        else this->set_cursor_visibility(false);
    }

public:
    /// @brief kinda obvious ngl
    inline static void ClearAll() {
        std::cout << "\x1B[2J";
        Terminal t = Terminal(); 
        t.SetCursorPos(0, 0);
    }

    inline static void ClearLine(int X, int Y) {
        if (X < 0 || Y < 0) throw std::invalid_argument("Invalid argument type: \'negative number\'.");

        Terminal T = Terminal();
        T.SetCursorPos(X, Y);

        std::string final_msn = CLEAR_ESCAPE + X + ';' + Y + 'H' + std::string(CLEAR_ESCAPE) + "2K";
        std::cout << final_msn;
        T.SetCursorPos(0, Y);
    }
};