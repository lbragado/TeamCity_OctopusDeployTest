using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Entities.Specifications
{
    public class NotSpecification<T> : Specification<T>
    {
        readonly Specification<T> Specification;

        public NotSpecification(Specification<T> specification) => Specification = specification;

        public override Expression<Func<T, bool>> Expression
        {
            get
            {
                var Param = System.Linq.Expressions.Expression
                    .Parameter(typeof(T));

                var Body = System.Linq.Expressions.Expression
                    .Not(System.Linq.Expressions.Expression
                    .Invoke(Specification.Expression, Param));

                return System.Linq.Expressions.Expression
                    .Lambda<Func<T, bool>>(Body, Param);
            }
        }


    }
}
