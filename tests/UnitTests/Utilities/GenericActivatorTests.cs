using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities;

/// <summary>
/// Tests for <see cref="GenericActivator"/>
/// </summary>
public sealed class GenericActivatorTests
{
    [Fact(DisplayName = "Create returns instance when calling constructor without parameters")]
    public void Create_WhenCtorWithoutParameters_ReturnsInstance()
    {
        // Arrange, Act
        var instance = GenericActivator.Create<NoCtorParametersClass>();

        // Assert
        instance.ShouldNotBeNull();
        instance.Value.ShouldBe(NoCtorParametersClass.DefaultValue);
    }

    [Fact(DisplayName = "Create returns instance when calling constructor with parameter")]
    public void Create_WhenCtorWithParameter_ReturnsInstance()
    {
        // Arrange, Act
        var value = 200;
        var instance = GenericActivator.Create<int, OneCtorParameterClass>(value);

        // Assert
        instance.ShouldNotBeNull();
        instance.Value.ShouldBe(value);
    }

    [Fact(DisplayName = "GetInstanceInitializer returns instance initializer")]
    public void GetInstanceInitializer_ReturnsInitializer()
    {
        // Arrange
        var instanceType = typeof(NoCtorParametersClass);

        // Act
        var instanceInitializer = GenericActivator.GetInstanceInitializer<NoCtorParametersClass>(instanceType);
        var instance = instanceInitializer.Invoke();

        // Assert
        instance.ShouldNotBeNull();
        instance.Value.ShouldBe(NoCtorParametersClass.DefaultValue);
    }

    [Fact(DisplayName = "BuildFactory returns factory for concrete type")]
    public void BuildFactory_WhenConcreteType_ReturnsFactory()
    {
        // Arrange
        var instanceType = typeof(NoCtorParametersClass);

        // Act
        var factory = GenericActivator.BuildFactory<NoCtorParametersClass>(instanceType);
        var instance = factory.Invoke();

        // Assert
        instance.ShouldNotBeNull();
        instance.Value.ShouldBe(NoCtorParametersClass.DefaultValue);
    }

    [Fact(DisplayName = "BuildFactory returns factory for derived type")]
    public void BuildFactory_WhenDerivedType_ReturnsFactory()
    {
        // Arrange
        var instanceType = typeof(DerivedNoCtorParametersClass);

        // Act
        var factory = GenericActivator.BuildFactory<NoCtorParametersClass>(instanceType);
        var instance = factory.Invoke();

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeOfType<DerivedNoCtorParametersClass>();
        instance.Value.ShouldBe(DerivedNoCtorParametersClass.DerivedDefaultValue);
    }

    [Fact(DisplayName = "BuildFactory returns new instances on multiple calls")]
    public void BuildFactory_WhenCalledMultipleTimes_ReturnsNewInstancesEachTime()
    {
        // Arrange
        var factory = GenericActivator.BuildFactory<NoCtorParametersClass>(typeof(NoCtorParametersClass));

        // Act
        var instance1 = factory.Invoke();
        var instance2 = factory.Invoke();

        // Assert
        instance1.ShouldNotBeNull();
        instance2.ShouldNotBeNull();
        instance1.ShouldNotBeSameAs(instance2);
    }

    private class NoCtorParametersClass
    {
        public const int DefaultValue = 100;

        public int Value { get; protected init; } = DefaultValue;
    }

    private sealed class OneCtorParameterClass : NoCtorParametersClass
    {
        public OneCtorParameterClass(int value)
        {
            Value = value;
        }
    }

    private sealed class DerivedNoCtorParametersClass : NoCtorParametersClass
    {
        public const int DerivedDefaultValue = 500;

        public DerivedNoCtorParametersClass()
        {
            Value = DerivedDefaultValue;
        }
    }
}
