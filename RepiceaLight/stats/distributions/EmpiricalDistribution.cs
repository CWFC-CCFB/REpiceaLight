/*
 * This file is part of the REpiceaLight library.
 *
 * Copyright (C) 2026 His Majesty the King in right of Canada
 * Author: Mathieu Fortin, Canadian Forest Service
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed with the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE. See the GNU Lesser General Public
 * License for more details.
 *
 * Please see the license at http://www.gnu.org/copyleft/lesser.html.
 */
using REpiceaLight.math;
using System;

namespace REpiceaLight.stats.distributions
{
    public class EmpiricalDistribution : AbstractEmpiricalDistribution
    {


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
                        sum = (Matrix)mat.Clone();
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
            return (SymmetricMatrix)convertedSse.ScalarMultiply(1d / (observations.Count - 1));
        }


    }

}
