/* <id name="GraphDB – TypesOfOperators Enum" />
 * <copyright file="TypesOfOperators.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

namespace sones.GraphDB.Structures.Enums
{
    /// <summary>
    /// This enum describes the impact of operators on ExpressionGraph integration
    /// </summary>
    public enum TypesOfOperators
    {
        AffectsLocalLevelOnly, //affects the local level key only
        AffectsLowerLevels, //affects lower levels too
    }
}
