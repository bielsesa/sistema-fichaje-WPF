using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFichaje
{
    public class ComboBoxItem
    {
        public string DisplayText { get; set; }
        public int Value { get; set; }

        public ComboBoxItem(string text, int value)
        {
            DisplayText = text;
            Value = value;
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
