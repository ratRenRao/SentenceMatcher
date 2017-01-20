using GeneticAlgorithms;

namespace SentenceMatcher
{
    internal class Sentence : Member
    {
        public int[] IntArrayValue { get; set; }

        public Sentence(int[] intArrayValue)
        {
            IntArrayValue = intArrayValue;
        }

        public Sentence()
        {
        }
    }
}
