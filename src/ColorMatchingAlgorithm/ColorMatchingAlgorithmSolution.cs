using GeneticAlgorithmsFramework;

namespace ColorMatchingAlgorithm;

/// <summary>
/// Representation of color matching algorithm solution, which is in fact representation of a color,
/// that supports operations related to genetic algorithms.
/// </summary>
public sealed class ColorMatchingAlgorithmSolution: IGeneticAlgorithmSolution
{
    #region Constants
    public const int NumberOfChannels = 4;      // Color is represented by 4 data channels - alpha, red, green and blue. 
    public const int BitsPerChannel = 8;        // Value of each channel is represented by one byte - range <0;255>.
    public const int MinChannelValue = 0;
    #endregion

    #region Static properties
    private static int? s_maxChannelValue;

    public static int MaxChannelValue
    {
        get
        {
            return s_maxChannelValue ??= Convert.ToInt32(Math.Pow(2.0, BitsPerChannel) - 1);
        }
    }
    #endregion

    #region Properties
    private readonly Random _randomNumberGenerator;
    private readonly int[] _channelsValues;
    #endregion

    #region Instantiation
    /// <summary>
    /// Generates a representation of randomly generated color.
    /// </summary>
    /// <param name="randomNumberGenerator">
    /// Reference to random number generator, which shall be used internally by created object
    /// and during its generation.
    /// </param>
    /// <returns>
    /// New, randomly generated representation of a color.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one argument is a null reference.
    /// </exception>
    public static ColorMatchingAlgorithmSolution RandomColor(Random randomNumberGenerator)
    {
        #region Arguments validation
        if (randomNumberGenerator is null)
        {
            string argumentName = nameof(randomNumberGenerator);
            string errorMessage = $"Provided random number generator is a null reference: {randomNumberGenerator}";
            throw new ArgumentNullException(argumentName, errorMessage);
        }
        #endregion

        int[] randomChannelValues = Enumerable.Range(0, NumberOfChannels)
            .Select(_ => randomNumberGenerator.Next(MinChannelValue, MaxChannelValue + 1))  // Generation range is exclusive at the top.
            .ToArray();

        return new ColorMatchingAlgorithmSolution(randomChannelValues, randomNumberGenerator);
    }

    /// <summary>
    /// Instantiates a new color representation.
    /// </summary>
    /// <param name="channelsValues">
    /// Collection contain values of each data channel.
    /// </param>
    /// <param name="randomNumberGenerator">
    /// Reference to random number generator, which shall be used internally by instantiated object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, when value of at least one argument will be considered as invalid.
    /// </exception>
    private ColorMatchingAlgorithmSolution(IEnumerable<int> channelsValues, Random randomNumberGenerator)
    {
        #region Arguments validation
        if (randomNumberGenerator is null)
        {
            string argumentName = nameof(randomNumberGenerator);
            string errorMessage = $"Provided random number generator is a null reference: {randomNumberGenerator}";
            throw new ArgumentNullException(argumentName, errorMessage);
        }

        if (channelsValues is null)
        {
            string argumentName = nameof(channelsValues);
            string errorMessage = $"Provided collection of channels values is a null reference: {channelsValues}";
            throw new ArgumentNullException(argumentName, errorMessage);
        }

        if (channelsValues.Count() != NumberOfChannels)
        {
            string argumentName = nameof(channelsValues);
            string rrrorMessage = $"Invalid number of provided channels - shall be equal to {NumberOfChannels}: {channelsValues.Count()}";
            throw new ArgumentException(rrrorMessage, argumentName);
        }

        foreach (int channelValue in channelsValues)
        {
            if (channelValue < MinChannelValue || MaxChannelValue < channelValue)
            {
                string argumentName = nameof(channelsValues);
                string errorMessage = $"Invalid channel value specified - shall be in range <{MinChannelValue};{MaxChannelValue}>: {channelValue}";
                throw new ArgumentOutOfRangeException(argumentName, channelValue, errorMessage);
            }
        }
        #endregion

        _randomNumberGenerator = randomNumberGenerator;
        _channelsValues = channelsValues.ToArray();
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Computes fitness score of this solution by taking into account provided reference.
    /// </summary>
    /// <param name="referenceColor">
    /// Color, which shall be used as reference during fitness score computation.
    /// </param>
    /// <returns>
    /// Value of computed fitness score.
    /// The higher the value of fitness score, the more this color is similar to the reference.
    /// </returns>
    public int ComputeFitnessScore(ColorMatchingAlgorithmSolution referenceColor)
    {
        int deviationFromReference = 0;

        for (int channelIndex = 0; channelIndex < NumberOfChannels; channelIndex++)
        {
            int channelValue = _channelsValues[channelIndex];
            int referenceChannelValue = referenceColor._channelsValues[channelIndex];
            
            int channelDeviation = Math.Abs(referenceChannelValue - channelValue);
            deviationFromReference += channelDeviation;
        }

        int maxFitnessScore = MaxChannelValue * NumberOfChannels;
        return maxFitnessScore - deviationFromReference;

    }

    /// <summary>
    /// Combines properties of this color and provided one into a new descendant color.
    /// </summary>
    /// <remarks>
    /// Currently colors are combined by averaging values of their data channels.
    /// </remarks>
    /// <param name="otherSolution"></param>
    /// <returns>
    /// Representation of a new color, which is combination of this color and provided one.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown, when at least one argument is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown, when at least one argument will be considered as invalid.
    /// </exception>
    public IEnumerable<IGeneticAlgorithmSolution> CombineGenomesWith(IGeneticAlgorithmSolution otherSolution)
    {
        #region Arguments validation
        if (otherSolution is null)
        {
            string argumentName = nameof(otherSolution);
            string errorMessage = $"Provided solution is a null reference: {otherSolution}";
            throw new ArgumentNullException(argumentName, errorMessage);
        }

        if (otherSolution is not ColorMatchingAlgorithmSolution)
        {
            string argumentName = nameof(otherSolution);
            const string ErrorMessage = "Provided solution is not a color:";
            throw new ArgumentException(ErrorMessage, argumentName);
        }
        #endregion

        var otherColor = (ColorMatchingAlgorithmSolution)otherSolution;

        var averageChannelsValues = new int[NumberOfChannels];

        for (int channelIndex = 0; channelIndex < NumberOfChannels; channelIndex++)
        {
            double averageChannelValue = new[] { _channelsValues[channelIndex], otherColor._channelsValues[channelIndex] }.Average();
            averageChannelValue = double.Round(averageChannelValue);

            averageChannelsValues[channelIndex] = Convert.ToInt32(averageChannelValue);
        }

        return [new ColorMatchingAlgorithmSolution(averageChannelsValues, _randomNumberGenerator)];
    }

    /// <summary>
    /// Randomly changes one of color properties.
    /// </summary>
    /// <remarks>
    /// Currently mutation is performed by assigning new, random value to a randomly chosen data channel.
    /// </remarks>
    public void Mutate()
    {
        int randomChannelValue = _randomNumberGenerator.Next(MinChannelValue, MaxChannelValue + 1); // Generation range is exclusive at the top.
        int randomChannelIndex = _randomNumberGenerator.Next(NumberOfChannels);

        _channelsValues[randomChannelIndex] = randomChannelValue;
    }

    /// <summary>
    /// Generates string-based representation of this color instance.
    /// </summary>
    /// <returns>
    /// String-based representation of this color.
    /// </returns>
    public override string ToString()
    {
        return $"[{string.Join(", ", _channelsValues)}]";
    }
    #endregion
}
