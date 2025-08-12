using System;
using System.Linq;
using System.Linq.Expressions;

namespace POL.DB.Root
{
    public static class ExpressionExtensions
    {
        public static Expression Visit<T>(
            this Expression exp,
            Func<T, Expression> visitor) where T : Expression
        {
            return ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static TExp Visit<T, TExp>(
            this TExp exp,
            Func<T, Expression> visitor)
            where T : Expression
            where TExp : Expression
        {
            return (TExp) ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static Expression<TDelegate> Visit<T, TDelegate>(
            this Expression<TDelegate> exp,
            Func<T, Expression> visitor) where T : Expression
        {
            return ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static IQueryable<TSource> Visit<T, TSource>(
            this IQueryable<TSource> source,
            Func<T, Expression> visitor) where T : Expression
        {
            return source.Provider.CreateQuery<TSource>(ExpressionVisitor<T>.Visit(source.Expression, visitor));
        }
    }
}
