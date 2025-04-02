using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Pokemon
{
    public class Pokemon
    {
        public string Name { get; init; } = "Unknown";
        public Element Element { get; init; }
        public int AttackPower { get; private set; }
        public int TotalHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Speed { get; private set; }
        public int Level { get; private set; } = Constants.StartingLevel;
        public int Experience { get; private set; } = Constants.StartingExperience;


        // random generator
        private static readonly Random random = new Random();

        // Constructors
        public Pokemon()
        {
            // generate random stats
            AttackPower = random.Next(Constants.MinAttackPower, Constants.MaxAttackPower);
            TotalHealth = random.Next(Constants.MinHealth, Constants.MaxHealt);
            CurrentHealth = TotalHealth;
            Speed = random.Next(Constants.MinSpeed, Constants.MaxSpeed);

            // generate random element
            Element = Element.RandomElement();
        }

        public Pokemon(string name) : this()
        {
            Name = name;
        }

        // Levels & Experience
        public void GainExperience(int experience)
        {
            Experience += experience;

            while (Experience >= Constants.ExperienceInLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            Experience -= Constants.ExperienceInLevel;
            AttackPower += random.Next(Constants.LevelUpAttackPowerMinGain, Constants.LevelUpAttackPowerMaxGain);
            TotalHealth += random.Next(Constants.LevelUpHealthMinGain, Constants.LevelUpHealthMaxGain);
        }

        // Attacks
        public void TakeDamage(Pokemon enemy)
        {
            int damage = enemy.AttackDamage(this);
            CurrentHealth -= damage;

            // example: Maritide dealt 2 damage to Photosprout.
            enemy.PrintName();
            Console.Write($" dealt {damage} damage to ");
            PrintName();
            Console.Write(". ");


            if (CurrentHealth <= 0)
            {
                // example: Photosprout is defeated!
                PrintName();
                Console.WriteLine(" is defeated!");
            }
            else
            {
                Console.WriteLine();
                PrintName();
                Console.WriteLine($" has currently {CurrentHealth} HP.");
            }
        }

        public int AttackDamage(Pokemon enemy)
        {
            if ((Element == Element.Fire && enemy.Element == Element.Grass)
                || (Element == Element.Water && enemy.Element == Element.Fire)
                || (Element == Element.Grass && enemy.Element == Element.Water))
            {
                return AttackPower * Constants.CritDamageModifier;
            }

            return AttackPower;
        }

        // Info
        public void ShowInfo()
        {
            PrintName();
            Console.WriteLine($": {AttackPower} Attack, {CurrentHealth} HP, {Speed} Speed");
        }

        public void ShowInfoPlayer()
        {
            PrintName();
            Console.WriteLine($": {AttackPower} Attack, {CurrentHealth} HP, {Speed} Speed, level {Level}, {Experience}/{Constants.ExperienceInLevel} XP");
        }

        private ConsoleColor GetConsoleColor()
        {
            return Element switch
            {
                Element.Fire => ConsoleColor.Red,
                Element.Water => ConsoleColor.Blue,
                Element.Grass => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }

        public void PrintName()
        {
            Console.ForegroundColor = GetConsoleColor();
            Console.Write(Name);
            Console.ResetColor();

        }
    }
}
