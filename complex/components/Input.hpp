#pragma one
#include "../api/WinAPI.hpp"
#include "../api/Terminal.hpp"
#include <functional>
#include <string>

class Input : protected WinAPI {
private:
    Terminal *T = new Terminal();
    void BasicHandler();

    Couple<int, int> coordinates = Couple<int, int>();

public:
    Input() {
        coordinates = T->GetCursorPos();
        coordinates.first += 1;
    }

    /// @brief deletes pointers
    ~Input() {
        delete T;
    }

    void SetInput(std::function<void()> handler = nullptr, const std::string& placeholder = std::string("[Clique aqui                       ]"));
};