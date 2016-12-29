using GeneticAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceMatcher
{
    public class SentenceMatcher
    {
        private int[] _sentenceAsInts;
        private GeneticStrategies<Sentence> _strategies;
        private Population<Sentence> _population; 
        
        public SentenceMatcher(string sentence)
        {
            _sentenceAsInts = Sentence.StringToIntArray(sentence);
            _strategies = new GeneticStrategies<Sentence>(
                propInfo =>
                {
                    Random rand = new Random();
                    return rand.Next(1, 26);
                },
                phrase =>
                {
                    var difference = 0;
                    var maxPossibleDifference = 26*26;
                    for (int i = 0; i < phrase.IntArrayValue.Length; i ++)
                    {
                        difference += Math.Abs(_sentenceAsInts[i] - phrase.IntArrayValue[i]);
                    }

                    return (float)(maxPossibleDifference - difference)/maxPossibleDifference;
                },
                attribute =>
                {
                    Random rand = new Random();
                    return attribute + (rand.Next(0, 7)/10);
                });
        }


        public void Start()
        {
            _population = new Population<Sentence>(_strategies, 10);

            RunLoop();
        }

        private void RunLoop()
        {

            do
            {
                _population.CreateNextGeneration(10);
                _population.KillOffLowFitnessMembers(.10);
                Console.WriteLine(_population.Members.Aggregate((m1, m2) => m1.Fitness > m2.Fitness ? m1 : m2).StringValue);
            } while (!_population.Members.Select(x => x.IntArrayValue).Equals(_sentenceAsInts));
        }
    }
}
