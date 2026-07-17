#include <iostream>
#include "../utils/Utils.hpp"
#include "Input.hpp"

void Input::BasicHandler() {
    Terminal T = __basic_setings();
    T.ToggleCursorVisibility();

    std::string input = "";
    std::cin >> input;
    Couple<int, int> c = T.GetCursorPos();

    T.ClearLine(c.first, c.second);
    std::cout << "O input foi: " << input;

    T.ToggleCursorVisibility();
    return;
}

void Input::SetInput(std::function<void()> handler, const std::string& placeholder) {
    if (!handler) handler = [this]() { BasicHandler(); };

    T->SetCursorVisibility(false);

    // Desenha o botão
    int bx = 0;
    int by = 0;

    Terminal::ClearAll();
    T->SetCursorPos(bx + 1, by);
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