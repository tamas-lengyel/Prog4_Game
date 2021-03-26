using System;

namespace Logic
{
    public interface IGameLogic<T> where T:class
    {
        event EventHandler RefreshScreen;
        event EventHandler Collide;
    }
}
