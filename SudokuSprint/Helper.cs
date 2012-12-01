using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSprint
{
    class Helper
    {
        public static HashSet<char> Intersection(IEnumerable<char> a, IEnumerable<char> b)
        {
            var set = new HashSet<char>(a);
            set.IntersectWith(b);

            return set;
        }
    }
}
