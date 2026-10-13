#include "NativeComponentsLib.hpp"
#include "components/Components.hpp"
#include "utils/Utils.hpp"
#include <iostream>
#include <stdio.h>

int main(int argc, char **argv) {
    Input *inpt = new Input();
    Button *btn = new Button();
    btn->SetPosition(0, 0);

    inpt->SetInput([](){});

    btn->SetButton([&inpt](){   
        printf("%s");
    }, "[ sair ]");

    delete btn;
    return 0;
}