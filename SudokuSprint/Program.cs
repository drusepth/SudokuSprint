using System;
using System.Diagnostics;

namespace SudokuSprint
{
    class Program
    {
        static void Main(string[] args)
        {
            SectorTests();
            BoardTests();

            // Pause
            Console.WriteLine("You da man");
            Console.Read();
        }

        

        static void SectorTests()
        {
            Sector s = new Sector();
            Debug.Assert(s[0] == Sector.UNKNOWN && s[8] == Sector.UNKNOWN, "Incorrect initialization of UNKNOWN values");

            Debug.Assert(s.ValidColumn(1), "ValidColumn fails for three UNKNOWNs");
            Debug.Assert(s.ValidRow(1), "ValidRow fails for three UNKNOWNs");

            s[4] = '4';
            Debug.Assert(s[4] == '4', "Sector assignment does not work");

            Debug.Assert(s.ValidRow(2), "ValidRow returns false for two UNKNOWN values in a row");
            s[5] = '5';
            Debug.Assert(s.ValidRow(2), "ValidRow returns false for two unique values in a row");
            s[5] = '4';
            Debug.Assert(s.ValidRow(2) == false, "ValidRow returns true for two nonunique values in a row");
            s[3] = '3';
            s[5] = '5';
            Debug.Assert(s.ValidRow(2), "ValidRow returns false for three unique values in a row");

            Debug.Assert(s.ValidRow(1), "ValidRow works on row 1");
            Debug.Assert(s.ValidRow(3), "ValidRow works on row 3");

            Debug.Assert(s.ValidColumn(1) && s.ValidColumn(2) && s.ValidColumn(3), "ValidColumn fails with 1 unique value in column");

            // Current state:
            // . . .
            // 3 4 5
            // . . .

            s[0] = '5';
            Debug.Assert(s.ValidColumn(1), "ValidColumn fails with 2 uniques");

            s[6] = '5';
            Debug.Assert(s.ValidColumn(1) == false, "ValidColumn should return false for two nonunique numbers");

            Debug.Assert(s.ValidSector() == false, "ValidSector fails with two repeating numbers in Column 1");
            s[6] = '6';
            Debug.Assert(s.ValidSector() == false, "ValidSector should return false");
            s[0] = '9';
            Debug.Assert(s.ValidSector(), "ValidSector should return true");

            // Current state: 
            // 9 . .
            // 3 4 5
            // 6 . .

            Debug.Assert(s.UnusedValues().Count == 4, "UnusedValues doesn't work with 4 missing values");

            s[1] = '1';
            s[2] = '2';
            s[7] = '7';
            Debug.Assert(s.UnusedValues().Count == 1, "UnusedValues doesn't work with just 1 missing value");

            s[2] = '7';
            Debug.Assert(s.UnusedValues().Count == 2, "UnusedValues should ignore invalid sectors");

            s[2] = '2';
            s[8] = '8';

            Debug.Assert(s.UnusedValues().Count == 0, "UnusedValues doesn't work for no values left");
            Debug.Assert(s.UsedValues().Count == 9, "UsedValues should return 9 when they are all used");

            // Current state:
            // 9 1 2
            // 3 4 5
            // 6 7 8
        }
        static void BoardTests()
        {
            Board b = new Board();
            // Initial state:
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .

            b[0, 0] = '0';
            b[0, 1] = '1';
            b[1, 0] = '2';
            Debug.Assert(b[0, 0] == '0', "Board assignment doesn't work");
            Debug.Assert(b[0, 1] == '1', "Board assignment doesn't work");
            Debug.Assert(b[1, 0] == '2', "Board assignment doesn't work");

            b[3, 0] = '4';
            Debug.Assert(b[3, 0] == '4', "Assignment to top-mid sector doesn't work");

            b[0, 3] = '4';
            Debug.Assert(b[3, 0] == '4', "Assignment to left-mid sector doesn't work");

            b[4, 4] = '3';
            Debug.Assert(b[4, 4] == '3', "Assignment to mid-mid sector doesn't work");

            // Current state:
            //   - - -   - - -   - - -
            // | 0 2 . | 4 . . | . . . |
            // | 1 . . | . . . | . . . |
            // | . . . | . . . | . . . |
            //   - - -   - - -   - - -
            // | 4 . . | . . . | . . . |
            // | . . . | . 3 . | . . . |
            // | . . . | . . . | . . . |
            //   - - -   - - -   - - -
            // | . . . | . . . | . . . |
            // | . . . | . . . | . . . |
            // | . . . | . . . | . . . |
            //   - - -   - - -   - - -

            for (int i = 1; i < 9; i++)
            {
                Debug.Assert(b.ValidRow(i), "Row " + i + "should have returned Valid");
            }

            for (int i = 1; i < 9; i++)
            {
                Debug.Assert(b.ValidColumn(i), "Column " + i + "should have returned Valid");
            }

            b[0, 8] = '4';
            Debug.Assert(b.ValidColumn(1) == false, "Column 1 should have been invalid");

            b[8, 0] = '2';
            Debug.Assert(b.ValidRow(1) == false, "Row 1 should have been invalid");

            Debug.Assert(b.ValidSectors() == true, "Sectors should have returned true");
            Debug.Assert(b.ValidBoard() == false, "ValidBoard should have returned false, 2 problems");

            b[8, 0] = '7';
            Debug.Assert(b.ValidBoard() == false, "ValidBoard should have returned false, 1 problem");

            b[0, 8] = '6';
            Debug.Assert(b.ValidBoard() == true, "ValidBoard should have returned true");

            b[0, 0] = '3';

            // Current state:
            //   - - -   - - -   - - -   For x:
            // | 3 2 x | 4 . . | . . 7 | Row: 1 5 6 8 9, Column: 1 2 3 4 5 6 7 8 9, Sector: 4 5 6 7 8 9
            // | 1 . . | . . . | . . . | Possible for slot: 5 6 8 9
            // | . . . | . . . | . . . |
            //   - - -   - - -   - - -
            // | 4 . . | . . . | . . . |
            // | . . . | . 3 . | . . . |
            // | . . . | . . . | . . . |
            //   - - -   - - -   - - -
            // | . . . | . . . | . . . |
            // | . . . | . . . | . . . |
            // | 6 . . | . . . | . . . |
            //   - - -   - - -   - - -

            var possible_values = b.PossibleValuesForColumn(1);
            Debug.Assert(b.PossibleValuesForRow(1).Count == 5, "PossibleValuesForRow is wrong");
            Debug.Assert(b.PossibleValuesForColumn(3).Count == 9, "PossibleValuesForColumn is wrong");
            Debug.Assert(b.PossibleValuesFor(0, 2).Count == 4, "PossibleValuesFor is wrong");

            b.PrintBoard();
        }

    }
}
