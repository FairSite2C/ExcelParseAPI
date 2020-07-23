using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Configuration;

//http://aonnull.blogspot.com/2010/08/dynamic-sql-like-linq-orderby-extension.html
public static class OrderByHelper
{
    public static IEnumerable<T> MaybeMultipleSorts<T>(this IEnumerable<T> enumerable, string sortBy)
    {
        return enumerable.AsQueryable().MaybeMultipleSorts(sortBy).AsEnumerable();
    }

    public static IQueryable<T> MaybeMultipleSorts<T>(this IQueryable<T> collection, string sortBy)
    {
        foreach (OrderByInfo orderByInfo in ParseOrderBy(sortBy))
        {
            collection = ApplyOrderBy<T>(collection, orderByInfo);
        }

        return collection;
    }

    private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
    {
        Type type = typeof(T);

        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;

        string prop = orderByInfo.PropertyName;

        // use reflection (not ComponentModel) to mirror LINQ
        PropertyInfo pi = OrmHelper.GetPropertyInfo<T>(prop);
        expr = Expression.Property(expr, pi);
        type = pi.PropertyType;

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
        string methodName = String.Empty;

        if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
        {
            methodName = orderByInfo.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending";
        }
        else
        {
            methodName = orderByInfo.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
        }

        //TODO: apply caching to the generic methodsinfos?
        return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
            method => method.Name == methodName
                    && method.IsGenericMethodDefinition
                    && method.GetGenericArguments().Length == 2
                    && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), type)
            .Invoke(null, new object[] { collection, lambda });
    }

    private static IEnumerable<OrderByInfo> ParseOrderBy(string sortBy)
    {
        // since this is buried deep and all sorts coming through controllers
        // are properly formatted I assume the sortBy string is proper

        var  retVal = new List<OrderByInfo>();

        string[] items = sortBy.Split(',');
        bool initial = true;

        foreach (string item in items)
        {
            string[] pair = item.Split(':');

            string prop = pair[0];

            SortDirection dir = SortDirection.Ascending;

            if (pair.Length == 2)
            {
                dir = pair[1] == "desc" ? SortDirection.Descending : SortDirection.Ascending;
            }

            yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

            initial = false;
        }
    }

    private class OrderByInfo
    {
        public string PropertyName { get; set; }
        public SortDirection Direction { get; set; }
        public bool Initial { get; set; }
    }

    private enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}

public static class OrmHelper
{
    public static Type GetType(string objType)
    {
        return Type.GetType(objType).MakeGenericType();
    }

    public static T GetInstance<T>(string type)
    {
        return (T)Activator.CreateInstance(Type.GetType(type));
    }
       
    public static T CreateType<T>() where T : new()
    {
        return new T();
    }

    public static bool PropertyExists<T>(string propertyName)
    {
        return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
            BindingFlags.Public | BindingFlags.Instance) != null;
    }

    public static PropertyInfo GetPropertyInfo<T>(string propertyName)
    {
        return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
            BindingFlags.Public | BindingFlags.Instance);
    }

    public static void SetPropertyValue<T>(ref T obj, string propertyName, dynamic value)
    {
        if (!PropertyExists<T>(propertyName)) return;

        var property = obj.GetType().GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        Type propertyType = property.PropertyType;

        var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;

        value = Convert.ChangeType(value, targetType);

        property.SetValue(obj, value, null);
    }

    private static bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }

    public static dynamic GetPropertyValue<T>(T obj, string propertyName)
    {
        if (!PropertyExists<T>(propertyName)) return null;

        var property = obj.GetType().GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
  
        return property.GetValue(obj);
    }

    private static Type GetElementType(IQueryable source)
    {
        Expression expr = source.Expression;
        Type elementType = source.ElementType;
        while (expr.NodeType == ExpressionType.Call &&
               elementType == typeof(object))
        {
            var call = (MethodCallExpression)expr;
            expr = call.Arguments.First();
            elementType = expr.Type.GetGenericArguments().First();
        }

        return elementType;
    }

    public static Expression GetBinaryExpression<TSource>(string propertyName, int comparer, dynamic valueIn)
    {

        if (!PropertyExists<TSource>(propertyName)) return null;

        /*
        ParameterExpression parameter = Expression.Parameter(typeof(TSource), typeof(TSource).Name);

        //the property of the user object to use in expression
        Expression property = Expression.Property(parameter, propertyName);

        Type pt = GetPropertyInfo<TSource>(propertyName).PropertyType;

        var targetType = IsNullableType(pt) ? Nullable.GetUnderlyingType(pt) : pt;

        value = Convert.ChangeType(value, targetType);

        //the value to compare to the user property
        Expression val = Expression.Constant(value);
        /*
                Equal = 1,
                NotEqual = 2,
                GreaterThan = 3,
                GreaterThanEqual = 4,
                LessThan = 5,
                LessThanEqual = 6
        //the binary expression using the above expressions

        ParameterExpression param = Expression.Parameter(typeof(TSource));
        Expression left = Expression.Property(param, propertyName);
        left = Expression.Convert(left, pt);
        Expression right = Expression.Constant(value);
*/
        var parameter = Expression.Parameter(typeof(TSource), "t");
        var property = Expression.Property(parameter, propertyName);

        PropertyInfo pi = GetPropertyInfo<TSource>(propertyName);
        Type pt = pi.PropertyType;
        //Type pt = GetPropertyInfo<TSource>(propertyName).PropertyType;
 


        if (pt == typeof(System.DateTime))
        {
            var outVal = new DateTime();
            DateTime.TryParse(valueIn, out outVal);
            valueIn = outVal.ToString("yyyy-MM-dd hh:mm:ss");
            //property = Expression.MakeMemberAccess(property, typeof(DateTime).GetMember("Date").Single());
        }

              
        var targetType = IsNullableType(pt) ? Nullable.GetUnderlyingType(pt) : pt;
       
        valueIn = Convert.ChangeType(valueIn, targetType);

        var value = Expression.Constant(valueIn);
        var val = Expression.Convert(value, property.Type);

        BinaryExpression exp = null;

        switch (comparer)
        {
            case 1:
                exp = Expression.Equal(property, val);
                break;
            case 2:
                exp = Expression.NotEqual(property, val);
                break;
            case 3:
                exp = Expression.GreaterThan(property, val);
                break;
            case 4:
                exp = Expression.GreaterThanOrEqual(property, val);
                break;
            case 5:
                exp = Expression.LessThan(property, val);
                break;
            case 6:
                exp = Expression.LessThanOrEqual(property, val);
                break;
            case 7:
                return ExpStartsWith<TSource>(pi, valueIn);
            case 8:
                return ExpContains<TSource>(pi, valueIn);

        }

        //create the Expression<Func<T, Boolean>>
        return Expression.Lambda<Func<TSource, Boolean>>(exp, parameter);
    }

    public static Expression ExpStartsWith<T>(PropertyInfo propertyInfo, string propertyValue)
    {

        ParameterExpression e = Expression.Parameter(typeof(T), "e");
        MemberExpression m = Expression.MakeMemberAccess(e, propertyInfo);
        ConstantExpression c = Expression.Constant(propertyValue, typeof(string));
        MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

        var call = Expression.Call(m, mi, c);

        return Expression.Lambda<Func<T, bool>>(call, e);
    }

    static Expression ExpContains<T>(PropertyInfo propertyInfo, string propertyValue)
    {
        ParameterExpression e = Expression.Parameter(typeof(T), "e");
        MemberExpression m = Expression.MakeMemberAccess(e, propertyInfo);
        ConstantExpression c = Expression.Constant(propertyValue, typeof(string));
        MethodInfo mi = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

        var call = Expression.Call(m, mi, c);

        return Expression.Lambda<Func<T, bool>>(call, e);
    }

    public static IQueryable<TSource> EqualToProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 1, value);

        return WhereWrapper(source, expression);
    }

    public static IQueryable<TSource> NotEqualToProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 2, value);

        return WhereWrapper(source, expression);
    }

    public static IQueryable<TSource> GreaterProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 3, value);

        return WhereWrapper(source, expression);
    }

    public static IQueryable<TSource> GreaterEqualProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 4, value);

        return WhereWrapper(source, expression);
    }

    public static IQueryable<TSource> LessProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 5, value);

        return WhereWrapper(source, expression);
    }

    public static IQueryable<TSource> LessEqualProperty<TSource>(this IQueryable<TSource> source, string propertyName, dynamic value)
    {
        var expression = GetBinaryExpression<TSource>(propertyName, 6, value);

        return WhereWrapper(source, expression);
    }
      
    public static Expression<Func<T, bool>> ExpressionAnd<T>(Expression<Func<T, Boolean>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return (Expression.Lambda<Func<T, Boolean>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters));
    }

    public static Expression<Func<T, bool>> ExpressionOr<T>(Expression<Func<T, Boolean>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return (Expression.Lambda<Func<T, Boolean>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters));
    }

    public static IQueryable<TSource> WhereWrapper<TSource>(this IQueryable<TSource> source, Expression expression)
    {
        try
        {
            var retVal = (IQueryable<TSource>)source.Provider.CreateQuery(Expression.Call(typeof(Queryable),
                                                                         "Where",
                                                                         new[] { GetElementType(source) },
                                                                         source.Expression,
                                                                         Expression.Quote(expression)));
            return retVal;
        }
        catch
        {
            throw new ArgumentNullException();
        }
    }

    public static IQueryable<T> AddStdCollectionParameters<T>(IQueryable<T> q,
    int offset = 0, int limit = 0, bool includeDeleted = false, string sortBy = "", string updatedSince = "")
    {

        if (!includeDeleted)
        {
            q = q.EqualToProperty("Deleted", false);
        }

         if (!String.IsNullOrEmpty(sortBy))
        {
            q = q.MaybeMultipleSorts(sortBy);

            // these do not work without a sort
            // so only set if have a sort
            if (offset > 0) q = q.Skip(offset);

            if (limit > 0) q = q.Take(limit);

        }

        return q;

    }

    public static void SetAuditColumns<T>(ref T entity, bool Adding , long personId)
    {
        if (Adding)
        {
            SetPropertyValue<T>(ref entity, "CreateDT", DateTime.Now.ToUniversalTime());
            SetPropertyValue<T>(ref entity, "CreateBy", personId);
            SetPropertyValue<T>(ref entity, "Deleted", false);
        }

        SetPropertyValue<T>(ref entity, "UpdateDT", DateTime.Now.ToUniversalTime());
        SetPropertyValue<T>(ref entity, "UpdateBy", personId);
    }
     
    public static bool IsTrue(string value)
    {
        try
        {
            // 1
            // Avoid exceptions
            if (value == null)
            {
                return false;
            }

            // 2
            // Remove whitespace from string
            value = value.Trim();

            // 3
            // Lowercase the string
            value = value.ToLower();

            // 4
            // Check for word true
            if (value == "true")
            {
                return true;
            }

            // 5
            // Check for letter true
            if (value == "t")
            {
                return true;
            }

            // 6
            // Check for one
            if (value == "1")
            {
                return true;
            }

            // 7
            // Check for word yes
            if (value == "yes")
            {
                return true;
            }

            // 8
            // Check for letter yes
            if (value == "y")
            {
                return true;
            }

            // 9
            // It is false
            return false;
        }
        catch
        {
            return false;
        }
    }
}

 


