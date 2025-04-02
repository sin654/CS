using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Trainer
{
    public class Player : Trainer
    {
        public Player(string name) : base(name) { }
        public int BattlesWon { get; private set; } = 0;

        // random generator
        private static readonly Random random = new Random();

        public bool PokemonsSelected()
        {
            return Pokemons.Count == 3;
        }

        public void UnselectPokemons()
        {
            Pokemons.Clear();
        }

        public override void Info()
        {
            Console.WriteLine($"Battles won: {BattlesWon}");
            Console.WriteLine("Your pokemons:");
            foreach (var pokemon in Pokemons)
            {
                pokemon.ShowInfoPlayer();
            }
        }

        public void SortPokemons()
        {
            void FormatError()
            {
                Console.WriteLine("ERROR: Invalid input format. Expected are 3 integer numbers from range of selection. (\"1 2 3\")");
            }

            Console.WriteLine("Choose the order:");
            for (int i = 0; i < Pokemons.Count; i++)
            {
                Console.Write($"{i + 1}: ");
                Pokemons[i].PrintName();
                Console.WriteLine();
            }

            // handle input
            string input = Console.ReadLine();
            string[] indexes = input.Split(' ');

            if(indexes.Length == 3)
            {
                if(int.TryParse(indexes[0], out int first) && int.TryParse(indexes[1], out int second) && int.TryParse(indexes[2], out int third))
                {
                    if (first >= 1 && first <= Pokemons.Count && second >= 1 && second <= Pokemons.Count && third >= 1 && third <= Pokemons.Count)
                    {
                        List<Pokemon.Pokemon> newOrder = new List<Pokemon.Pokemon>();
                        newOrder.Add(Pokemons[first - 1]);
                        newOrder.Add(Pokemons[second - 1]);
                        newOrder.Add(Pokemons[third - 1]);
                        Pokemons = newOrder;
                        Console.WriteLine("The order of your pokemons has been updated.");
                    }
                    else
                    {
                        FormatError();
                    }
                }
                else
                {
                    FormatError();
                }
            }
            else
            {
                FormatError();
            }
        }

        public void PrintPokemonSelection()
        {
            Console.Write("You have chosen ");
            for (int i = 0; i < Pokemons.Count(); i++)
            {
                if (i == 1)
                {
                    Console.Write(", ");
                }
                else if (i == 2)
                {
                    Console.Write(" and ");
                }

                Pokemons[i].PrintName();
            }
            Console.WriteLine();
        }

        // Battles and rounds
        public void WinBattle()
        {
            BattlesWon++;
            int expGain = random.Next(Constants.ExperienceForWinMin, Constants.ExperienceForWinMax);

            // get XP to pokemons
            bool leveledUp = false;
            foreach (Pokemon.Pokemon pokemon in Pokemons)
            {
                if(pokemon.GainExperience(expGain))
                {
                    leveledUp = true;
                }
            }

            if (leveledUp)
            {
                Console.WriteLine($"You won the battle! Pokemons gained {expGain} XP! Your pokemons leveled up!");
            }
            else
            {
                Console.WriteLine($"You won the battle! Pokemons gained {expGain} XP!");
            }

            // reset pokemons health for next battle
            foreach (Pokemon.Pokemon pokemon in Pokemons)
            {
                pokemon.ResetHealth();
            }
        }

        public void LoseBattle()
        {
            int expGain = random.Next(Constants.ExperienceForLooseMin, Constants.ExperienceForLooseMax);

            // get XP to pokemons
            bool leveledUp = false;
            foreach (Pokemon.Pokemon pokemon in Pokemons)
            {
                if (pokemon.GainExperience(expGain))
                {
                    leveledUp = true;
                }
            }

            if (leveledUp)
            {
                Console.WriteLine($"You lost the battle! Pokemons gained {expGain} XP! Your pokemons leveled up!");
            }
            else
            {
                Console.WriteLine($"You lost the battle! Pokemons gained {expGain} XP!");
            }

            // reset pokemons health for next battle
            foreach (Pokemon.Pokemon pokemon in Pokemons)
            {
                pokemon.ResetHealth();
            }
        }
    }
}
