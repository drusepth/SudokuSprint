using System;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSprint
{
    class Board
    {
        // Consists of nine joined sectors
        Sector[] sectors = new Sector[9];

        public Board()
        {
            for (int i = 0; i < 9; i++)
            {
                sectors[i] = new Sector();
            }
        }

        // Validations
        public bool ValidSectors()
        {
            for (int i = 0; i < 9; i++)
            {
                bool valid = sectors[i].ValidSector();
                if (!valid)
                {
                    return false;
                }
            }

            return true;
        }
        public bool ValidRows()
        {
            for (int i = 1; i <= 9; i++)
            {
                if (ValidRow(i) == false)
                {
                    return false;
                }
            }

            return true;
        }
        public bool ValidRow(int row_index) // Row index is 1-based: 1 gives (0, 1, 2, ...), 2 gives (9, 10, 11, ...), etc
        {
            // Ensure the row[row_index] doesn't have duplicate values
            List<char> values = new List<char>();
            int y = row_index - 1;
            for (int x = 9 * 0; x < 9; x++)
            {
                char val = this[x, y];
                if (val != Sector.UNKNOWN)
                {
                    values.Add(val);
                }
            }

            return values.Count == values.Distinct().Count();
        }
        public bool ValidColumns()
        {
            for (int i = 1; i <= 9; i++)
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
            int x = column_index - 1;
            List<char> values = new List<char>();
            for (int y = 0; y < 9; y++)
            {
                char val = this[x, y];
                if (val != Sector.UNKNOWN)
                {
                    values.Add(val);
                }
            }

            return values.Count == values.Distinct().Count();
        }
        public bool ValidBoard()
        {
            return ValidColumns() && ValidRows();
        }

        // Board information
        public List<char> PossibleValuesFor(int x, int y)
        {
            HashSet<char> possibilities = new HashSet<char>();
            int sector_index = sector_map[y * 9 + x];

            List<char> row_possibilities = PossibleValuesForRow(x + 1);
            List<char> column_possibilities = PossibleValuesForColumn(y + 1);
            possibilities = Helper.Intersection(row_possibilities, column_possibilities);

            List<char> sector_possibilities = sectors[sector_index].UnusedValues();
            possibilities = Helper.Intersection(possibilities, sector_possibilities);

            return possibilities.ToList<char>();
        } // Values that have been used in this sector
        public List<char> PossibleValuesForRow(int row_index)
        {
            List<char> possibilities = new List<char>();
            bool[] seen = new bool[10]; // Should default false

            int y = row_index - 1;
            for (int x = 0; x < 9; x++)
            {
                if (this[x, y] == Sector.UNKNOWN) continue;

                int num = this[x, y] - ('1' - 1); // Magic, assuming only numbers
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
        }
        public List<char> PossibleValuesForColumn(int column_index)
        {
            List<char> possibilities = new List<char>();
            bool[] seen = new bool[10]; // Should default false

            int x = column_index - 1;
            for (int y = 0; y < 9; y++)
            {
                if (this[x, y] == Sector.UNKNOWN) continue;

                int num = this[x, y] - ('1' - 1); // Magic, assuming only numbers
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
        }
        public bool IsSolved()
        {
            for (int i = 0; i < 81; i++)
            {
                if (this[i] == Sector.UNKNOWN)
                {
                    return false;
                }
            }

            return true;
        }
        public bool NotSolved()
        {
            for (int i = 0; i < 81; i++)
            {
                if (this[i] == Sector.UNKNOWN)
                {
                    return true;
                }
            }

            return false;
        }

        public void PrintBoard()
        {
            for (int i = 0; i < 9 * 9; i++)
            {
                if (i != 0 && i % 9 == 0)
                {
                    Console.WriteLine("|");
                }
                
                if (i % 27 == 0)
                {
                    Console.WriteLine("  - - -   - - -   - - -");
                }

                if (i % 3 == 0)
                {
                    Console.Write("| ");
                }

                Console.Write("{0} ", this[i]);
            }

            Console.WriteLine("|");
            Console.WriteLine("  - - -   - - -   - - -");
        }

        // Cell accessor: (0, 0) being top-left, (0, 1) being right beneath it
        public char this[int x, int y]
        {
            get
            {
                int board_offset = y * 9 + x;
                int sector_index = sector_map[board_offset];
                int sector_offset = offset_map[board_offset];

                return sectors[sector_index][sector_offset];
            }
            set
            {
                int board_offset = y * 9 + x;
                int sector_index = sector_map[board_offset];
                int sector_offset = offset_map[board_offset];

                sectors[sector_index][sector_offset] = value;
            }
        }

        // Cell accessor: row-major order
        public char this[int board_offset]
        {
            get
            {
                int sector_index = sector_map[board_offset];
                int sector_offset = offset_map[board_offset];

                return sectors[sector_index][sector_offset];
            }
        }

        #region lookup tables
        // Hard-code things because lol, O(1) lookup
        int[] sector_map = new int[] {
            0, 0, 0, 1, 1, 1, 2, 2, 2, 
            0, 0, 0, 1, 1, 1, 2, 2, 2, 
            0, 0, 0, 1, 1, 1, 2, 2, 2, 
            3, 3, 3, 4, 4, 4, 5, 5, 5,
            3, 3, 3, 4, 4, 4, 5, 5, 5,
            3, 3, 3, 4, 4, 4, 5, 5, 5,
            6, 6, 6, 7, 7, 7, 8, 8, 8,
            6, 6, 6, 7, 7, 7, 8, 8, 8,
            6, 6, 6, 7, 7, 7, 8, 8, 8
        };

        int[] offset_map = new int[] {
            0, 1, 2, 0, 1, 2, 0, 1, 2,
            3, 4, 5, 3, 4, 5, 3, 4, 5,
            6, 7, 8, 6, 7, 8, 6, 7, 8,
            0, 1, 2, 0, 1, 2, 0, 1, 2,
            3, 4, 5, 3, 4, 5, 3, 4, 5,
            6, 7, 8, 6, 7, 8, 6, 7, 8,
            0, 1, 2, 0, 1, 2, 0, 1, 2,
            3, 4, 5, 3, 4, 5, 3, 4, 5,
            6, 7, 8, 6, 7, 8, 6, 7, 8
        };
        #endregion lookup tables
    }
}
