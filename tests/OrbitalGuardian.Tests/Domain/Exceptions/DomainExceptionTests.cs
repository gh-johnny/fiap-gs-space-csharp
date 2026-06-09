using FluentAssertions;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Tests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void InvalidOrbitalElementsException_InheritsFromBase()
    {
        new InvalidOrbitalElementsException("msg").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void InvalidStateVectorException_InheritsFromBase()
    {
        new InvalidStateVectorException("msg").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void InvalidCollisionProbabilityException_InheritsFromBase()
    {
        new InvalidCollisionProbabilityException("msg").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void SpaceObjectNotFoundException_InheritsFromBase()
    {
        new SpaceObjectNotFoundException("25544").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void ConjunctionAlreadyClosedException_InheritsFromBase()
    {
        new ConjunctionAlreadyClosedException("closed").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void AlertAlreadyAcknowledgedException_InheritsFromBase()
    {
        new AlertAlreadyAcknowledgedException("acked").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void InvalidStateTransitionException_InheritsFromBase()
    {
        new InvalidStateTransitionException("A", "B").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void UserNotFoundException_InheritsFromBase()
    {
        new UserNotFoundException("test@x.com").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void InvalidCredentialsException_InheritsFromBase()
    {
        new InvalidCredentialsException().Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }

    [Fact]
    public void DuplicateEmailException_InheritsFromBase()
    {
        new DuplicateEmailException("test@x.com").Should().BeAssignableTo<OrbitalGuardianDomainException>();
    }
}
