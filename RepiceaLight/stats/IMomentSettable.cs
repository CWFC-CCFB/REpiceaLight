using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats
{
    public interface IMomentSettable
    {
        public void SetMean(Matrix m);

        public void SetVariance(SymmetricMatrix v);
    }
}
