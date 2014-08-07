
namespace Keystone.Web.Utilities.Expression
{
    using System.ComponentModel.DataAnnotations;
    public static class BinaryOperator
    {
        [Display(Name = "Equals To")]
        public static string EqualsTo { get { return "{0}={1}"; } }

        [Display(Name = "Not Equals To")]
        public static string NotEqualsTo { get { return "{0}<>{1}"; } }

        [Display(Name = "Less Than")]
        public static string LessThan { get { return "{0}<{1}"; } }

        [Display(Name = "Less Than Equal")]
        public static string LessThanEqual { get { return "{0}<={1}"; } }

        [Display(Name = "Greater Than")]
        public static string GreaterThan { get { return "{0}>{1}"; } }

        [Display(Name = "Greater Than Equal")]
        public static string GreaterThanEqual { get { return "{0}>={1}"; } }

        [Display(Name = "Contains")]
        public static string Contains { get { return "{0}.ContainsSearchEx({1})"; } }

        [Display(Name = "Starts With")]
        public static string StartsWith { get { return "{0}.StartsWithSearchEx({1})"; } }

        [Display(Name = "Ends With")]
        public static string EndsWith { get { return "{0}.EndsWithSearchEx({1})"; } }
    }
}