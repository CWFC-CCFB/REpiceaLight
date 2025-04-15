using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.distributions
{
    public class EmpiricalDistribution : AbstractEmpiricalDistribution {


    public override Matrix GetMean()
    {
        if (observations == null || observations.Count == 0)
        {
            return null;
        }
        else
        {
            Matrix sum = null;
            foreach (Matrix mat in observations)
            {
                if (sum == null)
                    sum = mat.Clone();
                else
                    sum = sum.Add(mat);
            }
            return sum.ScalarMultiply(1d / observations.Count);
        }
    }

    public override SymmetricMatrix GetVariance()
    {
        Matrix mean = GetMean();
        if (!mean.IsColumnVector())
            throw new InvalidOperationException("The variance cannot be calculated since the vector is not a column vector!");
        Matrix sse = null;
        Matrix error;
        foreach (Matrix mat in observations)
        {
            error = mat.Subtract(mean);
            if (sse == null)
                sse = error.Multiply(error.Transpose());
            else
                sse = sse.Add(error.Multiply(error.Transpose()));
        }
        SymmetricMatrix convertedSse = SymmetricMatrix.ConvertToSymmetricIfPossible(sse);
        return convertedSse.ScalarMultiply(1d / (observations.Count - 1));
    }


}

}
