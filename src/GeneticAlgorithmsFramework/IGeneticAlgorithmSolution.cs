namespace GeneticAlgorithmsFramework;

/// <summary>
/// Interface, that shall be implemented by types,
/// which models the solutions of genetic algorithm.
/// </summary>
/// <remarks>
/// Mechanisms related to fitness score were not taken into account
/// during definition of this interface, as algorithm runner do not use it directly.
/// Mechanism of ranking the solutions shall be implemented
/// by derivative type and taken into account in type derivative from algorithm runner.
/// </remarks>
public interface IGeneticAlgorithmSolution
{
    #region Methods
    /// <summary>
    /// Implements a mutation mechanism, which randomly modifies the solution.
    /// </summary>
    /// <remarks>
    /// This method shall modify properties of an object, on which it was called.
    /// </remarks>
    void Mutate();

    /// <summary>
    /// Combines genomes of solution with other, provided one to create a new descendant solutions.
    /// </summary>
    /// <param name="otherSolution">
    /// Other parent-solution, which shall be used to perform the operation.
    /// </param>
    /// <returns>
    /// Descendant solutions, which are the result of performed operation.
    /// </returns>
    public abstract IEnumerable<IGeneticAlgorithmSolution> CombineGenomesWith(IGeneticAlgorithmSolution otherSolution);
    #endregion
}
