#include "NativeComponentsLib.hpp"
#include "components/Button.hpp"
#include <iostream>

int main(int argc, char **argv) {
    Button *btn = new Button();
    
    btn->SetButton([]() {
        std::cout << std::endl;
        Terminal::ClearLine(1, 1);
        Terminal T = Terminal();
        T.SetCursorPos(0, 1);
        std::cout << "Bom dia!";
    }, "[ executar ]");

    delete btn;
    return 0;
}