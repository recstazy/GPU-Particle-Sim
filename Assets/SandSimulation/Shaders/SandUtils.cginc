uint _SandGridWidth;
uint _SandGridHeight;
uint _MaxSandId;
uint _DefaultSandId;
float _SandCellSize;
float _SandParticleSize;

float GetParticleDiameterSqr()
{
    return _SandParticleSize * _SandParticleSize;
}

float GetParticleRadius()
{
    return _SandCellSize * 0.5;
}

bool IsInBounds(int2 gridPos)
{
    return gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < _SandGridWidth && gridPos.y < _SandGridHeight;
}

float2 GetCellCenter(uint2 gridPos)
{
    return gridPos + _SandCellSize * 0.5;
}

float SqrLength(float2 vec)
{
    return vec.x * vec.x + vec.y * vec.y;
}

uint GetPlaneLength()
{
    return _SandGridWidth * _SandGridHeight;
}

float GetPlaneLengthFloat()
{
    return GetPlaneLength();
}

uint ToPlaneIndex(int2 gridPos)
{
    return gridPos.y * _SandGridWidth + gridPos.x;
}

uint ToPlaneIndex(uint3 id)
{
    return ToPlaneIndex(id.xy);
}

uint2 ToGridPos(uint id)
{
    return uint2(id % _SandGridWidth, id / _SandGridWidth);
}

uint2 GetGridSize()
{
    return uint2(_SandGridWidth, _SandGridHeight); 
}

float2 GetGridSizeFloat()
{
    return float2(_SandGridWidth, _SandGridHeight);
}

uint2 UvToGridPos(float2 uv)
{
    return floor(uv * GetGridSizeFloat());
}

float4 VisualizeGridPos(uint2 gridPos, float alpha)
{
    return float4(gridPos / GetGridSizeFloat(), 0, alpha);
}

float2 GetCellUV(float2 uv, uint2 gridPos)
{
    const float2 cellStartUV = gridPos / GetGridSizeFloat();
    const float2 cellEndUV = (gridPos + 0.999) / GetGridSizeFloat();
    return  (uv - cellStartUV) / (cellEndUV - cellStartUV);
}
