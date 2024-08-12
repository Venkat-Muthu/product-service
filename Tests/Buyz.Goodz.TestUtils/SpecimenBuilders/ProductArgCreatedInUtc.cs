using AutoFixture.Kernel;
using Buyz.Goodz.Domain.AggregateRoots;
using System.Reflection;

namespace Buyz.Goodz.TestUtils.SpecimenBuilders;

public class ProductArgCreatedInUtc : ISpecimenBuilder
{
    private readonly DateTime value;

    public ProductArgCreatedInUtc(DateTime value)
    {
        this.value = value;
    }

    public object Create(object request, ISpecimenContext context)
    {
        var pi = request as ParameterInfo;
        if (pi == null)
            return new NoSpecimen();

        if (pi.Member.DeclaringType != typeof(Product) ||
            pi.ParameterType != typeof(DateTime) ||
            pi.Name != "createdInUtc")
            return new NoSpecimen();

        return value;
    }
}
