/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="PandoraDB – ErrorCode enum" />
 * <copyright file="ErrorCodeEnum.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class carries error codes.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Lib.ErrorHandling
{
    
    public enum ErrorCode
    {

        None = 0,

        CPU = 0x01,
        Memory = 0x02,
        UndefinedResource = 0x04,
        ResourceErrorClass = 0x07,

        Syntax = 0x08,
        Semantic = 0x10,
        Logic = 0x20,
        WrongParameterType = 0x21,
        WrongDataTypes = 0x22,
        GrammarErrorClass = 0x38,

        PerformanceWarning = 0x40,
        QualityWarning = 0x80,
        WarningClass = 0xC0,

        NotImplemented,

        DataTypeDoesNotMatchValue,

        /// <summary>Tuple with more than one element are not allowed</summary>
        TupleElementCountMismatch,

        #region Type / Attribute

        /// <summary>An attribute with the given name or uuid does not exist.</summary>
        AttributeNotFound,
        /// <summary>An attribute which is going to be added already exist in any supertype.</summary>
        AttributeExistsInSupertype,
        /// <summary>The attribute already exist for this type.</summary>
        AttributeAlreadExists,
        /// <summary>The type with the given name or uuid does not exist.</summary>
        TypeNotFound,
        /// <summary>The type already exist</summary>
        TypeAlreadyExists,
        /// <summary>The type of the attribute was not found</summary>
        TypeOfAttributeNotFound,
        /// <summary>The name of the attribute is the same as the name of its related type.</summary>
        AmbigousAttribute,
        /// <summary>The parent type does not exist.</summary>
        ParentTypeDoesNotExist,
        /// <summary>An attribute is somehow invalid.</summary>
        InvalidAttribute,

        #endregion

        #region Aggregate / Function

        /// <summary>The aggregate does not exist</summary>
        AggregateOrFunctionDoesNotExist,
        /// <summary>Aggregates allows only one expression</summary>
        AggregateParameterMismatch,

        /// <summary>The number of parameters passed to a function does not match the definition</summary>
        FunctionParameterCountMismatch,
        /// <summary>The type of a function parameter does not match the definition</summary>
        FunctionParameterTypeMismatch,

        /// <summary>If the Type was null or empty, either as string or as uuid</summary>
        ArgumentIsNullOrEmpty_Type,
        /// <summary>If the Type of the attribute was null or empty, either as string or as uuid</summary>
        ArgumentIsNullOrEmpty_AttributeType,
        /// <summary>If the AttributeName was null or empty, either as string or as uuid</summary>
        ArgumentIsNullOrEmpty_AttributeName,
        /// <summary>If the AttributeValue was null or empty, either as string or as uuid</summary>
        ArgumentIsNullOrEmpty_AttributeValue,
        /// <summary>If the ParentType was null or empty, either as string or as uuid</summary>
        ArgumentIsNullOrEmpty_ParentType,
        /// <summary>If the Attributes were null or empty</summary>
        ArgumentIsNullOrEmpty_Attributes,
        /// <summary>If the parameters of a function are null</summary>
        ArgumentIsNull,

        #endregion

        /// <summary>Flush of a DBO failed</summary>
        FlushDBObjectFailed,
        /// <summary>Load of a DBO failed</summary>
        LoadDBObjectFailed,

        #region BackwardEdge

        /// <summary>Creating of a BackwardEdge with a not reference attribute as destination is not allowed</summary>
        BackwardEdgesForNotReferenceAttributeTypesAreNotAllowed,
        /// <summary>Try to create a BackwardEdge Attribute with a typ/attribute combination which is not valid. E.g: User{Friends User} Region{Backwardedge to User.Friends} <- Friends is not of type Region!</summary>
        BackwardEdgeDestinationIsInvalid,
        /// <summary>Try to create a BackwardEdge attribute with an already existing BE to the same type/attribute</summary>
        BackwardEdgeAlreadyExist,

        #endregion

        #region Index

        /// <summary>The index does not exist.</summary>
        IndexDoesNotExist,
        /// <summary>The unqiue contraint was violated. The unique index identifier already contains a key with these values</summary>
        IndexKeyAlreadyExistInUniqueIndex,
        /// <summary>The attribute for which the index should be created does not exist.</summary>
        IndexAttributeDoesNotExist,
        /// <summary>The index type does not exist</summary>
        IndexTypeDoesNotExist,

        #endregion

        /// <summary>The unqiue option is invalid</summary>
        InvalidUniqueOption,

        #region Edge

        /// <summary>The edtype is invalid. On single reference attributes only SingleEdgeTypes allowed</summary>
        InvalidEdgeType_SingleEdgeTypeExpected,
        /// <summary>The edtype is invalid. On list reference attributes only ListEdgeTypes allowed</summary>
        InvalidEdgeType_ListEdgeTypeExpected,
        /// <summary>The edtype is invalid. On ADBBaseObject list attributes only ListBaseEdgeType allowed</summary>
        InvalidEdgeType_ListBaseEdgeTypeExpected,
        /// <summary>The edtype is invalid. An reference edge type was expected</summary>
        InvalidEdgeType_ReferenceEdgeTypeExpected,

        #endregion

        /// <summary>The IDNode is not valid.</summary>
        InvalidIDNode,       
        
        /// <summary>
        /// The setting was not found.
        /// </summary>
        SettingNotExist,

        /// <summary>The setting is incative</summary>
        SettingIsInactive,

        /// <summary>
        /// A TimeOut occured during a query
        /// </summary>
        TimeOut,

        #region Select / BinaryExpression

        /// <summary>
        /// An associativity that is not handled by a binary expression.
        /// </summary>
        UnhandledAssociativity,

        /// <summary>
        /// There has been an error during the evaluation of an expression.
        /// </summary>
        InvalidExpressionEvaluation,

        #endregion


        /// <summary>
        /// A file could not be found.
        /// </summary>
        FileNotFound,

        /// <summary>
        /// error code for HttpRequestResponse
        /// </summary>
        NoDefaultValue_Defined,

        /// <summary>
        /// A statement is invalid
        /// </summary>
        InvalidStatement,

        /// <summary>
        /// There were no elements found by a expression evaluation
        /// </summary>
        NoElementsFound,

        /// <summary>
        /// The mandatory constraint has been violated
        /// </summary>
        MandatoryConstraintViolation,

        /// <summary>
        /// The database was not found in the specified path
        /// </summary>
        DatabaseNotFound,

        /// <summary>
        /// The database was not able to create an ObjectSchemeSetting
        /// </summary>
        CouldNotCreateObjectSchemeSettings,

        #region Insert / Update

        /// <summary>Invalid attribute assignment.</summary>
        InvalidAttributeAssignment,       

        #endregion

    }
}
