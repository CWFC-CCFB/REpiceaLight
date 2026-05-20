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
using System;

namespace REpiceaLight.simulation
{
    /// <summary>
    /// A class providing static instances defining the hierarchical class of an instance.
    /// </summary>
    public class HierarchicalLevel
    {


        public static readonly HierarchicalLevel PLOT = new HierarchicalLevel("plot");
        public static readonly HierarchicalLevel TREE = new HierarchicalLevel("tree");
        public static readonly HierarchicalLevel YEAR = new HierarchicalLevel("year");
        public static readonly HierarchicalLevel INTERVAL_NESTED_IN_PLOT = new HierarchicalLevel("interval_nested_in_plot");
        public static readonly HierarchicalLevel CRUISE_LINE = new HierarchicalLevel("cruise_line");

        private readonly string levelName;

        protected HierarchicalLevel(String levelName)
        {
            this.levelName = levelName;
        }

        internal string GetName() { return levelName; }

        public override string ToString() { return GetName(); }

    }
}
