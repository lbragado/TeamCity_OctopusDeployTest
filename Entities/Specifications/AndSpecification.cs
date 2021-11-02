using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Entities.Specifications
{
    public class AndSpecification<T> : Specification<T>
    {
        readonly Specification<T> Left;
        readonly Specification<T> Right;

        public AndSpecification(
                Specification<T> left,
                Specification<T> right) =>
                    (Left, Right) = (left, right);
            
        public override Expression<Func<T, bool>> Expression
        {
            get
            {
                var Param = System.Linq.Expressions.Expression.Parameter(typeof(T));

                var Body = System.Linq.Expressions.Expression.AndAlso(System.Linq.Expressions.Expression
                            .Invoke(Left.Expression, Param),
                            System.Linq.Expressions.Expression
                            .Invoke(Right.Expression, Param));

                return System.Linq.Expressions.Expression
                        .Lambda<Func<T, bool>>(Body, Param);
            }
        }


    }
}
