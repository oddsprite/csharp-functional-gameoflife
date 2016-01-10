using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class A
    {
        public static bool Match(Func<bool> predicate, Action result)
        {
            Func<bool> runResult = () =>
            {
                result();
                return true;
            };

            return predicate() && runResult();
        }
    }
}
