using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

namespace UserInterface
{
    public class GameSettingsForm : Form
    {        
        private const int k_FormMargin = 12;
        private const int k_SpaceBetweenTwoControls = 10;
        private const int k_MinSizeBoard = 4;
        private const int k_MaxSizeBoard = 6;
        private const string k_TextShownWhenSecondPlayerIsComputer = "- computer -";
        private readonly List<string> m_OptionalSizesOfGameBoard = new List<string>();

        private readonly Label r_LabelFirstPlayerName = new Label();
        private readonly Label r_LabelSecondPlayerName = new Label();
        private readonly Label r_LabelBoardSize = new Label();
        private TextBox textBoxFirstPlayerName = new TextBox();
        private TextBox textBoxSecondPlayerName = new TextBox();
        private Button buttonAgainstAFriendOrComputer = new Button();
        private Button buttonBoardSize = new Button();
        private Button buttonStart = new Button();

        private int m_CurrentIndexOfOptionalBoardSize = 0;
        private string m_NameOfFirstPlayer;
        private string m_NameOfSecondPlayer;
        private string m_HeightAndWidthOfBoard;
        private bool m_IsAgainstFriend;

        internal GameSettingsForm()
        {
            this.setFormStyle();
            this.initializeArrayOfOptionalBoardSizes();
            this.initializeControls();
        }

        private void setFormStyle()
        {
            this.Text = "Memory Game - Settings";
            this.Font = new Font("Arial", 10);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 420;
            this.Height = 240;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        internal string FirstPlayerName
        {
            get { return this.m_NameOfFirstPlayer; }
        }

        internal string SecondPlayerName
        {
            get { return this.m_NameOfSecondPlayer; }
        }

        internal bool IsAgainstAFriend
        {
            get { return this.m_IsAgainstFriend; }
        }

        internal string HeightAndWidthOfBoard
        {
            get { return this.m_HeightAndWidthOfBoard; }
        }

        private void initializeArrayOfOptionalBoardSizes()
        {
            for (int i = k_MinSizeBoard; i <= k_MaxSizeBoard; i++)
            {
                for (int j = k_MinSizeBoard; j <= k_MaxSizeBoard; j++)
                {
                    if ((i * j) % 2 == 0)
                    {
                        this.m_OptionalSizesOfGameBoard.Add(string.Format("{0}x{1}", i, j));
                    }
                }
            }
        }

        private void initializeControls()
        {
            this.initializeLabelControls();
            this.initializeTextBoxControls();
            this.initializeButtonControls();
        }

        private void initializeLabelControls()
        {
            this.r_LabelFirstPlayerName.Text = "First Player Name:";
            this.r_LabelFirstPlayerName.AutoSize = true;
            this.r_LabelFirstPlayerName.Left = k_FormMargin;
            this.r_LabelFirstPlayerName.Top = k_FormMargin;
            this.Controls.Add(r_LabelFirstPlayerName);

            this.r_LabelSecondPlayerName.Text = "Second Player Name:";
            this.r_LabelSecondPlayerName.AutoSize = true;
            this.r_LabelSecondPlayerName.Left = k_FormMargin;
            this.r_LabelSecondPlayerName.Top = k_SpaceBetweenTwoControls + r_LabelFirstPlayerName.Bottom;
            this.Controls.Add(r_LabelSecondPlayerName);

            this.r_LabelBoardSize.Text = "Board Size:";
            this.r_LabelBoardSize.Left = k_FormMargin;
            this.r_LabelBoardSize.Top = (2 * k_SpaceBetweenTwoControls) + r_LabelSecondPlayerName.Bottom;
            this.Controls.Add(r_LabelBoardSize);
        }

        private void initializeTextBoxControls()
        {
            this.textBoxFirstPlayerName.Left = k_SpaceBetweenTwoControls + r_LabelSecondPlayerName.Right;
            this.textBoxFirstPlayerName.Top = k_FormMargin;
            this.textBoxFirstPlayerName.MaxLength = this.textBoxFirstPlayerName.Width;
            this.Controls.Add(textBoxFirstPlayerName);

            this.textBoxSecondPlayerName.Text = k_TextShownWhenSecondPlayerIsComputer;
            this.textBoxSecondPlayerName.Left = textBoxFirstPlayerName.Left;
            this.textBoxSecondPlayerName.Top = r_LabelSecondPlayerName.Top;
            this.textBoxSecondPlayerName.Enabled = false;
            this.textBoxSecondPlayerName.MaxLength = this.textBoxSecondPlayerName.Width;
            this.Controls.Add(textBoxSecondPlayerName);
        }

        private void initializeButtonControls()
        {
            this.buttonAgainstAFriendOrComputer.Text = "Against a Friend";
            this.buttonAgainstAFriendOrComputer.Left = k_SpaceBetweenTwoControls + textBoxSecondPlayerName.Right;
            this.buttonAgainstAFriendOrComputer.Top = r_LabelSecondPlayerName.Top;
            this.buttonAgainstAFriendOrComputer.AutoSize = true;
            this.buttonAgainstAFriendOrComputer.Cursor = Cursors.Hand;
            this.Controls.Add(buttonAgainstAFriendOrComputer);
            this.buttonAgainstAFriendOrComputer.Click += buttonAgainstAFriendOrComputer_Click;

            this.buttonBoardSize.Text = this.m_OptionalSizesOfGameBoard[0];
            this.buttonBoardSize.Width = 120;
            this.buttonBoardSize.Height = 80;
            this.buttonBoardSize.Left = k_FormMargin;
            this.buttonBoardSize.Top = r_LabelBoardSize.Bottom;
            this.buttonBoardSize.BackColor = Color.FromArgb(191, 191, 255);
            this.buttonBoardSize.Cursor = Cursors.Hand;
            this.Controls.Add(buttonBoardSize);
            this.buttonBoardSize.Click += buttonBoardSize_Click;

            this.buttonStart.Text = "Start!";
            this.buttonStart.Left = buttonAgainstAFriendOrComputer.Right - buttonStart.Width;
            this.buttonStart.Top = buttonBoardSize.Bottom - buttonStart.Height;
            this.buttonStart.BackColor = Color.FromArgb(0, 192, 0);
            this.buttonStart.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(buttonStart);
            buttonStart.Click += buttonStart_Click;
        }

        private void buttonBoardSize_Click(object sender, EventArgs e)
        {
            Button theSender = sender as Button;
            m_CurrentIndexOfOptionalBoardSize++;
            m_CurrentIndexOfOptionalBoardSize = m_CurrentIndexOfOptionalBoardSize % m_OptionalSizesOfGameBoard.Count;
            theSender.Text = m_OptionalSizesOfGameBoard[m_CurrentIndexOfOptionalBoardSize];
        }

        private void buttonAgainstAFriendOrComputer_Click(object sender, EventArgs e)
        {
            Button theSender = sender as Button;

            if (this.textBoxSecondPlayerName.Enabled == false)
            {
                this.textBoxSecondPlayerName.Text = string.Empty;
                theSender.Text = "Against Computer";
                this.m_IsAgainstFriend = true;
            }
            else
            {
                this.textBoxSecondPlayerName.Text = k_TextShownWhenSecondPlayerIsComputer;
                theSender.Text = "Against a Friend";
                this.m_IsAgainstFriend = false;
            }

            this.textBoxSecondPlayerName.Enabled = !this.textBoxSecondPlayerName.Enabled;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.updateGameSettings();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.updateGameSettings();
        }

        private void updateGameSettings()
        {
            this.m_NameOfFirstPlayer = textBoxFirstPlayerName.Text;
            this.m_IsAgainstFriend = textBoxSecondPlayerName.Enabled;
            this.m_NameOfSecondPlayer = textBoxSecondPlayerName.Text;
            this.m_HeightAndWidthOfBoard = buttonBoardSize.Text;
        }
    }
}
