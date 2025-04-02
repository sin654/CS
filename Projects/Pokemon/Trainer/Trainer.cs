using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Trainer
{
    public abstract class Trainer
    {
        public string Name { get; set; } = "Unknown";

        protected List<Pokemon.Pokemon> Pokemons = new List<Pokemon.Pokemon>();

        public Trainer(string name)
        {
            Name = name;
        }

        public abstract void Info();

        public void ChoosePokemon(Pokemon.Pokemon pokemon)
        {
            if(Pokemons.Contains(pokemon))
            {
                Console.WriteLine("You already have this pokemons!");
                return;
            }

            if(Pokemons.Count == 3)
            {
                Console.WriteLine("You already have 3 pokemons!");
                return;
            }

            Pokemons.Add(pokemon);
        }

        public Queue<Pokemon.Pokemon> GetPokemons()
        {
            return new Queue<Pokemon.Pokemon>(Pokemons);
        }
    }
}
