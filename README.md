# Genetic Algorithms Demo

## Description

Repository contains a library, which serves as a base for implementations of [genetic algorithms](https://en.wikipedia.org/wiki/Genetic_algorithm)
along with some demo projects, to give an example of its usage and demonstrate the whole idea of genetic algorithms.
Entire code base is based on *.NET 9.0*.

## Repository Structure

* */doc* - Contains project documentation.
* */src* - Stores *Visual Studio* projects containing source code of the program.

## Documentation

To view project documentation containing more details about
program implementation please open */doc/html/index.html* file.

## Getting started

### Genetic Algorithms Framework

It's a library, which implements core mechanisms of genetic algorithms to make their implementation fast, easy, efficient and secure in terms of data hermietization.
All You need to do is to add reference the library to Your project, create classes derivative to algorithm runner and model of its solution, and implement abstract methods
of those classes in manner according to Your needs and expectations.

### Color Matching Algorithm

Demo project, in which simple implementation of genetic algorithm tries to match its solution to, randomly generated color.
The example is trivial but shows the concept of genetic algorithms and how they works very well.
Currently project is arranged as console application - there is a plan to build algorithm runtime visualization using *Windows Presentation Foundation* (*WPF*).

To launch the application all You need to do is to build it - nothing else needed.

## Used Tools

* IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
* Documentation generator: [DoxyGen 1.12.0](https://www.doxygen.nl/)

## Authors

* Jakub Miodunka
  * [GitHub](https://github.com/JakubMiodunka)
  * [LinkedIn](https://www.linkedin.com/in/jakubmiodunka/)

## License

This project is licensed under the MIT License - see the *LICENSE.md* file for details.
