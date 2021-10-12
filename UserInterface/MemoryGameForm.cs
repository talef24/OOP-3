using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using B20_Ex02;
using Timer = System.Windows.Forms.Timer;

namespace UserInterface
{
    public class MemoryGameForm : Form
    {
        private const int k_CardSize = 90;
        private const int k_SpaceBetweenTwoControls = 10;
        private const int k_TotalSpaceBetweenAllLabels = k_SpaceBetweenTwoControls * 6;
        private const int k_FormMargin = 12;
        private const int k_TotalMarginOfTheForm = 3 * k_FormMargin;
        private const int k_NumberOfWrittenRowsBelowTheBoard = 3;
        private const int k_NumberOfCellsToChooseInOneTurn = 2;
        private const int k_IndexOfChosenCellRow = 0;
        private const int k_IndexOfChosenCellCol = 1;
        private const int k_IndexOfHeight = 0;
        private const int k_IndexOfWidth = 2;
        private const char k_MinimumValueOfTheNoneExposedValues = 'A';
        private const string k_URLOfImagesPool = "https://picsum.photos/80";

        private readonly Timer r_TimerSleep;
        private readonly Color r_FirstPlayerColor = Color.FromArgb(192, 255, 192);
        private readonly Color r_SecondPlayerColor = Color.FromArgb(191, 191, 255);
        private readonly Color r_CardColor = Color.LightGray;

        private PictureBox[,] m_GameBoard;
        private PictureBox[,] m_BoardContainingTheHiddenImages;

        private Label labelCurrentPlayer = new Label();
        private Label labelFirstPlayer = new Label();
        private Label labelSecondPlayer = new Label();

        private int m_SecondsSleep = 0;
        private eTurnChoosingStatus m_CountOfChoosingInTurn = eTurnChoosingStatus.NoChoosingYet;
        private string[] m_IndexesOfCellsChoosingInTurn = new string[k_NumberOfCellsToChooseInOneTurn];
        private MemoryGame m_LogicMemoryGame;

        public MemoryGameForm()
        {
            this.initializeGameSettings();
            this.setFormStyle();
            this.buildBoardOfNoneExposedImages();
            this.initializeGameBoard();
            this.initializeControls();
            r_TimerSleep = new Timer();
            r_TimerSleep.Interval = 1000;
            r_TimerSleep.Tick += m_TimerSleep_Tick;
        }

        private void initializeGameSettings()
        {
            GameSettingsForm settingsForm = new GameSettingsForm();
            settingsForm.ShowDialog(); 
            string firstPlayerName = settingsForm.FirstPlayerName;
            string secondPlayerName = settingsForm.SecondPlayerName;
            firstPlayerName = firstPlayerName.Equals(string.Empty) ? "First Player" : settingsForm.FirstPlayerName;
            secondPlayerName = secondPlayerName.Equals(string.Empty) ? "Second Player" : settingsForm.SecondPlayerName;
            int heightOfBoard = int.Parse(settingsForm.HeightAndWidthOfBoard[k_IndexOfHeight].ToString());
            int widthOfBoard = int.Parse(settingsForm.HeightAndWidthOfBoard[k_IndexOfWidth].ToString());

            this.m_LogicMemoryGame = new MemoryGame(
                firstPlayerName,
                settingsForm.IsAgainstAFriend,
                secondPlayerName,
                heightOfBoard,
                widthOfBoard);
            this.m_GameBoard = new PictureBox[m_LogicMemoryGame.GameBoard.Height, m_LogicMemoryGame.GameBoard.Width];
            this.m_BoardContainingTheHiddenImages = new PictureBox[m_LogicMemoryGame.GameBoard.Height, m_LogicMemoryGame.GameBoard.Width];
        }

        private void setFormStyle()
        {
            this.Text = "Memory Game";
            this.Font = new Font("Arial", 10);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.setHeightAndWidthOfForm();
        }

        private void setHeightAndWidthOfForm()
        {
            int numberOfCardsInOneRow = this.m_LogicMemoryGame.GameBoard.Width;
            int numberOfCardsInOneCol = this.m_LogicMemoryGame.GameBoard.Height;
            int widthOfAllCardsInForm = (numberOfCardsInOneRow * k_CardSize) + (k_SpaceBetweenTwoControls * (numberOfCardsInOneRow - 1));
            int heightOfAllCardsInForm = (numberOfCardsInOneCol * k_CardSize) + (k_SpaceBetweenTwoControls * (numberOfCardsInOneCol - 1));
            int heightOfAllLabelsInForm = (labelFirstPlayer.Height * k_NumberOfWrittenRowsBelowTheBoard) + k_TotalSpaceBetweenAllLabels;

            this.Width = widthOfAllCardsInForm + k_TotalMarginOfTheForm;
            this.Height = heightOfAllCardsInForm + heightOfAllLabelsInForm + k_TotalMarginOfTheForm;
        }

        private void buildBoardOfNoneExposedImages()
        {
            Dictionary<char, Image> dictionaryOfLettersAndImages = createDictionaryOfLettersAndImages();
            char currentCharToFindInTheDictionary = '\0';
            Image currentImageForInsertToTheBoard = null;

            for (int i = 0; i < m_LogicMemoryGame.GameBoard.Height; i++)
            {
                for (int j = 0; j < m_LogicMemoryGame.GameBoard.Width; j++)
                {
                    this.m_BoardContainingTheHiddenImages[i, j] = new PictureBox();
                    currentCharToFindInTheDictionary = (char)m_LogicMemoryGame.GameBoard.BoardContainingTheHiddenValues[i, j];
                    currentImageForInsertToTheBoard = dictionaryOfLettersAndImages[currentCharToFindInTheDictionary];
                    this.m_BoardContainingTheHiddenImages[i, j].Image = currentImageForInsertToTheBoard;
                }
            }
        }

        private Dictionary<char, Image> createDictionaryOfLettersAndImages()
        {
            int sizeOfDictionary = (m_LogicMemoryGame.GameBoard.Height * m_LogicMemoryGame.GameBoard.Width) / 2;
            Dictionary<char, Image> dictionaryOfLettersAndImages = new Dictionary<char, Image>();
            char currentLetterToAddToTheDictionary = k_MinimumValueOfTheNoneExposedValues;
            List<string> allStringsRepresentingImages = new List<string>();
            PictureBox currentImageToInsert = new PictureBox();
            string currentImageURI = string.Empty;

            for (int i = 0; i < sizeOfDictionary; i++)
            {
                currentImageURI = getRandomImageURI();
                while (checkIfStringOfImageIsAlreadyExist(allStringsRepresentingImages, currentImageURI) == true)
                {
                    currentImageURI = getRandomImageURI();
                }

                currentImageToInsert.Load(currentImageURI);
                dictionaryOfLettersAndImages.Add((char)currentLetterToAddToTheDictionary, currentImageToInsert.Image);
                allStringsRepresentingImages.Add(currentImageURI);
                currentLetterToAddToTheDictionary++;
            }

            return dictionaryOfLettersAndImages;
        }

        private bool checkIfStringOfImageIsAlreadyExist(List<string> i_ImagesStrings, string i_StringToCheckIfExist)
        {
            bool isExist = false;

            foreach (string currentStringInList in i_ImagesStrings)
            {
                if (i_StringToCheckIfExist.Equals(currentStringInList))
                {
                    isExist = true;
                }
            }

            return isExist;
        }

        private string getRandomImageURI()
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(k_URLOfImagesPool);
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            string currentImageURI = webResponse.ResponseUri.ToString();
            webResponse.Close();

            return currentImageURI;
        }

        private void initializeGameBoard()
        {
            int xOfCurrentCard;
            int yOfCurrentCard;

            for (int i = 0; i < m_LogicMemoryGame.GameBoard.Height; i++)
            {
                for (int j = 0; j < m_LogicMemoryGame.GameBoard.Width; j++)
                {
                    xOfCurrentCard = k_FormMargin + (j * (k_CardSize + k_SpaceBetweenTwoControls));
                    yOfCurrentCard = k_FormMargin + (i * (k_CardSize + k_SpaceBetweenTwoControls));
                    this.m_GameBoard[i, j] = new PictureBox();
                    this.m_GameBoard[i, j].Name = string.Format("{0}{1}", i, j);
                    this.m_GameBoard[i, j].Height = k_CardSize;
                    this.m_GameBoard[i, j].Width = k_CardSize;
                    this.m_GameBoard[i, j].BorderStyle = BorderStyle.FixedSingle;
                    this.m_GameBoard[i, j].BackColor = r_CardColor;
                    this.m_GameBoard[i, j].Location = new Point(xOfCurrentCard, yOfCurrentCard);
                    this.m_GameBoard[i, j].Cursor = Cursors.Hand;
                    this.m_GameBoard[i, j].Click += card_Click;
                    this.Controls.Add(m_GameBoard[i, j]);
                }
            }
        }

        private void initializeControls()
        {
            int lastRow = m_LogicMemoryGame.GameBoard.Height - 1;
            int gameBoardBottom = this.m_GameBoard[lastRow, 0].Bottom;
            updateCurrentPlayerSettings();
            this.labelCurrentPlayer.Left = k_FormMargin;
            this.labelCurrentPlayer.Top = gameBoardBottom + (k_SpaceBetweenTwoControls * 2);
            this.Controls.Add(labelCurrentPlayer);

            this.labelFirstPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.FirstPlayer);
            this.labelFirstPlayer.AutoSize = true;
            this.labelFirstPlayer.Left = k_FormMargin;
            this.labelFirstPlayer.Top = this.labelCurrentPlayer.Bottom + k_SpaceBetweenTwoControls;
            this.labelFirstPlayer.BackColor = r_FirstPlayerColor;
            this.Controls.Add(labelFirstPlayer);

            this.labelSecondPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.SecondPlayer);
            this.labelSecondPlayer.AutoSize = true;
            this.labelSecondPlayer.Left = k_FormMargin;
            this.labelSecondPlayer.Top = this.labelFirstPlayer.Bottom + k_SpaceBetweenTwoControls;
            this.labelSecondPlayer.BackColor = this.r_SecondPlayerColor;
            this.Controls.Add(labelSecondPlayer);
        }

        private void m_TimerSleep_Tick(object sender, EventArgs e)
        {
            const int k_MaxSleepSeconds = 1;
            bool isPair;

            if (m_SecondsSleep >= k_MaxSleepSeconds && this.m_CountOfChoosingInTurn.Equals(eTurnChoosingStatus.SecondChoosing))
            {
                isPair = this.m_LogicMemoryGame.CheckIfFoundPair(this.m_IndexesOfCellsChoosingInTurn);
                this.m_LogicMemoryGame.EndOfTurnAccordingToIfPairFoundOrNot(isPair);
                this.endOfTurn(isPair);
                r_TimerSleep.Stop();
                if (this.m_LogicMemoryGame.IsGameOver() == true)
                {
                    endOfRound();
                }

                if (this.m_LogicMemoryGame.CurrentPlayer.IsHuman == false)
                {
                    computerMove();
                }
            }
            else
            {
                m_SecondsSleep++;
            }
        }

        private void endOfTurn(bool i_IsPairFound)
        {
            this.m_CountOfChoosingInTurn = eTurnChoosingStatus.NoChoosingYet;

            if (i_IsPairFound == true)
            {
                labelFirstPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.FirstPlayer);
                labelSecondPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.SecondPlayer);
                labelFirstPlayer.Refresh();
                labelSecondPlayer.Refresh();
            }
            else
            {
                hideBothCellsThatWereChosenInTurn();
                updateCurrentPlayerSettings();
            }

            enableAllCardsThatAreNotExposedYet();
        }

        private void hideBothCellsThatWereChosenInTurn()
        {
            int row = 0;
            int col = 0;

            for (int i = 0; i < this.m_IndexesOfCellsChoosingInTurn.Length; i++)
            {
                row = int.Parse(this.m_IndexesOfCellsChoosingInTurn[i][k_IndexOfChosenCellRow].ToString());
                col = int.Parse(this.m_IndexesOfCellsChoosingInTurn[i][k_IndexOfChosenCellCol].ToString());
                this.m_GameBoard[row, col].BackgroundImage = null;
                this.m_GameBoard[row, col].BackColor = r_CardColor; 
            }

            this.m_LogicMemoryGame.GameBoard.HidePairOfCellsInGameBoard(this.m_IndexesOfCellsChoosingInTurn);
        }

        private void updateCurrentPlayerSettings()
        {
            bool isCurrentPlayerIsTheFirstPlayer = isFirstPlayerCurrentPlaying();

            labelCurrentPlayer.Text = string.Format("Current Player: {0}", m_LogicMemoryGame.CurrentPlayer.Name);
            labelCurrentPlayer.AutoSize = true;
            labelCurrentPlayer.BackColor = isCurrentPlayerIsTheFirstPlayer ? r_FirstPlayerColor : r_SecondPlayerColor;
            labelCurrentPlayer.Refresh();
        }

        private bool isFirstPlayerCurrentPlaying()
        {
            return m_LogicMemoryGame.CurrentPlayer.Equals(m_LogicMemoryGame.FirstPlayer);
        }

        private void enableAllCardsThatAreNotExposedYet()
        {
            for (int i = 0; i < m_LogicMemoryGame.GameBoard.Height; i++)
            {
                for (int j = 0; j < m_LogicMemoryGame.GameBoard.Width; j++)
                {
                    if (this.m_LogicMemoryGame.IsChosenCellIsExposed(i, j) == false)
                    {
                        this.m_GameBoard[i, j].Enabled = true;
                    }
                }
            }
        }

        private void card_Click(object sender, EventArgs e)
        {
            PictureBox theSender = sender as PictureBox;
            string rowAndColOfSenderInBoard = theSender.Name;
            theSender.Enabled = false;
            int rowInBoard = int.Parse(rowAndColOfSenderInBoard[k_IndexOfChosenCellRow].ToString());
            int colInBoard = int.Parse(rowAndColOfSenderInBoard[k_IndexOfChosenCellCol].ToString());
            exposeChosenCell(rowInBoard, colInBoard);
        }

        private void exposeChosenCell(int i_RowOfCellToExpose, int i_ColOfCellToExpose)
        {
            if ((int)this.m_CountOfChoosingInTurn < (int)eTurnChoosingStatus.SecondChoosing)
            {
                this.m_LogicMemoryGame.ExposeTheChosenCell(i_RowOfCellToExpose, i_ColOfCellToExpose);
                this.disableAllCardsInGameBoard();
                this.exposeImageOfChosenCell(i_RowOfCellToExpose, i_ColOfCellToExpose);
                string rowAndColInBoardAsString = m_GameBoard[i_RowOfCellToExpose, i_ColOfCellToExpose].Name;
                this.m_IndexesOfCellsChoosingInTurn[(int)this.m_CountOfChoosingInTurn] = rowAndColInBoardAsString; 
                this.m_CountOfChoosingInTurn++; 
                if (this.m_CountOfChoosingInTurn.Equals(eTurnChoosingStatus.SecondChoosing))
                {
                    m_SecondsSleep = 0;
                    r_TimerSleep.Start();
                }
                else
                {
                    this.enableAllCardsThatAreNotExposedYet();
                }
            }
        }

        private void disableAllCardsInGameBoard()
        {
            for (int i = 0; i < m_LogicMemoryGame.GameBoard.Height; i++)
            {
                for (int j = 0; j < m_LogicMemoryGame.GameBoard.Width; j++)
                {
                    this.m_GameBoard[i, j].Enabled = false;
                }
            }
        }

        private void exposeImageOfChosenCell(int i_RowOfCell, int i_ColOfCell)
        {
            Image currentImageToExpose = this.m_BoardContainingTheHiddenImages[i_RowOfCell, i_ColOfCell].Image;
            m_GameBoard[i_RowOfCell, i_ColOfCell].BackgroundImage = currentImageToExpose;
            m_GameBoard[i_RowOfCell, i_ColOfCell].BackgroundImageLayout = ImageLayout.Center;
            bool isCurrentPlayerIsTheFirstPlayer = m_LogicMemoryGame.CurrentPlayer.Equals(m_LogicMemoryGame.FirstPlayer);
            Color currentColor = isCurrentPlayerIsTheFirstPlayer ? r_FirstPlayerColor : r_SecondPlayerColor;
            m_GameBoard[i_RowOfCell, i_ColOfCell].BackColor = currentColor;
        }

        private void computerMove()
        {
            System.Threading.Thread.Sleep(2000);
            int[] computerChoosing;

            for (int i = 0; i < k_NumberOfCellsToChooseInOneTurn; i++)
            {
                computerChoosing = this.m_LogicMemoryGame.ComputerChoosingForNextCell();
                this.exposeChosenCell(computerChoosing[k_IndexOfChosenCellRow], computerChoosing[k_IndexOfChosenCellCol]);
            }
        }

        private string getTextForLabelNumOfPointsOfPlayer(Player i_Player)
        {
            string singleOrPlural = "(s)";

            if (i_Player.Points > 1)
            {
                singleOrPlural = "s";
            }

            return string.Format("{0}: {1} Pair{2}", i_Player.Name, i_Player.Points, singleOrPlural);
        }

        private void endOfRound()
        {
            string endRoundTitle = "Round Ended";
            string messageAnnouncingPointsAndWinner = getMessaageAnnouncingTheWinnerOfTheRound();
            MessageBoxButtons yesOrNoButton = MessageBoxButtons.YesNo;
            DialogResult rematchResult = MessageBox.Show(messageAnnouncingPointsAndWinner, endRoundTitle, yesOrNoButton);

            if (rematchResult == DialogResult.No)
            {
                MessageBox.Show("Game Over!");
                Environment.Exit(0);
            }
            else
            {
                this.roundInitialization();
            }
        }

        private string getMessaageAnnouncingTheWinnerOfTheRound()
        {
            string messageForTieOrSingleWinner = "The winner is: ";
            string winnerOfTheGame = m_LogicMemoryGame.GetNameOfTheWinner();
            string messageOfEndGame = string.Empty;

            if (winnerOfTheGame == null)
            {
                messageForTieOrSingleWinner = "There is a tie";
            }

            messageOfEndGame = string.Format(
                @"{0}: {1} - {2}: {3}
{4}{5} !
Would you like to play one more round?",
m_LogicMemoryGame.FirstPlayer.Name,
m_LogicMemoryGame.FirstPlayer.Points,
m_LogicMemoryGame.SecondPlayer.Name,
m_LogicMemoryGame.SecondPlayer.Points,
messageForTieOrSingleWinner,
winnerOfTheGame);

            return messageOfEndGame;
        }

        private void roundInitialization()
        {
            this.m_LogicMemoryGame.RoundInitialization(m_LogicMemoryGame.GameBoard.Height, m_LogicMemoryGame.GameBoard.Width);
            buildBoardOfNoneExposedImages();

            for (int i = 0; i < m_LogicMemoryGame.GameBoard.Height; i++)
            {
                for (int j = 0; j < m_LogicMemoryGame.GameBoard.Width; j++)
                {
                    this.m_GameBoard[i, j].BackgroundImage = null;
                    this.m_GameBoard[i, j].BackColor = r_CardColor;
                    this.m_GameBoard[i, j].Enabled = true;
                }
            }

            this.updateCurrentPlayerSettings();
            this.labelFirstPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.FirstPlayer);
            this.labelSecondPlayer.Text = getTextForLabelNumOfPointsOfPlayer(m_LogicMemoryGame.SecondPlayer);
            this.m_CountOfChoosingInTurn = eTurnChoosingStatus.NoChoosingYet;
        }
    }
}
