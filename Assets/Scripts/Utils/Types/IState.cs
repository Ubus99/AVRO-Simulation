using System;

namespace Utils.Types
{
    public interface IState
    {
        public event EventHandler<IState> OnExit;
        public event EventHandler OnEntry;
        public event EventHandler OnDo;
    }
}
