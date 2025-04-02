using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Trainer
{
    public class Enemy : Trainer
    {
        public Enemy(string name) : base(name) 
        {
            ChoosePokemon(new Pokemon.Pokemon(Constants.GetRandomPokemonName()));
            ChoosePokemon(new Pokemon.Pokemon(Constants.GetRandomPokemonName()));
            ChoosePokemon(new Pokemon.Pokemon(Constants.GetRandomPokemonName()));
        }

        public override void Info()
        {
            Console.WriteLine("The next trainer has these pokemons:");
            foreach (var pokemon in Pokemons)
            {
                pokemon.ShowInfo();
            }
        }
    }
}
