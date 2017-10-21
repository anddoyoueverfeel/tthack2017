using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuffle
{
    abstract public class ISteppable<T>
    {
        abstract public T step();
    }
}
