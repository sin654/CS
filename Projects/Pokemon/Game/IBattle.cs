using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Game
{
    public interface IBattle
    {
        /// <summary>
        /// Performs a battle between two trainers.
        /// </summary>
        /// <param name="player">Player trainer</param>
        /// <param name="enemy">Enemy trainer</param>
        /// <returns>Winner trainer</returns>
        //TODO:

        /// <summary>
        /// Performs one round of a battle between two FImons.
        /// </summary>
        /// <param name="playerFImon">Player trainer's FImon</param>
        /// <param name="enemyFImon">Enemy trainer's FImon</param>
        /// <returns>Winner FImon</returns>
        public Pokemon.Pokemon BattleRound(Pokemon.Pokemon playerFImon, Pokemon.Pokemon enemyFImon);
    }
}
