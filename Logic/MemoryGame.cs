using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B20_Ex02
{
    public class MemoryGame
    {
        private const int k_NumberOfCellsToChooseInOneTurn = 2;
        private const int k_IndexOfRowInArrContainingRowAndCol = 0;
        private const int k_IndexOfColInArrContainingRowAndCol = 1;
        private const int k_IndexOfFirstCell = 0;
        private const int k_IndexOfSecondCell = 1;
        private const int k_NumOfIndexedRepresentingOneCell = 2;
        private readonly Player r_FirstPlayer;
        private readonly Player r_SecondPlayer;
        private GameBoard m_Board;
        private Player m_PlayerPlayingCurrentTurn;
        private Random m_Random = new Random();

        public MemoryGame(
            string i_NameOfFirstPlayer,
            bool i_IsHuman,
            string i_NameOfSecondPlayer,
            int i_HeightOfBoard,
            int i_WidthOfBoard)
        {
            this.r_FirstPlayer = new Player(i_NameOfFirstPlayer);
            this.r_SecondPlayer = new Player(i_NameOfSecondPlayer, i_IsHuman);
            RoundInitialization(i_HeightOfBoard, i_WidthOfBoard);
        }

        public void RoundInitialization(int i_HeightOfBoard, int i_WidthOfBoard)
        {
            this.m_Board = new GameBoard(i_HeightOfBoard, i_WidthOfBoard);
            r_FirstPlayer.Points = 0;
            r_SecondPlayer.Points = 0;
            this.m_PlayerPlayingCurrentTurn = r_FirstPlayer;
        }

        public Player FirstPlayer
        {
            get
            {
                return this.r_FirstPlayer;
            }
        }

        public Player SecondPlayer
        {
            get
            {
                return this.r_SecondPlayer;
            }
        }

        public Player CurrentPlayer
        {
            get
            {
                return this.m_PlayerPlayingCurrentTurn;
            }
        }

        public GameBoard GameBoard
        {
            get
            {
                return this.m_Board;
            }
        }
        
        public void ExposeTheChosenCell(int i_RowOfCellToExpose, int i_ColOfCellToExpose)
        {
            m_Board.ExposeCellInGameBoard(i_RowOfCellToExpose, i_ColOfCellToExpose);
        }
        
        public bool CheckIfFoundPair(string[] i_IndexesOfBothChosenCells)
        {
            char[] valuesOfChosenCells = new char[k_NumberOfCellsToChooseInOneTurn];
            bool isPair = true;
            char firstChosenCellHiddenvalue = m_Board.GetHiddenValueOfCell(i_IndexesOfBothChosenCells[k_IndexOfFirstCell]);
            char secondChosenCellHiddenvalue = m_Board.GetHiddenValueOfCell(i_IndexesOfBothChosenCells[k_IndexOfSecondCell]);

            valuesOfChosenCells[k_IndexOfFirstCell] = firstChosenCellHiddenvalue;
            valuesOfChosenCells[k_IndexOfSecondCell] = secondChosenCellHiddenvalue;
            if (valuesOfChosenCells[k_IndexOfFirstCell] != valuesOfChosenCells[k_IndexOfSecondCell])
            {
                isPair = false;
            }

            return isPair;
        }

        public void EndOfTurnAccordingToIfPairFoundOrNot(bool i_IsPair)
        {
            if(IsGameOver() == false)
            {
                if (i_IsPair == true)
                {
                    m_PlayerPlayingCurrentTurn.UpdatePoints();
                }
                else
                {
                    m_PlayerPlayingCurrentTurn = m_PlayerPlayingCurrentTurn.Equals(r_FirstPlayer) ? r_SecondPlayer : r_FirstPlayer;
                }
            }
        }

        public int[] ComputerChoosingForNextCell()
        {
            int[] computerChoosing = new int[k_NumOfIndexedRepresentingOneCell];
            int numberForRow = m_Random.Next(0, m_Board.Height);
            int numberForColumn = m_Random.Next(0, m_Board.Width);
            
            while (IsChosenCellIsExposed(numberForRow, numberForColumn) == true)
            {
                numberForRow = m_Random.Next(0, m_Board.Height);
                numberForColumn = m_Random.Next(0, m_Board.Width);
            }

            computerChoosing[k_IndexOfRowInArrContainingRowAndCol] = numberForRow;
            computerChoosing[k_IndexOfColInArrContainingRowAndCol] = numberForColumn;

            return computerChoosing;
        }

        public bool IsChosenCellIsExposed(int i_RowOfChosenCellToCheckIfExposed, int i_ColOfChosenCellToCheckIfExposed)
        {
            bool isCellIsExposed = false;

            if (m_Board.Board[i_RowOfChosenCellToCheckIfExposed, i_ColOfChosenCellToCheckIfExposed] != '\0')
            {
                isCellIsExposed = true;
            }

            return isCellIsExposed;
        }

        public string GetNameOfTheWinner()
        {
            string winnerOfTheGame = null;

            if (r_FirstPlayer.Points > r_SecondPlayer.Points)
            {
                winnerOfTheGame = r_FirstPlayer.Name;
            }
            else if (r_FirstPlayer.Points < r_SecondPlayer.Points)
            {
                winnerOfTheGame = r_SecondPlayer.Name;
            }

            return winnerOfTheGame;
        }

        public bool IsGameOver()
        {
            int numberOfPairsInBoard = (this.m_Board.Height * this.m_Board.Width) / 2;

            // Each pair worth a point, so the total points at the end of the game should be as the number of pairs.
            return FirstPlayer.Points + SecondPlayer.Points == numberOfPairsInBoard;
        }
    }
}
