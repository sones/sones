namespace sones.GraphDB.Expression
{
    public enum TypeOfExpression
    {
        /// <summary>
        /// binary expression like User/Age = 1
        /// </summary>
        Binary,
        
        /// <summary>
        /// unary expression like Not(Expression)
        /// </summary>
        Unary,

        /// <summary>
        /// a constant like 20 or "Alice"
        /// </summary>
        Constant,

        /// <summary>
        /// a property like User/Name
        /// </summary>
        Property
    }
}