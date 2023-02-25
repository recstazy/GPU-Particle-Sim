namespace SandSimulation
{
    public interface ISandModel
    {
        NativeGrid<Cell> Cells { get; }
    }
}
