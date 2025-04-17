# HSH Code assignment

## About

This solution is just a simulated solution for developing a application according to an assignment and has thus no real usage other than an example.

**NOTE** The document with the actual assignment is not provided within this solution/repository.

## Design

The following design decision were made:

* OS: `Windows 64-bit` application,
* Language / framework: `C#` with `.NET 8` SDK
* Assembly type: `Windows Presentation Foundation (WPF)` as a user interface application
* Unit test frameowrk: `xUnit`
* Strategy:
  * Using the `Model-View-ViewModel` design pattern, however not as strict as it would be in a clean book.
  * Partial test coverage to injectable 'services' but not full simply to save some time.
  * Separate C# projects:
    * **DataViewer** - the composition of UI and the main entry point (holds Views, Models, ViewModels and relevant UI Resources and Common tool logic)
    * **DataViewer.AppLogic** - re-usable handlers and 'services' used by the tool
    * **DataViewer.AppLogic.Tests** - unit tests for AppLogic
    * **DataViewer.Interfaces** - keeping the interfaces entirely separate as to simulate a possible requirement of reusability and modular approach
    * **DataViewer.Models** - data structures (exceptions and json data representation) that can be considered as "they dont necessarily have anything to do with UI"

## Coding style

This solution follows the following [coding style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) using the `.editorconfig` file present in the solution root folder (natively integrated by the visual studio IDE).

I have decided to deviate from certain coding style rules, namely in the usage of the `var` keyword in unit testing or when constructing object instances using explicitly obvious functions:
  * `var xxx = TimeSpan.FromYYY()`, or
  * `var ex = Assert.Throws<MyException>(...)`, or even at 
  * `var xx = $"...string format expression ..."` and
  * `var xx = default(string?)` - albeit equivalent to `string? xx = null`, i much prefer the former format especially when there are multiple variables declared of which names are nicely shown (aligned) to the left whereas if each variable name would have had its own data type first and the name's position distorted *somewhere to the right*, its pain to read.
  * `foreach (var article in articles)`, as this is also available in the builtin text template
  * any primitive .net literals that are very obvious for the reader - if this would however be a loop in a list of `Regex matches` , i would use an explicit type instead of var as it is a must there.
  * additionally, if there will only be a single instance of a `some json parser` then i would never have the need to be explicit about it, because it cannot be ambiguous to the compiler.

of which usage i strongly support and encourage due to much higher readability rather than being 100% strict about a discipline which cannot possibly reason about useful and obvious exceptions. Its much better to be forced to use a better naming convention where context can be easily inferred from it, expressing our intention rather than obliging a rule.

I much more prefer to use the explicit data type only in "tricky" corners where it's really hard to determine the data type and on places where the reader would strongly benefit from knowing what they are dealing with.

If however i am to work in an existing project with others where there are different rules than what i chose then I would of course adapt, which is the thing everyone should do.

## Versioning

Additionally the format of the `git commit messages` should be simple but descriptive in summarising what was changed and why - but this is rather a recommendation than an assertive rule.

## Implemented by

Martin Holkovic (https://github.com/bo100nka)