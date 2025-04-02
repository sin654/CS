using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pokemon.Pokemon;

namespace Pokemon
{
    static class Constants
    {
        // Base stats
        public const int MinAttackPower = 1;
        public const int MaxAttackPower = 11; // used in random generator as upper bound (not inclusive)

        public const int MinSpeed = 1;
        public const int MaxSpeed = 11; // used in random generator as upper bound (not inclusive)

        public const int MinHealth = 1;
        public const int MaxHealt = 11; // used in random generator as upper bound (not inclusive)
        public const int StartingLevel = 1;
        public const int StartingExperience = 0;

        // Attack
        public const int CritDamageModifier = 2;

        // Levels
        public const int ExperienceInLevel = 100; // experience needed to level up
        public const int LevelUpAttackPowerMinGain = 1;
        public const int LevelUpAttackPowerMaxGain = 6;
        public const int LevelUpHealthMinGain = 1;
        public const int LevelUpHealthMaxGain = 6;

        // Names
        public const string PlayerName = "John";
        public const string EnemyName1 = "Baltazar";
        public const string EnemyName2 = "Horridon";
        public const string EnemyName3 = "King Kong";

        private static List<string> PokemonNames = new List<string>
        {
            "Pikachu",
            "Charmander",
            "Bulbasaur",
            "Squirtle",
            "Supa",
            "Mer",
            "Bisu",
            "Rek",
            "Tailin",
            "Kroko",
            "Barb",
            "Kro",
            "Tar",
            "Beril",
            "Api",
            "Tris",
            "Rabi"
        };

        public static string GetRandomPokemonName()
        {
            Random random = new Random();
            string name = PokemonNames[random.Next(0, PokemonNames.Count)];
            PokemonNames.Remove(name);
            return name;
        }
    }
}
