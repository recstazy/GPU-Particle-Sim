using System;

namespace SandSimulation
{
    public interface ICellDatabaseProvider : IDisposable
    {
        public CellDatabase Database { get; }
        void SaveChanges();
    }
}
