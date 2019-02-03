# Contributing to ORM

## Local development

ORM can be developed locally. I personally use the following environment:

- Visual Studio 2017 Community

This project already has a pre-configured .editorconfig file, so that the configuration of the code style will be automatically loaded by Visual Studio, and code that violates the code style rule won't pass compile.
Because this project is based on the .NET standard and the .NET core, you can also try to choose other development environments.

## Unit Test

Unit test is required for all code. The test coverage goal of this library is 100% :D
For a library, UT is almost the only way to test the behavior of APIs and write UT can help you to decouple your code.
There are some basic rules of UT:

- **Test Naming** - Create Tests class for each target class with {ClassName}Tests, and create test method for each public API with Test{MethodName}\_{ArgumentsType}\_{Optional description}.

- **Always test public interface** - That's means you shall not test private methods by using reflection to increase your code coverage. When you writing UT, you should play as the 'user' of your test target.

- **use Mock to resolve dependencies** - Mock is a great tool to help you deal the dependencies of your test target. Avoid introduce too much real dependecies which you actually not want to test.

## Pull Request

// TODO
