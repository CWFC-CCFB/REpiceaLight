using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REpiceaLight.simulation
{
    public class HierarchicalLevel
    {


        public static readonly HierarchicalLevel PLOT = new("plot");
        public static readonly HierarchicalLevel TREE = new("tree");
        public static readonly HierarchicalLevel YEAR = new("year");
        public static readonly HierarchicalLevel INTERVAL_NESTED_IN_PLOT = new("interval_nested_in_plot");
        public static readonly HierarchicalLevel CRUISE_LINE = new("cruise_line");

        private readonly string levelName;

        protected HierarchicalLevel(String levelName)
        {
            this.levelName = levelName;
        }

        internal string GetName() { return levelName; }

        public override string ToString() { return GetName(); }

    }
}
