namespace GeneticAlgorithmsFramework;

/// <summary>
/// Abstract runner, which implements core mechanisms of genetic algorithms.
/// Runners, which implements a specific algorithm shall derivate from this class.
/// </summary>
/// <remarks>
/// Class was designed to be both robust and flexible but to work correctly and be secure
/// derivative class shall follow below guidelines:
///     1. Keep in mind that this runner supports only a variant of generic algorithms, where size of population is fixed.
///     2. Derivative class shall implement all abstract methods defined within this class according to their doc-strings.
///     3. It is not recommended to directly expose to public an internally stored collection of ISolution-type objects, which represents the population of solutions
///         or any ISolution-type member of it as it violates data security - collection or its members (keep in mind, that ISolution type is mutable)
///         could be edited externally, which may cause unexpected behavior of the runner.
///         When exposure is needed, the derivative class shall convert particular ISolution-type object to its raw, primitive-type-based representation
///         (ex. by object serialization or exposing one of its primitive-type property related to its raw value) or create its deep copy and then expose it.
/// </remarks>
public abstract class GeneticAlgorithmRunner
{
    #region Properties
    private IGeneticAlgorithmSolution[] _populationOfSolutions;
    
    protected readonly Random _randomNumberGenerator;

    public readonly int PopulationSize;
    public readonly double MutationProbability;

    protected virtual IEnumerable<IGeneticAlgorithmSolution> PopulationOfSolutions
    {
        get
        {
            return _populationOfSolutions;
        }
        private set
        {
            if (value.Count() != PopulationSize)
            {
                string argumentName = nameof(value);
                string errorMessage = $"Invalid size of provided population of solutions - shall be equal to {PopulationSize}: {value.Count()}";
                throw new ArgumentException(errorMessage, argumentName);
            }

            // Using IEnumerable<T>.ToArray() to make sure, that collection is instantiated (is not ex. LINQ query).
            _populationOfSolutions = value.ToArray();
        }
    }
    
    public int CurrentGeneration
    {
        get;
        private set;
    }
    #endregion

    #region Instantiation
    /// <summary>
    /// Initializes core functionalities of algorithm runner.
    /// </summary>
    /// <param name="populationSize">
    /// Number of solutions, which creates one generation.
    /// </param>
    /// <param name="mutationProbability">
    /// Probability of mutation, which can occur whenever two solutions combines their genomes to create a new one.
    /// Shall range between 0.0 (inclusive) and 1.0 (inclusive).
    /// </param>
    /// <param name="seedForRandomNumberGenerator">
    /// Value of seed, using which internal pseudo-random number generator shall be initialized.
    /// When null value will be specified, seed will be chosen automatically.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    protected GeneticAlgorithmRunner(int populationSize, double mutationProbability, int? seedForRandomNumberGenerator)
    {
        #region Arguments validation
        if (populationSize < 2)
        {
            string argumentName = nameof(populationSize);
            string errorMessage = $"Invalid population size -  shall be greater that 1: {populationSize}";
            throw new ArgumentOutOfRangeException(argumentName, populationSize, errorMessage);
        }

        if (mutationProbability < 0.0 || 1.0 < mutationProbability)
        {
            string argumentName = nameof(mutationProbability);
            string errorMessage = $"Invalid mutation probability -  shall be in range <0;1>: {mutationProbability}";
            throw new ArgumentOutOfRangeException(argumentName, mutationProbability, errorMessage);
        }
        #endregion

        if (seedForRandomNumberGenerator.HasValue)
        {
            _randomNumberGenerator ??= new Random(seedForRandomNumberGenerator.Value);
        }
        else
        {
            _randomNumberGenerator ??= new Random();
        }

        PopulationSize = populationSize;
        CurrentGeneration = 0;
        MutationProbability = mutationProbability;

        _populationOfSolutions = Array.Empty<IGeneticAlgorithmSolution>();

        PopulationOfSolutions = PrepareInitialPopulation();
        
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
    protected abstract IEnumerable<IGeneticAlgorithmSolution> PrepareInitialPopulation();
    #endregion

    #region Interactions
    /// <summary>
    /// Creates a new generation of solutions by crossing and mutating genomes of solutions selected from a current population.
    /// </summary>
    /// <returns>
    /// Solutions, which are descendants of solutions contained by current population.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown, when internal data is not consistent.
    /// </exception>
    private IGeneticAlgorithmSolution[] CreateNewGeneration()
    {
        var newPopulation = new List<IGeneticAlgorithmSolution>();

        while (newPopulation.Count() < PopulationSize)
        {
            var (mother, father) = PickParents();

            // Picked parents are being validated as parents selection is implemented externally.
            if (!PopulationOfSolutions.Contains(mother) || !PopulationOfSolutions.Contains(father))
            {
                const string ErrorMessage = "At least one picked parent does not belong to a current population: ";
                throw new InvalidOperationException(ErrorMessage);
            }

            IEnumerable<IGeneticAlgorithmSolution> children = mother.CombineGenomesWith(father);

            foreach (IGeneticAlgorithmSolution child in children)
            {
                if (_randomNumberGenerator.NextDouble() < MutationProbability)
                {
                    child.Mutate();
                }
            }

            newPopulation.AddRange(children);
        }

        // Newly created population can be larger than assumed population size.
        // IEnumerable<T>.Take() will limit its size to valid range.
        return newPopulation.Take(PopulationSize).ToArray();
    }

    /// <summary>
    /// Checks if stopping condition of algorithm is fulfilled.
    /// </summary>
    /// <remarks>
    /// Shall be implemented in derivative class according to adopted stopping condition.
    /// Check is performed before each new generation of solutions is generated.
    /// </remarks>
    /// <returns>
    /// True if stopping condition of is fulfilled and algorithm shall be stopped, false otherwise.
    /// </returns>
    protected abstract bool ShallAlgorithmStop();

    /// <summary>
    /// Selects parent solutions for a new descendant solution from a current population.
    /// </summary>
    /// <remarks>
    /// Shall be implemented in derivative class according to adopted mechanism of parent solutions selection.
    /// </remarks>
    /// <returns>
    /// Solutions, which genomes shall be crossed to create a new descendant solution.
    /// Both picked solutions shall be contained by current population.
    /// </returns>
    protected abstract (IGeneticAlgorithmSolution, IGeneticAlgorithmSolution) PickParents();

    /// <summary>
    /// Triggers runtime of the algorithm.
    /// </summary>
    /// <param name="newGenerationCallback">
    /// Action, which shall be called every time a new generation of solutions is generated.
    /// Is optional.
    /// </param>
    public void Run(Action<GeneticAlgorithmRunner>? newGenerationCallback = null)
    {
        while (!ShallAlgorithmStop())
        {
            PopulationOfSolutions = CreateNewGeneration();
            CurrentGeneration++;

            if (newGenerationCallback is not null)
            {
                newGenerationCallback(this);
            }
        }
    }
    #endregion
}
