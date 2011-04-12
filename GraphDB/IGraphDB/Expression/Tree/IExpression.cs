namespace sones.GraphDB.Expression
{
    /// <summary>
    /// The interface for all expressions
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// The type of the expression (binary/unary/...)
        /// </summary>
        TypeOfExpression TypeOfExpression { get; }
    }
}
