using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Contracts
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
