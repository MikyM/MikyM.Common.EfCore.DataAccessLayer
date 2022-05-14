using System.Collections.Generic;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public interface IInMemorySpecificationEvaluator
{
    IEnumerable<TResult> Evaluate<T, TResult>(IEnumerable<T> source, ISpecification<T, TResult> specification) where T : class;
    IEnumerable<T> Evaluate<T>(IEnumerable<T> source, ISpecification<T> specification) where T : class;
}