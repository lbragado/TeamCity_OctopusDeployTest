using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Entities.Specifications
{
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> Expression { get;  }

        public bool IsSatisfiedBy(T entity) {
            Func<T, bool> DelegateExpression = Expression.Compile();
            return DelegateExpression(entity);
        }

        public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);

        public Specification<T> Not() => new NotSpecification<T>(this);
    }
}
