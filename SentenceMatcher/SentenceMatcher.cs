using GeneticAlgorithms;
using System;
using System.Linq;

namespace SentenceMatcher
{
    public class SentenceMatcher
    {
        private readonly int[] _sentenceAsFloats;
        private readonly GeneticStrategies<Sentence> _strategies;
        private PopulationManager<Sentence> _populationManager;
        private readonly Random _rand = new Random();

        public SentenceMatcher(string sentence)
        {
            _sentenceAsFloats = StringToIntArray(sentence);
            _strategies = new GeneticStrategies<Sentence>(new FitnessStrategy<Sentence>(
                phrase =>
                {
                    var difference = 0d;
                    var weight = 1.1;
                    var maxPossibleDifference = 13*weight*phrase.IntArrayValue.Length;
                    Func<int, int, int, int> distanceFunc = (correctVal, givenVal, maxVal) =>
                    {
                        if (correctVal == givenVal)
                            return 0;

                        int i = givenVal;
                        int j = correctVal;
                        int rightDistance = 0;
                        int leftDistance = 0;
                        while ((++i - 1) % maxVal + 1 != correctVal)
                        {
                            rightDistance++;
                        }

                        while ((++j - 1) % maxVal + 1 != givenVal)
                        {
                            leftDistance++;
                        }

                        return (leftDistance < rightDistance ? leftDistance : rightDistance) + 1;
                    };
                    for (int i = 0; i < phrase.IntArrayValue.Length; i ++)
                    {
                        var actual = distanceFunc.Invoke(phrase.IntArrayValue[i], _sentenceAsFloats[i], 26);
                        difference += actual * weight;
                    }

                    return maxPossibleDifference - difference;
                }));
            _strategies.GenerationStrategies.Add(new GenerationStrategy<Sentence, int[]>(propInfo =>
            {
                var array = new int[sentence.Length];
                for (int i = 0; i < sentence.Length; i++)
                {
                    array[i] = _rand.Next(1, 26);
                }
                return array;
            }, "FloatArrayValue"));
            _strategies.MutationStrategies.Add(new MutationStrategy<Sentence, int[]>(originalValue =>
            {
                for (int i = 0; i < originalValue.Length; i++)
                {
                    originalValue[i] +=
                        (_rand.Next(0, 10) == 1 ? 0 : 1) *
                        (_rand.Next(0, 2) * 2 - 1 == 1 ? 1 : -1)
                        * _rand.Next(0, 2);

                    if (originalValue[i] < 1)
                        originalValue[i] += 26;
                    if (originalValue[i] > 26)
                        originalValue[i] -= 26;
                }

                return originalValue;
            }));
            _strategies.ReproductionStrategies.Add(new ReproductionStrategy<Sentence, int[]>((p1, p2) =>
            {
                int[] result = new int[p1.Length];
                for (int i = 0; i < p1.Length; i++)
                {
                    result[i] = (p1[i] + p2[i]) / 2;
                }
                return result;
            }));
        }

        public void Start()
        {
            _populationManager = new PopulationManager<Sentence>(_strategies);
            _populationManager.InitializePopulationMembers(30);
            RunLoop();
        }

        private void RunLoop()
        {
            do
            {
                _populationManager.ProgressOneGeneration(.2, .2);
                var fittestMember = GetFittestMember();
                Console.Write("\r{0}", IntArrayToString(fittestMember.IntArrayValue) + " => " + fittestMember.Fitness);
            } while (!ArraysAreEqual(GetFittestMember().IntArrayValue, _sentenceAsFloats));
                Console.WriteLine("\n" +
                                  $"Generation : {GetFittestMember().Generation}");
        }

        private bool ArraysAreEqual(int[] array1, int[] array2)
        {
            if (array1.Count() != array2.Count())
                return false;

            for (int i = 0; i < array1.Count(); i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }

        private Sentence GetFittestMember()
        {
            return _populationManager.Population.Members.Aggregate((m1, m2) => m1.Fitness > m2.Fitness ? m1 : m2);
        }

        public int[] StringToIntArray(string sentence)
        {
            var letterArray = new int[sentence.Length];
            for (int i = 0; i < letterArray.Length; i++)
            {
                var letter = char.ToLower(sentence[i]);
                letterArray[i] = letter == ' ' ? 26 : char.ToLower(sentence[i]) - 96;
            }

            return letterArray;
        }

        public string IntArrayToString(int[] floatArray)
        {
            return floatArray.Aggregate("", (current, t) => $"{current}{(char)(t + 96)}");
        }
    }
}
