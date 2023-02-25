// TODO: Add 128-bit stride to increase structured buffer read performance
// https://developer.nvidia.com/content/understanding-structured-buffer-performance

struct CellConfig
{
    int2 id; // x - id, y - isStatic
    float2 textureOffset;
};

struct Cell
{
    int id;
    int2 position;
    int dummy;
};