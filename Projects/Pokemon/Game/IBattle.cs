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
        public Trainer.Trainer TrainerBattle(Trainer.Player player, Trainer.Enemy enemy);

        /// <summary>
        /// Performs one round of a battle between two pokemons.
        /// </summary>
        /// <param name="playerPokemon">Player trainer's pokemon</param>
        /// <param name="enemyPokemon">Enemy trainer's pokemon</param>
        /// <returns>Winner pokemon</returns>
        public Pokemon.Pokemon PokemonBattle(Pokemon.Pokemon playerPokemon, Pokemon.Pokemon enemyPokemon);
    }
}
