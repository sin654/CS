using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Pokemon
{
    public enum Element
    {
        Fire,
        Water,
        Grass
    }

    public static class ElementExtensions
    {
        public static Element RandomElement(this Element element)
        {
            var values = Enum.GetValues(typeof(Element));
            return (Element)values.GetValue(Random.Shared.Next(values.Length));
        }
    }
}
