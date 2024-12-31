using ColorMatchingAlgorithm;
using GeneticAlgorithmsFramework;

#region Configuration
const int PopulationSize = 1000;
const double MutationProbability = 0.1;
const int GenerationLimit = 100;
#endregion

#region Methods
void ProgressCallback(GeneticAlgorithmRunner geneticAlgorithmRunner)
{
    #region Arguments validation
    if (geneticAlgorithmRunner is null)
    {
        string argumentName = nameof(geneticAlgorithmRunner);
        string errorMessage = $"Provided algorithm runner is a null reference: {geneticAlgorithmRunner}";
        throw new ArgumentNullException(argumentName, errorMessage);
    }

    if (geneticAlgorithmRunner is not ColorMatchingAlgorithmRunner)
    {
        string argumentName = nameof(geneticAlgorithmRunner);
        const string ErrorMessage = "Provided algorithm runner is not related to color matching algorithm:";
        throw new ArgumentException(ErrorMessage, argumentName);
    }
    #endregion

    var colorMatchingAlgorithmRunner = (ColorMatchingAlgorithmRunner)geneticAlgorithmRunner;

    if (colorMatchingAlgorithmRunner.CurrentGeneration % 10 == 0)
    {
        Console.WriteLine($"Generation: {colorMatchingAlgorithmRunner.CurrentGeneration}, Best solution: {colorMatchingAlgorithmRunner.BestSolution}");
    }
}
#endregion

#region Main method
var algorithmRunner = new ColorMatchingAlgorithmRunner(PopulationSize, MutationProbability, GenerationLimit);

Console.WriteLine($"Reference solution: {algorithmRunner.ReferenceSolution}");

Console.WriteLine();

Console.WriteLine($"Launching genetic algorithm...");
algorithmRunner.Run(ProgressCallback);
Console.WriteLine($"End of algorithm runtime reached...");

Console.WriteLine();

Console.WriteLine($"Reference solution: {algorithmRunner.ReferenceSolution}");
Console.WriteLine($"Solution found by algorithm: {algorithmRunner.BestSolution}");

Console.ReadLine();
#endregion