#include "Button.hpp"
#include <iostream>
#include <cstdlib>
#include <stdio.h>

#pragma region 
    /// @brief Get cursor current position
    /// @param x X coordinates references
    /// @param y Y coordinates references
    void Button::GetCursorPos(int& x, int& y) const
    {
        CONSOLE_SCREEN_BUFFER_INFO csbi;
        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleScreenBufferInfo(hOut, &csbi);
        x = csbi.dwCursorPosition.X;
        y = csbi.dwCursorPosition.Y;
    }

    /// @brief Set cursor to position
    /// @param x Position X
    /// @param y Position Y
    void Button::SetCursorPos(int x, int y) const
    {
        if (x < 0 || y < 0) 
            throw std::invalid_argument("Negative value provided.");

        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        COORD coord;
        coord.X = static_cast<SHORT>(x);
        coord.Y = static_cast<SHORT>(y);
        SetConsoleCursorPosition(hOut, coord);
    }

    /// @brief Toggle cursor visibility
    /// @param visible boolean -- true or false
    void Button::SetCursorVisible(bool visible) const
    {
        HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
        CONSOLE_CURSOR_INFO cursorInfo;
        GetConsoleCursorInfo(hOut, &cursorInfo);
        cursorInfo.bVisible = visible ? TRUE : FALSE;
        SetConsoleCursorInfo(hOut, &cursorInfo);
    }
#pragma endregion

// * optional handler for missing function in parameter
void Button::BasicHandler() {
    register Couple<int, int> coordinates = T->GetCursorPos();
    T->SetCursorVisibility(false);

    counter += 1;

    SetCursorPos(coordinates.first, coordinates.second);
    std::cout << "Contagem: " << counter << std::endl;
    SetCursorPos(coordinates.first, coordinates.second);
}

/// @brief Draw and watches button
/// @param handler handler for click event
/// @param placeholder button placeholder string
void Button::SetButton(std::function<void()> handler, const std::string& placeholder) {
    if (!handler) handler = [this]() { BasicHandler(); };
    Coordinates cdn = this->GetPosition();

    T->SetCursorVisibility(false);

    // Desenha o botão
    Button::SetPosition(0, 0);
    Terminal::ClearAll();

    int bx = cdn.first;
    int by = cdn.second;

    T->SetCursorPos(bx, by);

    std::cout << placeholder;

    // Habilita eventos do mouse
    HANDLE handle = GetStdHandle(kStdInputHandle);

    DWORD mode;
    GetConsoleMode(handle, &mode);

    mode &= ~kEnableQuickEditMode;
    mode |= kEnableExtendedFlags;
    mode |= kEnableMouseInput;

    SetConsoleMode(handle, mode);

    while (true)
    {
        INPUT_RECORD record[1];
        bool pressionadoNoBotao = false;

        while (true)
        {
            DWORD eventosLidos;
            ReadConsoleInputA(handle, record, 1, &eventosLidos);

            if (record[0].EventType != kMouseEvent)
                continue;

            // Na struct nativa do Windows o campo de mouse fica dentro
            // da union "Event" (record[0].Event.MouseEvent), diferente
            // do MouseEvent.cs feito por P/Invoke que expunha o campo
            // direto via FieldOffset.
            const MOUSE_EVENT_RECORD& mouse = record[0].Event.MouseEvent;

            int mx = mouse.dwMousePosition.X;
            int my = mouse.dwMousePosition.Y;

            bool sobreBotao =
                mx >= bx &&
                mx < bx + static_cast<int>(placeholder.length()) &&
                my == by;

            // Mouse pressionado
            if ((mouse.dwButtonState & kFromLeft1stButtonPressed) != 0)
            {
                if (sobreBotao)
                    pressionadoNoBotao = true;
            }
            // Mouse liberado
            else
            {
                if (pressionadoNoBotao && sobreBotao)
                {
                    handler();
                }

                pressionadoNoBotao = false;
            }
        }
    }
}