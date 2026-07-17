#include "NativeComponentsLib.hpp"
#include "components/Components.hpp"
#include "utils/Utils.hpp"
#include <iostream>
#include <stdio.h>

int main(int argc, char **argv) {
    Button *btn = new Button();
    btn->SetPosition(0, 0);

    btn->SetButton([]() {
        Terminal T = Terminal();
        T.ClearAll();
        T.ToggleCursorVisibility();
        register int op = 0;

        do {
            std::cout << "===== MENU =====\n";
            std::cout << "1. Comprar\n";
            std::cout << "2. Vender\n";
            std::cout << "3. Perecer\n";
            std::cout << "================\n";
            scanf("%i", &op);
            switch (op) {

            }
        } while (op != 4);
    }, "[ abrir menu ]");

    btn->SetPosition(1, 1);

    btn->SetButton([]() {
        return;
    }, "[ sair ]");

    delete btn;
    return 0;
}