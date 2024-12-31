using GeneticAlgorithmsFramework;

namespace ColorMatchingAlgorithm;

/// <summary>
/// Runner, which implements color matching as genetic algorithm.
/// </summary>
public sealed class ColorMatchingAlgorithmRunner : GeneticAlgorithmRunner
{
    #region Properties
    private readonly ColorMatchingAlgorithmSolution _referenceSolution;

    public readonly int GenerationLimit;

    public new IEnumerable<ColorMatchingAlgorithmSolution> PopulationOfSolutions
    {
        get
        {
            return base.PopulationOfSolutions.Cast<ColorMatchingAlgorithmSolution>().ToArray();
        }
    }
    public string ReferenceSolution
    {
        get
        {
            return _referenceSolution.ToString();
        }
    }
    public string BestSolution
    {
        get
        {
            return PopulationOfSolutions
                .OrderBy(solution => solution.ComputeFitnessScore(_referenceSolution))
                .Last()
                .ToString();
        }
    }
    #endregion

    #region Instantiation

    /// <summary>
    /// Instantiates color matching algorithm runner.
    /// </summary>
    /// <param name="populationSize">
    /// Number of solutions, which creates one generation.
    /// </param>
    /// <param name="mutationProbability">
    /// Probability of mutation, which can occur whenever two solutions combines their genomes to create a new one.
    /// Shall range between 0.0 (inclusive) and 1.0 (inclusive).
    /// </param>
    /// <param name="generationLimit">
    /// Number of generations, after passing which algorithm shall stop its runtime.
    /// </param>
    /// <param name="seedForRandomNumberGenerator">
    /// Value of seed, using which internal pseudo-random number generator shall be initialized.
    /// Specifying a seed is optional - when not specified seed will be chosen automatically.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    public ColorMatchingAlgorithmRunner(int populationSize, double mutationProbability, int generationLimit, int? seedForRandomNumberGenerator = null)
        : base(populationSize, mutationProbability, seedForRandomNumberGenerator)
    {
        #region Argument validation
        if (generationLimit < 1)
        {
            string argumentName = nameof(generationLimit);
            string errorMessage = $"Invalid generation limit specified - shall be greater than 1: {generationLimit}";
            throw new ArgumentOutOfRangeException(argumentName, generationLimit, errorMessage);
        }
        #endregion

        _referenceSolution = SelectReferenceSolution();

        GenerationLimit = generationLimit;
    }

    /// <summary>
    /// Prepares a collection of solutions, which will be used in evolution process as first generation.
    /// </summary>
    /// <remarks>
    /// Shall be implemented in derivative class according to adopted initialization method.
    /// </remarks>
    /// <returns>
    /// Valid solutions, which shall be used in evolution process as first generation.
    /// </returns>
    protected override IEnumerable<IGeneticAlgorithmSolution> PrepareInitialPopulation()
    {
        return Enumerable.Range(0, PopulationSize)
            .Select(_ => ColorMatchingAlgorithmSolution.RandomColor(_randomNumberGenerator))
            .ToArray();
    }

    /// <summary>
    /// Selects the color, shall be taken as solution of the algorithm.
    /// Algorithm, by itself is not aware of its value but computes fitness score of generated solutions basing on it.
    /// </summary>
    /// <remarks>
    /// Currently method returns randomly generated color.
    /// </remarks>
    /// <returns>
    /// Color representation, which shall be taken as algorithm solution.
    /// </returns>
    private ColorMatchingAlgorithmSolution SelectReferenceSolution()
    {
        return ColorMatchingAlgorithmSolution.RandomColor(_randomNumberGenerator);
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Picks one random member from current population of solutions.
    /// Probability of choosing a particular solution is directly proportional to its fitness score.  
    /// </summary>
    /// <returns>
    /// One randomly chosen member from current population of solutions.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown, when process is interrupted with internal error.
    /// </exception>
    /// TODO: Optimize to not compute fitness score two times per solution.
    private ColorMatchingAlgorithmSolution PickParent()
    {
        int totalFitnessScore = PopulationOfSolutions.Sum(solution => solution.ComputeFitnessScore(_referenceSolution));

        int randomValue = _randomNumberGenerator.Next(totalFitnessScore);

        foreach (ColorMatchingAlgorithmSolution solution in PopulationOfSolutions)
        {
            randomValue -= solution.ComputeFitnessScore(_referenceSolution);

            if (randomValue < 0)
            {
                return solution;
            }
        }

        const string ErrorMessage = "Internal error during parent picking:";
        throw new InvalidOperationException(ErrorMessage);
    }

    /// <summary>
    /// Selects parent solutions for a new descendant solution from a current population.
    /// </summary>
    /// <returns>
    /// Solutions, which genomes shall be crossed to create a new descendant solution.
    /// </returns>
    protected override (IGeneticAlgorithmSolution, IGeneticAlgorithmSolution) PickParents()
    {
        ColorMatchingAlgorithmSolution mother = PickParent();
        ColorMatchingAlgorithmSolution father = PickParent();

        // Protection to not return the same solution both as mother and father.
        while (ReferenceEquals(mother, father))
        {
            father = PickParent();
        }

        return (mother, father);
    }

    /// <summary>
    /// Checks if stopping condition of algorithm is fulfilled.
    /// </summary>
    /// <remarks>
    /// Currently algorithm is set to end its runtime after passing specified number of generations.
    /// </remarks>
    /// <returns>
    /// True if stopping condition of is fulfilled and algorithm shall be stopped, false otherwise.
    /// </returns>
    protected override bool ShallAlgorithmStop()
    {
        return CurrentGeneration == GenerationLimit;
    }
    #endregion
}
