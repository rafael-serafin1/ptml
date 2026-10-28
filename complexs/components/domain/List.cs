using complexs.components;
namespace complexs.components.domain;

public class ListComponent: Component
{
    private List<string> _itens = default;

    public ListComponent(List<string> itens, Coordinates position, int width, int height, string placeholder = string.Empty): 
        base(position, width, height, placeholder)
    {
        _itens = itens;
    }
}