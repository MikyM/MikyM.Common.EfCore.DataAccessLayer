using System.Collections.Generic;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public interface IInMemoryEvaluator
{
    IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class;
}