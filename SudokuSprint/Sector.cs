using System;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSprint
{
    // A Sector onsists of nine joined cells, ordered with the following indexes:
    //  0 1 2
    //  3 4 5
    //  6 7 8
    class Sector
    {
        public const char UNKNOWN = '0'; // The character to represent an unknown value in output

        char[] cells = new char[9];

        public Sector()
        {
            // Initialize all cells to unknown values
            for (int i = 0; i < 9; i++)
            {
                cells[i] = UNKNOWN;
            }
        }

        // Validations
        public bool ValidRows()
        {
            for (int i = 1; i <= 3; i++)
            {
                if (ValidRow(i) == false)
                {
                    return false;
                }
            }

            return true;
        }
        public bool ValidRow(int row_index) // Row index is 1-based: 1 gives (0, 1, 2), 2 gives (3, 4, 5), etc
        {
            // Ensure the row[row_index] doesn't have duplicate values
            List<char> values = new List<char>();
            for (int i = (3 * row_index) - 3; i < (3 * row_index); i++)
            {
                if (this[i] != UNKNOWN)
                {
                    values.Add(this[i]);
                }
            }

            return values.Count == values.Distinct().Count();
        }
        public bool ValidColumns()
        {
            for (int i = 1; i <= 3; i++)
            {
                if (ValidColumn(i) == false)
                {
                    return false;
                }
            }

            return true;
        }
        public bool ValidColumn(int column_index) // 1-based: 1 gives (0, 1, 2), 2 gives (3, 4, 5), etc
        {
            int[] cells;
            
            // I could calculate these values but I'm tired #todo
            switch (column_index)
            {
                case 1: cells = new int[3] { 0, 3, 6 }; break;
                case 2: cells = new int[3] { 1, 4, 7 }; break;
                case 3: cells = new int[3] { 2, 5, 8 }; break;
                default: throw new IndexOutOfRangeException("column_index should be between 1 and 3");
            }
            // note to future me: the pattern is start: column_index, step: +3

            List<char> values = new List<char>();
            for (int i = 0; i < 3; i++)
            {
                int real_index = cells[i];
                if (this[real_index] != UNKNOWN)
                {
                    values.Add(this[real_index]);
                }
            }

            return values.Count == values.Distinct().Count();
        }
        public bool ValidSector()
        {
            bool[] seen = new bool[10]; // Should default false

            for (int i = 0; i < 9; i++)
            {
                if (this[i] == UNKNOWN) continue;

                int num = this[i] - ('1' - 1); // Magic, assuming only numbers

                if (seen[num])
                {
                    return false;
                }

                seen[num] = true;
            }

            return true;
        }

        // Sector state information and whatnot
        public List<char> UnusedValues()
        {
            List<char> possibilities = new List<char>();
            bool[] seen = new bool[10]; // Should default false

            for (int i = 0; i < 9; i++)
            {
                if (this[i] == UNKNOWN) continue;

                int num = this[i] - ('1' - 1); // Magic, assuming only numbers
                seen[num] = true;
            }

            for (int i = 1; i < 10; i++)
            {
                if (seen[i] == false)
                {
                    char convert = (char)(i + '1' - 1);
                    possibilities.Add(convert);
                }
            }

            return possibilities;
        } // Values that have yet to be used in this sector
        public List<char> UsedValues()
        {
            List<char> possibilities = new List<char>();
            bool[] seen = new bool[10]; // Should default false

            for (int i = 0; i < 9; i++)
            {
                if (this[i] == UNKNOWN) continue;

                int num = this[i] - ('1' - 1); // Magic, assuming only numbers
                seen[num] = true;
            }

            for (int i = 1; i < 10; i++)
            {
                if (seen[i] == true)
                {
                    char convert = (char)(i + '1' - 1);
                    possibilities.Add(convert);
                }
            }

            return possibilities;
        } // Values that have been used in this sector

        // Cell accessor shorthand: sector[4] gives sector->cells[4]
        public char this[int i]
        {
            get { return cells[i]; }
            set { cells[i] = value; }
        }
    }
}
