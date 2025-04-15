using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.stats.distributions
{
    public class GaussianDistribution : StandardGaussianDistribution, IMomentSettable
    {

    /**
	 * Constructor. <br>
	 * <br>
	 * Creates a Gaussian distribution with mean mu and variance sigma2. NOTE: Matrix sigma2 must be 
	 * positive definite.
	 * 
	 * @param mu the mean of the function
	 * @param sigma2 the variance of the function
	 * @throws UnsupportedOperationException if the matrix sigma2 is not positive definite
	 */
    public GaussianDistribution(Matrix mu, SymmetricMatrix sigma2)
    {
        SetMean(mu);
        SetVariance(sigma2);
    }

    /**
	 * Constructor for univariate Gaussian distribution.
	 * @param mean the mean of the distribution
	 * @param variance the variance of the distribution
	 */
    public GaussianDistribution(double mean, double variance)
    {
        Matrix mu = new(1, 1);
        mu.SetValueAt(0, 0, mean);
        SetMean(mu);
        SymmetricMatrix sigma2 = new(1);
        sigma2.SetValueAt(0, 0, variance);
        SetVariance(sigma2);
    }

    /**
	 * Constructor for univariate Gaussian distribution centered on 0 with variance 1.
	 */
    public GaussianDistribution() : this(0d, 1d)
    {
    }

    public new void SetMean(Matrix mean)
    {
        base.SetMean(mean);
    }

    public new void SetVariance(SymmetricMatrix variance)
    {
        base.SetVariance(variance);
    }

}

}
