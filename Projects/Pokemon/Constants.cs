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
        /*
         * Value, which have MIN & MAX in name, are used in random generator as upper bound (not inclusive)
         * */

        // Base stats
        public const int MinAttackPower = 1;
        public const int MaxAttackPower = 11;

        public const int MinSpeed = 1;
        public const int MaxSpeed = 11;

        public const int MinHealth = 1;
        public const int MaxHealt = 11;
        public const int StartingLevel = 1;
        public const int StartingExperience = 0;

        // Attack
        public const int CritDamageModifier = 2;

        // Levels
        public const int ExperienceInLevel = 100; // experience needed to level up

        public const int ExperienceForWinMin = 50;
        public const int ExperienceForWinMax = 151;
        public const int ExperienceForLooseMin = 10;
        public const int ExperienceForLooseMax = 71;

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
            "Pikachu", "Bulbasaur", "Charmander", "Squirtle", "Eevee",
            "Snorlax", "Mewtwo", "Gengar", "Jigglypuff", "Meowth",
            "Psyduck", "Machamp", "Lapras", "Magikarp", "Vulpix",
            "Growlithe", "Abra", "Alakazam", "Slowpoke", "Pidgey",
            "Zubat", "Onix", "Geodude", "Rattata", "Caterpie",
            "Weedle", "Nidoran", "Clefairy", "Sandshrew", "Diglett",
            "Poliwag", "Tentacool", "Magnemite", "Doduo", "Seel",
            "Grimer", "Shellder", "Gastly", "Krabby", "Exeggcute",
            "Cubone", "Koffing", "Rhyhorn", "Horsea", "Staryu",
            "Scyther", "Pinsir", "Tauros", "Dratini", "Jynx"
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
