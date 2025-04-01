

namespace Pokemon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Pokemon.Pokemon> playerPokemons = new List<Pokemon.Pokemon>();

            Pokemon.Pokemon Pikachu = new Pokemon.Pokemon("Pikachu");
            playerPokemons.Add(Pikachu);
            Pokemon.Pokemon Charmander = new Pokemon.Pokemon("Charmander");
            playerPokemons.Add(Charmander);
            Pokemon.Pokemon Bulbasaur = new Pokemon.Pokemon("Bulbasaur");
            playerPokemons.Add(Bulbasaur);
            Pokemon.Pokemon Squirtle = new Pokemon.Pokemon("Squirtle");
            playerPokemons.Add(Squirtle);
            Pokemon.Pokemon Supa = new Pokemon.Pokemon("Supa");
            playerPokemons.Add(Supa);
            Pokemon.Pokemon Mer = new Pokemon.Pokemon("Mer");
            playerPokemons.Add(Mer);

            Console.WriteLine("Welcome to FImon Championship! Please, choose your three FImons:");
            for (int i = 0; i < playerPokemons.Count; i++)
            {
                Console.Write($"{i+1} ");
                playerPokemons[i].ShowInfo();
            }




            /* TODO remove
            Game.Game game = new Game.Game();
            game.BattleRound(Pikachu, Charmander);
            */
        }
    }
}
