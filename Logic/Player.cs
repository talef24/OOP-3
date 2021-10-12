using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B20_Ex02
{
    public class Player
    {
        private readonly bool r_IsHuman;
        private readonly string r_PlayerName;
        private int m_GainedPoints;

        internal Player(string i_PlayerName)
        {
            this.r_IsHuman = true;
            this.r_PlayerName = i_PlayerName;
            this.m_GainedPoints = 0;
        }

        public Player(string i_PlayerName, bool i_IsHuman)
        {
            if (i_IsHuman == false)
            {
                this.r_PlayerName = "Computer";
            }
            else
            {
                this.r_PlayerName = i_PlayerName;
            }

            r_IsHuman = i_IsHuman;
            this.m_GainedPoints = 0;
        }

        internal void UpdatePoints()
        {
            this.m_GainedPoints++;
        }

        public string Name
        {
            get
            {
                return this.r_PlayerName;
            }
        }

        public int Points
        {
            get
            {
                return this.m_GainedPoints;
            }

            set
            {
                this.m_GainedPoints = value;
            }
        }

        public bool IsHuman
        {
            get
            {
                return this.r_IsHuman;
            }
        }
    }
}
