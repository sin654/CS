using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Pokemon.Trainer;

namespace Pokemon.Game
{
    class Game : IGame, IBattle
    {
        private List<Pokemon.Pokemon> playerPokemons = new List<Pokemon.Pokemon>();
        private Trainer.Player player = new Trainer.Player(Constants.PlayerName);
        private Trainer.Enemy firstEnemy = new Trainer.Enemy(Constants.EnemyName1);
        private Trainer.Enemy secondEnemy = new Trainer.Enemy(Constants.EnemyName2);
        private Trainer.Enemy thirdEnemy = new Trainer.Enemy(Constants.EnemyName3);

        public Game()
        {
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
            CreatePlayerPokemon(Constants.GetRandomPokemonName());
        }


        // Input and commands
        private void HandleInput(string input)
        {
            string[] inputArray = input.Split(' ');
            string command = inputArray[0].ToLower();

            switch (command)
            {
                case "start":
                    StartCommand(inputArray);
                    break;
                case "check":
                    CheckCommand();
                    break;
                case "fight":
                    FightCommand();
                    break;
                case "info":
                    InfoCommand();
                    break;
                case "sort":
                    SortCommand();
                    break;
                case "quit":
                    QuitCommand();
                    break;
                default:
                    Console.WriteLine("Invalid command!");
                    break;
            }
        }

        private void StartCommand(string[] input)
        {
            void FormatError()
            {
                Console.WriteLine("ERROR: Invalid start command format. Expected is \"start x y z\", where x,z,y are positive integers in range of pokemon selection!");
            }

            // validate input
            if (input.Length == 4)
            {
                if(int.TryParse(input[1], out int first) && int.TryParse(input[2], out int second) && int.TryParse(input[3], out int third))
                {
                    if (first < 1 || first > playerPokemons.Count || second < 1 || second > playerPokemons.Count || third < 1 || third > playerPokemons.Count)
                    {
                        FormatError();
                    }
                    else
                    {
                        player.ChoosePokemon(playerPokemons[first - 1]);
                        player.ChoosePokemon(playerPokemons[second - 1]);
                        player.ChoosePokemon(playerPokemons[third - 1]);
                        player.PrintPokemonSelection();
                        Console.WriteLine("Your commands are: check, fight, info, sort, quit.");
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

        private void GameNotStarted()
        {
            Console.WriteLine("ERROR: Game has not started yet. Please use the start command to start the game.");
        }

        private void CheckCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            if (player.BattlesWon == 0)
            {
                firstEnemy.Info();
            }
            else if(player.BattlesWon == 1)
            {
                secondEnemy.Info();
            }
            else if (player.BattlesWon == 2)
            {
                thirdEnemy.Info();
            }
            else
            {
                Console.WriteLine("All enemies have been defeated.");
            }
        }

        private void FightCommand()
        {
            // TODO
        }

        private void InfoCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            player.Info();
        }

        private void SortCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            player.SortPokemons();
        }

        private void QuitCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            Console.WriteLine("You have lost the game :(");
            player.UnselectPokemons();
        }


        private void CreatePlayerPokemon(string name)
        {
            Pokemon.Pokemon pokemon = new Pokemon.Pokemon(name);
            playerPokemons.Add(pokemon);
        }

        // IBattle interface
        public Pokemon.Pokemon PokemonBattle(Pokemon.Pokemon playerFImon, Pokemon.Pokemon enemyFImon)
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

        public Trainer.Trainer TrainerBattle(Player player, Enemy enemy)
        {
            throw new NotImplementedException();
            // TODO
        }

        // IGame interface
        public void Start()
        {
            Console.WriteLine("Welcome to pokemon Championship! Please, choose your three pokemons:");
            for (int i = 0; i < playerPokemons.Count; i++)
            {
                Console.Write($"{i + 1} ");
                playerPokemons[i].ShowInfo();
            }

            while (true)
            {
                HandleInput(Console.ReadLine());
            }
        }
    }
}
