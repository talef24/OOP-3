using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace B20_Ex02
{
    public class GameBoard
    {
        private const char k_MinimunHightOrWidthOfTheGameBoardAsChar = '4';
        private const int k_MaximunHightOrWidthOfTheGameBoardAsChar = '6';
        private const char k_MinimumValueOfTheNoneExposedValues = 'A';
        private const int k_indexOfRowInString = 0;
        private const int k_indexOfColInString = 1;
        private readonly int r_Height;
        private readonly int r_Width;
        private char[,] m_BoardContainingTheHiddenValues;
        private char[,] m_GameBoard;

        public GameBoard(int i_Height, int i_Width)
        {
            this.r_Height = i_Height;
            this.r_Width = i_Width;
            this.m_BoardContainingTheHiddenValues = buildBoardOfNoneExposedValues();
            this.m_GameBoard = buildGameBoard();
        }

        public int Height
        {
            get
            {
                return this.r_Height;
            }
        }

        public int Width
        {
            get
            {
                return this.r_Width;
            }
        }

        public char[,] Board
        {
            get
            {
                return this.m_GameBoard;
            }
        }

        public char[,] BoardContainingTheHiddenValues
        {
            get { return this.m_BoardContainingTheHiddenValues; }
        }

        private char[,] buildGameBoard()
        {
            char[,] blankBoard = new char[this.r_Height, this.r_Width];
            return blankBoard;
        }

        private char[,] buildBoardOfNoneExposedValues()
        {
            char[,] boardOfNoneExposedValues = new char[this.r_Height, this.r_Width];
            List<char> poolOfLettersForFillingTheBoard = createPoolOfValuesForNoneExposedBoard();
            int currentNumOfCellsToFill = poolOfLettersForFillingTheBoard.Count;
            int randomIndexInPoolList = 0;
            Random random = new Random();

            for (int i = 0; i < r_Height; i++)
            {
                for (int j = 0; j < r_Width; j++)
                {
                    randomIndexInPoolList = random.Next(0, currentNumOfCellsToFill);
                    boardOfNoneExposedValues[i, j] = poolOfLettersForFillingTheBoard[randomIndexInPoolList];
                    poolOfLettersForFillingTheBoard.RemoveAt(randomIndexInPoolList);
                    currentNumOfCellsToFill = poolOfLettersForFillingTheBoard.Count;
                }
            }

            return boardOfNoneExposedValues;
        }

        private List<char> createPoolOfValuesForNoneExposedBoard()
        {
            List<char> poolOfValuesForNoneExposedBoard = new List<char>(this.r_Width * this.r_Height);
            char currentLetterToAddToThePool = k_MinimumValueOfTheNoneExposedValues;
            int sizeOfPool = poolOfValuesForNoneExposedBoard.Capacity;

            for (int i = 0; i < sizeOfPool / 2; i++)
            {
                poolOfValuesForNoneExposedBoard.Add((char)currentLetterToAddToThePool);
                poolOfValuesForNoneExposedBoard.Add((char)currentLetterToAddToThePool);
                currentLetterToAddToThePool++;
            }

            return poolOfValuesForNoneExposedBoard;
        }

        internal char GetHiddenValueOfCell(string i_RowAndColOfCellToExpose)
        {
            int rowOfCellToReturnItsValue = int.Parse(i_RowAndColOfCellToExpose[k_indexOfRowInString].ToString());
            int colOfCellToReturnItsValue = int.Parse(i_RowAndColOfCellToExpose[k_indexOfColInString].ToString());

            return this.m_BoardContainingTheHiddenValues[rowOfCellToReturnItsValue, colOfCellToReturnItsValue];
        }
      
        internal void ExposeCellInGameBoard(int i_RowOfCellToExpose, int i_ColOfCellToExpose)
        {
            char valueToExpose = this.m_BoardContainingTheHiddenValues[i_RowOfCellToExpose, i_ColOfCellToExpose];
            this.m_GameBoard[i_RowOfCellToExpose, i_ColOfCellToExpose] = valueToExpose;
        }

        public void HidePairOfCellsInGameBoard(string[] i_ArrOfIndexsOfCellsToHide)
        {
            int columnOfCurrentCellInArr = 0;
            int rowOfCurrentCellInArr = 0;

            for (int i = 0; i < i_ArrOfIndexsOfCellsToHide.Length; i++)
            {               
                rowOfCurrentCellInArr = int.Parse(i_ArrOfIndexsOfCellsToHide[i][0].ToString());
                columnOfCurrentCellInArr = int.Parse(i_ArrOfIndexsOfCellsToHide[i][1].ToString());
                this.m_GameBoard[rowOfCurrentCellInArr, columnOfCurrentCellInArr] = '\0';
            }
        }
    }
}
