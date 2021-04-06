using System;

namespace Model
{
    public interface IGameModel<T> where T : class
    {
        int DX { get; set; }
        int DY { get; set; }
        string Id { get; set; }
    }
}
