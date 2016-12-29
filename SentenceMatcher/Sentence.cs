using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithms;

namespace SentenceMatcher
{
    class Sentence : Member
    {
        public string StringValue
        {
            get { return StringValue; }
            set
            {
                StringValue = value;
                if(IntArrayValue.Length == 0)
                    IntArrayValue = StringToIntArray(value);
            }
        }

        public int[] IntArrayValue {
            get { return IntArrayValue; }
            set
            {
                IntArrayValue = value;
                if(StringValue.Length == 0)
                    StringValue = IntArrayToString(value);
            } }

        public static int[] StringToIntArray(string sentence)
        {
            var letterArray = new int[sentence.Length];
            for (int i = 0; i < letterArray.Length; i++)
            {
                letterArray[i] = char.ToUpper(sentence[i]) - 64;
            }

            return letterArray;
        }

        public static string IntArrayToString(int[] intArray)
        {
            string sentence = "";
            for (int i = 0; i < intArray.Length; i++)
            {
                sentence += $"{(char)intArray[i]}";
            }

            return sentence;
        }
    }
}
