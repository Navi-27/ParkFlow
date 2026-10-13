using System;
using System.Collections.Generic;
using System.Text;

namespace ParkFlow.Contracts
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
