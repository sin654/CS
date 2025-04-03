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

            Console.WriteLine();
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

            Console.WriteLine();
        }

        private void FightCommand()
        {
            Trainer.Enemy currentEnemy;

            switch (player.BattlesWon)
            {
                case 0:
                    currentEnemy = firstEnemy;
                    break;
                case 1:
                    currentEnemy = secondEnemy;

                    break;
                case 2:
                    currentEnemy = thirdEnemy;
                    break;
                default:
                    currentEnemy = null;
                    break;
            }

            if(currentEnemy != null)
            {
                TrainerBattle(player, currentEnemy);
            }
            else
            {
                Console.WriteLine("All enemies have been defeated.");
            }

            Console.WriteLine();
        }

        private void InfoCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            player.Info();
            Console.WriteLine();
        }

        private void SortCommand()
        {
            if (!player.PokemonsSelected())
            {
                GameNotStarted();
                return;
            }

            player.SortPokemons();
            Console.WriteLine();
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
        public Pokemon.Pokemon PokemonBattle(Pokemon.Pokemon playerPokemon, Pokemon.Pokemon enemyPokemon)
        {
            playerPokemon.PrintName();
            Console.Write(" vs ");
            enemyPokemon.PrintName();
            Console.WriteLine();

            Pokemon.Pokemon winner = playerPokemon;

            // find the faster pokemon (or the player if they have the same speed)
            Pokemon.Pokemon first;
            Pokemon.Pokemon second;
            if(playerPokemon.Speed >= enemyPokemon.Speed)
            {
                first = playerPokemon;
                second = enemyPokemon;
            }
            else
            {
                first = enemyPokemon;
                second = playerPokemon;
            }

            // pokemons will take turns attacking
            while(playerPokemon.CurrentHealth > 0 && enemyPokemon.CurrentHealth > 0)
            {
                // first pokemon attacks
                second.TakeDamage(first);
                if(second.CurrentHealth <= 0)
                {
                    winner = first;
                    break;
                }

                // second pokemon attacks
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
            // create Queues for Player and his current enemy
            Queue<Pokemon.Pokemon> playerPokemons = player.GetPokemons();
            Queue<Pokemon.Pokemon> enemyPokemons = enemy.GetPokemons();

            // then peek from each queue and initiate a duel between pokemons, the looser pokemon is dequeued
            for (int round = 1; playerPokemons.Count > 0 && enemyPokemons.Count > 0; round++)
            {
                Pokemon.Pokemon playerPokemon = playerPokemons.Peek();
                Pokemon.Pokemon enemyPokemon = enemyPokemons.Peek();

                Console.Write($"Round {round}: ");
                Pokemon.Pokemon winner = PokemonBattle(playerPokemon, enemyPokemon);
                if (winner == playerPokemon)
                {
                    enemyPokemons.Dequeue();
                }
                else
                {
                    playerPokemons.Dequeue();
                }
            }

            // fight until one of the queues is empty => empty queue means the trainer has lost
            if(playerPokemons.Count > 0)
            {
                player.WinBattle();
                enemy.BattleFinished();
                return player;
            }
            else
            {
                player.LoseBattle();
                enemy.BattleFinished();
                return enemy;
            }
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
