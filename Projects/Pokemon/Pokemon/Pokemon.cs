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
        public int CurrentHealth { get; set; }
        public int Speed { get; private set; }
        public int Level { get; private set; } = 1;
        public int Experience { get; private set; } = 0;


        // random generator
        private static readonly Random random = new Random();

        // Constructors
        public Pokemon()
        {
            // generate random stats
            AttackPower = random.Next(1, 11);
            TotalHealth = random.Next(1, 11);
            CurrentHealth = TotalHealth;
            Speed = random.Next(1, 11);

            // generate random element
            Element = (Element)random.Next(1, 4);
        }

        public Pokemon(string name) : this()
        {
            Name = name;
        }

        // Levels & Experience
        public void GainExperience(int experience)
        {
            Experience += experience;

            // 100 experience equals to one level
            while(Experience >= 100)
            {
                LevelUp();
                Experience -= 100;
            }
        }

        private void LevelUp()
        {
            Level++;
            AttackPower += random.Next(1, 6);
            TotalHealth += random.Next(1, 6);
        }

        // Attacks
        public void TakeDamage(Pokemon enemy)
        {
            int damage = enemy.AttackDamage(this);
            CurrentHealth -= damage;

            // console output
            Console.ForegroundColor = enemy.GetConsoleColor();
            Console.Write($"{enemy.Name}");
            Console.ResetColor();
            Console.Write($" dealt {damage} damage to");
            Console.ForegroundColor = GetConsoleColor();
            Console.Write($" {Name}.");
            Console.ResetColor();

            if (CurrentHealth <= 0)
            {
                Console.ForegroundColor = GetConsoleColor();
                Console.Write($" {Name}");
                Console.ResetColor();
                Console.WriteLine(" is defeated!");
            }
            else
            {
                Console.ForegroundColor = GetConsoleColor();
                Console.Write($"\n{Name}");
                Console.ResetColor();
                Console.WriteLine($" has currently {CurrentHealth} HP.");
            }
        }

        public int AttackDamage(Pokemon enemy)
        {
            if((Element == Element.Fire && enemy.Element == Element.Grass)
                || (Element == Element.Water && enemy.Element == Element.Fire)
                || (Element == Element.Grass && enemy.Element == Element.Water))
            {
                return AttackPower * 2;
            }

            return AttackPower;
        }

        // Info
        public void ShowInfo()
        {
            Console.ForegroundColor = GetConsoleColor();
            Console.Write($"{Name}");
            Console.ResetColor();
            Console.WriteLine($": {AttackPower} Attack, {CurrentHealth} HP, {Speed} Speed");
        }

        public ConsoleColor GetConsoleColor()
        {
            return Element switch
            {
                Element.Fire => ConsoleColor.Red,
                Element.Water => ConsoleColor.Blue,
                Element.Grass => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }

    }
}
