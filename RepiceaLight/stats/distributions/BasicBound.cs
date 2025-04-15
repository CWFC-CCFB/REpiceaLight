using REpiceaLight.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REpiceaLight.stats.distributions
{
    public class BasicBound
    {

        private bool isUpperBound;
        private Matrix value;


        protected BasicBound(bool IsUpperBound)
        {
            this.isUpperBound = IsUpperBound;
        }

        internal virtual void SetBoundValue(Matrix value)
        {
            this.value = value;
        }

        public virtual Matrix GetBoundValue() { return value; }

        protected bool IsUpperBound() { return isUpperBound; }
    }

}
