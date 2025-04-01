using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Game
{
    class Game : IGame, IBattle
    {
        public Pokemon.Pokemon BattleRound(Pokemon.Pokemon playerFImon, Pokemon.Pokemon enemyFImon)
        {
            Console.WriteLine();
            Console.WriteLine($"{playerFImon.Name} vs {enemyFImon.Name}");

            Pokemon.Pokemon winner = playerFImon;

            // find the faster FImon (or the player if they have the same speed)
            Pokemon.Pokemon first;
            Pokemon.Pokemon second;
            if(playerFImon.Speed >= enemyFImon.Speed)
            {
                first = playerFImon;
                second = enemyFImon;
            }
            else
            {
                first = enemyFImon;
                second = playerFImon;
            }

            // pokemons will take turns attacking
            while(playerFImon.CurrentHealth > 0 && enemyFImon.CurrentHealth > 0)
            {
                // first FImon attacks
                second.TakeDamage(first);
                if(second.CurrentHealth <= 0)
                {
                    winner = first;
                    break;
                }

                // second FImon attacks
                first.TakeDamage(second);
                if (first.CurrentHealth <= 0)
                {
                    winner = second;
                    break;
                }
            }

            return winner;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
