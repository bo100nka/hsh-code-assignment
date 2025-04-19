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
    * **DataViewer.Tests** - unit tests for DataViewer
    * **DataViewer.AppLogic** - re-usable handlers and 'services' used by the tool
    * **DataViewer.AppLogic.Tests** - unit tests for DataViewer.AppLogic
    * **DataViewer.Interfaces** - keeping the interfaces entirely separate as to simulate a possible requirement of reusability and modular approach
    * **DataViewer.Data** - data structures (exceptions and json data representation) that can be considered as "they dont necessarily have anything to do with UI"
    * **DataViewer.Data.Tests** - unit tests for DataViewer.Data

## Coding style

I have tried hard to follow the [coding style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) using the `.editorconfig` file present in the solution root folder (natively integrated by the visual studio IDE).

TODO: I have tried to run `dotnet format` inside the command inside the solution folder but i am not sure whether the IDE automatically enforces the rules or I have to run some tool on top that parses this config and then applies code formatting suggestions hence follows a section where i explain some deviations if encountered in the code.

I have decided to deviate from certain coding style rules and rather than writing it in the code as comments (which i have never seen anyone do in my life it), i have rather decided to talk about it here, in a single place.

### Deviations:
* usage of `this` - there are actual several usecases where i was forced to do it:
  * `ctor() : this(xxx)` - a constructor refering to another overload of the same constructor
  * `onEvent?.Invoke(this, args)`
  * `if (ReferenceEquals(this, other))` - a typical implementation of the `IEquatable<T>` interface
  * `static void Do(this object obj, ...)` - extensions
* unit tests for constructors:
  * i start a function that represents a test case as `ctor_ValidateArguments()` as an example, as the "ctor" is a shortcut and the "ending" of the word Constructor. For some reason i am simply used to this.
  * i have replaced it with `Constructor_` later, but this could be seen in git commit history
* the following functions have underscores in them:
  * unit test function names are in `Function_Condition_Behavior`, otherwise i would find it unreadable if they were missing
  * any functions used as an event handler: `obj.OnEvent += Obj_OnEvent;` - this is for much faster spoting difference between an actual event handler and also the obj of which event it did the function subscribe to (Obj).
* but namely in the usage of the `var` keyword in unit testing or when constructing object instances using explicitly obvious functions:
  * `var xxx = TimeSpan.FromYYY()`, or
  * `var ex = Assert.Throws<MyException>(...)`, or even at 
  * `var xx = $"...string format expression ..."` and
  * `var xx = default(string?)` - albeit equivalent to `string? xx = null`, i much prefer the former format especially when there are multiple variables declared of which names are nicely shown (aligned) to the left whereas if each variable name would have had its own data type first and the name's position distorted *somewhere to the right*, its pain to read.
  * `foreach (var article in articles)`, as this is also available in the builtin text template
  * any primitive .net literals that are very obvious for the reader - if this would however be a loop in a list of `Regex matches` , i would use an explicit type instead of var as it is a must there.
  * additionally, if there will only be a single instance of a `some json parser` then i would never have the need to be explicit about it, because it cannot be ambiguous to the compiler and thus, even a call such as `var parser = GetCustomJsonParser()` should be explicitly clear to the caller and make it obvious that rather than focusing on some rule, the "intention" is more important here.

### Remarks
Its much better to be forced to use a better naming convention where context can be easily inferred from it, expressing our intention rather than obliging a rule.

I much more prefer to use the explicit data type only in "tricky" corners where it's really hard to determine the data type and on places where the reader would strongly benefit from knowing what they are dealing with.

Lastly, my experience is that I have almost never been allowed the luxury of finetuning, proper refactoring and proper formatting, because as soon as an application was demonstrated to someone, it then became next to impossible to reason with a product manager about why we need more time to spend with it.

Most of the rules, in my opinion, should be managable by IDE and it's addons which should then raise various warnings for the developer so he is aware about deviating from something.

### Final thought

If however i am to work in an existing project with others where there are different rules than what i chose then I would of course try hard to adapt to any existing discipline and rules.

## Versioning

Additionally the format of the `git commit messages` should be simple but descriptive in summarising what was changed and why - but this is rather a recommendation than an assertive rule.

## Implemented by

Martin Holkovic (https://github.com/bo100nka)