/*-----------------------------------------------------------------------
<copyright file="UrlTemplate.cs" company="Environmental Science Associates">
Copyright (c) Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LtInfo.Common.DesignByContract;

namespace LtInfo.Common
{
    public class UrlTemplate<T1> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate) : base(urlTemplate, 1)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1)
        {
            var replace = ParameterReplace(UrlTemplateString, 1, realParameter1);
            Check.Ensure(AllParametersReplaced(replace), "Template replacement left unreplaced parameter");
            return replace;
        }
    }

    public class UrlTemplate<T1, T2> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate) : base(urlTemplate, 2)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            Check.Ensure(AllParametersReplaced(replace2), "Template replacement left unreplaced parameter");
            return replace2;
        }        
    }

    public class UrlTemplate<T1, T2, T3> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate) : base(urlTemplate, 3)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            Check.Ensure(AllParametersReplaced(replace3), "Template replacement left unreplaced parameter");
            return replace3;
        }        
    }

    public class UrlTemplate<T1, T2, T3, T4> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 4)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            Check.Ensure(AllParametersReplaced(replace4), "Template replacement left unreplaced parameter");
            return replace4;
        }
    }

    public class UrlTemplate<T1, T2, T3, T4, T5> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 5)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            Check.Ensure(AllParametersReplaced(replace5), "Template replacement left unreplaced parameter");
            return replace5;
        }
    }

    public class UrlTemplate<T1, T2, T3, T4, T5, T6> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 6)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5, T6 realParameter6)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            Check.Ensure(AllParametersReplaced(replace6), "Template replacement left unreplaced parameter");
            return replace6;
        }
    }

    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 7)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5, T6 realParameter6, T7 realParameter7)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            Check.Ensure(AllParametersReplaced(replace7), "Template replacement left unreplaced parameter");
            return replace7;
        }
    }

    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 8)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5, T6 realParameter6, T7 realParameter7, T8 realParameter8)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            Check.Ensure(AllParametersReplaced(replace8), "Template replacement left unreplaced parameter");
            return replace8;
        }
    }

    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8, T9> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 9)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
            Check.Require(IsRequiredType<T9>(CalculateType(urlTemplate, 9)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5, T6 realParameter6, T7 realParameter7, T8 realParameter8,
                                       T9 realParameter9)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            var replace9 = ParameterReplace(replace8, 9, realParameter9);
            Check.Ensure(AllParametersReplaced(replace9), "Template replacement left unreplaced parameter");
            return replace9;
        }

    }

    // 10 Parameters
    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 10)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
            Check.Require(IsRequiredType<T9>(CalculateType(urlTemplate, 9)), TypeErrorMessage);
            Check.Require(IsRequiredType<T10>(CalculateType(urlTemplate, 10)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1, T2 realParameter2, T3 realParameter3, T4 realParameter4, T5 realParameter5, T6 realParameter6, T7 realParameter7, T8 realParameter8, T9 realParameter9, T10 realParameter10)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            var replace9 = ParameterReplace(replace8, 9, realParameter9);
            var replace10 = ParameterReplace(replace9, 10, realParameter10);
            Check.Ensure(AllParametersReplaced(replace10), "Template replacement left unreplaced parameter");
            return replace10;
        }
    }


    // 11 Parameters
    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 11)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
            Check.Require(IsRequiredType<T9>(CalculateType(urlTemplate, 9)), TypeErrorMessage);
            Check.Require(IsRequiredType<T10>(CalculateType(urlTemplate, 10)), TypeErrorMessage);
            Check.Require(IsRequiredType<T11>(CalculateType(urlTemplate, 11)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1,
                                       T2 realParameter2,
                                       T3 realParameter3,
                                       T4 realParameter4,
                                       T5 realParameter5,
                                       T6 realParameter6,
                                       T7 realParameter7,
                                       T8 realParameter8,
                                       T9 realParameter9,
                                       T10 realParameter10,
                                       T11 realParameter11)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            var replace9 = ParameterReplace(replace8, 9, realParameter9);
            var replace10 = ParameterReplace(replace9, 10, realParameter10);
            var replace11 = ParameterReplace(replace10, 11, realParameter11);
            Check.Ensure(AllParametersReplaced(replace11), "Template replacement left unreplaced parameter");
            return replace11;
        }
    }


    // 12 Parameters
    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 12)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
            Check.Require(IsRequiredType<T9>(CalculateType(urlTemplate, 9)), TypeErrorMessage);
            Check.Require(IsRequiredType<T10>(CalculateType(urlTemplate, 10)), TypeErrorMessage);
            Check.Require(IsRequiredType<T11>(CalculateType(urlTemplate, 11)), TypeErrorMessage);
            Check.Require(IsRequiredType<T12>(CalculateType(urlTemplate, 12)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1,
                                       T2 realParameter2,
                                       T3 realParameter3,
                                       T4 realParameter4,
                                       T5 realParameter5,
                                       T6 realParameter6,
                                       T7 realParameter7,
                                       T8 realParameter8,
                                       T9 realParameter9,
                                       T10 realParameter10,
                                       T11 realParameter11,
                                       T12 realParameter12)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            var replace9 = ParameterReplace(replace8, 9, realParameter9);
            var replace10 = ParameterReplace(replace9, 10, realParameter10);
            var replace11 = ParameterReplace(replace10, 11, realParameter11);
            var replace12 = ParameterReplace(replace11, 12, realParameter12);
            Check.Ensure(AllParametersReplaced(replace12), "Template replacement left unreplaced parameter");
            return replace12;
        }
    }



    // 13 Parameters
    public class UrlTemplate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : UrlTemplate
    {
        public UrlTemplate(string urlTemplate)
            : base(urlTemplate, 13)
        {
            Check.Require(IsRequiredType<T1>(CalculateType(urlTemplate, 1)), TypeErrorMessage);
            Check.Require(IsRequiredType<T2>(CalculateType(urlTemplate, 2)), TypeErrorMessage);
            Check.Require(IsRequiredType<T3>(CalculateType(urlTemplate, 3)), TypeErrorMessage);
            Check.Require(IsRequiredType<T4>(CalculateType(urlTemplate, 4)), TypeErrorMessage);
            Check.Require(IsRequiredType<T5>(CalculateType(urlTemplate, 5)), TypeErrorMessage);
            Check.Require(IsRequiredType<T6>(CalculateType(urlTemplate, 6)), TypeErrorMessage);
            Check.Require(IsRequiredType<T7>(CalculateType(urlTemplate, 7)), TypeErrorMessage);
            Check.Require(IsRequiredType<T8>(CalculateType(urlTemplate, 8)), TypeErrorMessage);
            Check.Require(IsRequiredType<T9>(CalculateType(urlTemplate, 9)), TypeErrorMessage);
            Check.Require(IsRequiredType<T10>(CalculateType(urlTemplate, 10)), TypeErrorMessage);
            Check.Require(IsRequiredType<T11>(CalculateType(urlTemplate, 11)), TypeErrorMessage);
            Check.Require(IsRequiredType<T12>(CalculateType(urlTemplate, 12)), TypeErrorMessage);
            Check.Require(IsRequiredType<T13>(CalculateType(urlTemplate, 13)), TypeErrorMessage);
        }

        public string ParameterReplace(T1 realParameter1,
                                       T2 realParameter2,
                                       T3 realParameter3,
                                       T4 realParameter4,
                                       T5 realParameter5,
                                       T6 realParameter6,
                                       T7 realParameter7,
                                       T8 realParameter8,
                                       T9 realParameter9,
                                       T10 realParameter10,
                                       T11 realParameter11,
                                       T12 realParameter12,
                                       T13 realParameter13)
        {
            var replace1 = ParameterReplace(UrlTemplateString, 1, realParameter1);
            var replace2 = ParameterReplace(replace1, 2, realParameter2);
            var replace3 = ParameterReplace(replace2, 3, realParameter3);
            var replace4 = ParameterReplace(replace3, 4, realParameter4);
            var replace5 = ParameterReplace(replace4, 5, realParameter5);
            var replace6 = ParameterReplace(replace5, 6, realParameter6);
            var replace7 = ParameterReplace(replace6, 7, realParameter7);
            var replace8 = ParameterReplace(replace7, 8, realParameter8);
            var replace9 = ParameterReplace(replace8, 9, realParameter9);
            var replace10 = ParameterReplace(replace9, 10, realParameter10);
            var replace11 = ParameterReplace(replace10, 11, realParameter11);
            var replace12 = ParameterReplace(replace11, 12, realParameter12);
            var replace13 = ParameterReplace(replace12, 13, realParameter13);
            Check.Ensure(AllParametersReplaced(replace13), "Template replacement left unreplaced parameter");
            return replace13;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class UrlTemplate
    {
        public readonly string UrlTemplateString;
        public readonly Type[] Types;

        protected UrlTemplate(string urlTemplateString, int howManyToVerify)
        {
            Check.Require(IsValidUrlTemplate(urlTemplateString, howManyToVerify), "Url Template is not valid");
            UrlTemplateString = urlTemplateString;
            Types = CalculateTypes(urlTemplateString, howManyToVerify);
        }


        protected string ParameterReplace<TParameter>(string urlTemplate, int parameterPosition, TParameter parameterValue)
        {
            Check.Require(IsRequiredType<TParameter>(Types[parameterPosition - 1]), TypeErrorMessage);

            if (typeof(TParameter) == typeof(int))
            {
                return urlTemplate.Replace(IntParameters[parameterPosition - 1].ToString(), parameterValue.ToString());
            }
            return urlTemplate.Replace(StringParameters[parameterPosition - 1], parameterValue.ToString());
        }

        [Pure]
        public static bool IsValidUrlTemplate(string urlToValidate, int howManyToValidate)
        {
            // Can't have both a string parameter 1 and an int parameter 1
            // Must have them in contiguous order

            var containsOnlyOneOfEachParameterType = true;
            var parametersAreContiguous = true;
            var startLookingForContiguous = false;

            for (var i = howManyToValidate - 1; i >= 0; --i)
            {
                var containIntParam = urlToValidate.Contains(IntParameters[i].ToString());
                var containsStringParam = urlToValidate.Contains(StringParameters[i]);
                containsOnlyOneOfEachParameterType = containsOnlyOneOfEachParameterType && (containIntParam ^ containsStringParam);
                
                var containsThisParamNumber = containIntParam || containsStringParam;
                if (containsThisParamNumber)
                {
                    startLookingForContiguous = true;
                }
                if (startLookingForContiguous)
                {
                    parametersAreContiguous = parametersAreContiguous && containsThisParamNumber;
                }
            }
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            return containsOnlyOneOfEachParameterType && parametersAreContiguous;
// ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        [Pure]
        public static Type[] CalculateTypes(string urlToValidate, int howMany)
        {
            var types = new List<Type>();
            for (var i = 1; i <= howMany; i++)
            {
                types.Add(CalculateType(urlToValidate, i));
            }
            return types.ToArray();
        }

        [Pure]
        public static Type CalculateType(string urlToValidate, int parameterPosition)
        {
            Type currentType;
            var parameterIndex = parameterPosition - 1;
            var containIntParam = urlToValidate.Contains(IntParameters[parameterIndex].ToString());
            var containsStringParam = urlToValidate.Contains(StringParameters[parameterIndex]);
            if (containIntParam)
            {
                currentType = typeof(int);
            }
            else if (containsStringParam)
            {
                currentType = typeof(string);
            }
            else
            {
                throw new ArgumentException("Url doesn't match the types");
            }
            return currentType;
        }

        [Pure]
        public static bool IsRequiredType<TParameter>(Type typeToCheckAgainst)
        {
            return typeof(TParameter) == typeToCheckAgainst;
        }

        [Pure]
        public static bool AllParametersReplaced(string stringToCheck)
        {
            return !IntParameters.Any(intParameter => stringToCheck.Contains(intParameter.ToString())) && !StringParameters.Any(stringToCheck.Contains);
        }

        // All the parameter constants should be kept in sync with javascript class in sitka.js

        public const int Parameter1Int = -2111110001;
        public const int Parameter2Int = -2111110002;
        public const int Parameter3Int = -2111110003;
        public const int Parameter4Int = -2111110004;
        public const int Parameter5Int = -2111110005;
        public const int Parameter6Int = -2111110006;
        public const int Parameter7Int = -2111110007;
        public const int Parameter8Int = -2111110008;
        public const int Parameter9Int = -2111110009;
        public const int Parameter10Int = -2111110010;
        public const int Parameter11Int = -2111110011;
        public const int Parameter12Int = -2111110012;
        public const int Parameter13Int = -2111110013;

        public const string Parameter1String = "UrlTemplateParameter1String";
        public const string Parameter2String = "UrlTemplateParameter2String";
        public const string Parameter3String = "UrlTemplateParameter3String";
        public const string Parameter4String = "UrlTemplateParameter4String";
        public const string Parameter5String = "UrlTemplateParameter5String";
        public const string Parameter6String = "UrlTemplateParameter6String";
        public const string Parameter7String = "UrlTemplateParameter7String";
        public const string Parameter8String = "UrlTemplateParameter8String";
        public const string Parameter9String = "UrlTemplateParameter9String";
        public const string Parameter10String = "UrlTemplateParameter10String";
        public const string Parameter11String = "UrlTemplateParameter11String";
        public const string Parameter12String = "UrlTemplateParameter12String";
        public const string Parameter13String = "UrlTemplateParameter13String";
        
        protected const string TypeErrorMessage = "Must be type integer or string";

        public static readonly string[] StringParameters = new[]
                                                           {
                                                               Parameter1String,
                                                               Parameter2String,
                                                               Parameter3String,
                                                               Parameter4String,
                                                               Parameter5String,
                                                               Parameter6String,
                                                               Parameter7String,
                                                               Parameter8String,
                                                               Parameter9String,
                                                               Parameter10String,
                                                               Parameter11String,
                                                               Parameter12String,
                                                               Parameter13String
                                                           };

        public static readonly int[] IntParameters = new[]
                                                     {
                                                         Parameter1Int,
                                                         Parameter2Int,
                                                         Parameter3Int,
                                                         Parameter4Int,
                                                         Parameter5Int,
                                                         Parameter6Int,
                                                         Parameter7Int,
                                                         Parameter8Int,
                                                         Parameter9Int,
                                                         Parameter10Int,
                                                         Parameter11Int,
                                                         Parameter12Int,
                                                         Parameter13Int
                                                     };
        public static int MaximumParameters
        {
            get { return StringParameters.Length; }
        }

        public static HtmlString MakeHrefString(string url, string linkText)
        {
            return MakeHrefString(url, linkText, null, null);
        }

        public static HtmlString MakeHrefString(string url, string linkText, string titleText)
        {
            return MakeHrefString(url, linkText, titleText, null);
        }

        public static HtmlString MakeHrefString(string url, string linkText, Dictionary<string, string> attributeDict)
        {
            return MakeHrefString(url, linkText, null, attributeDict);
        }

        public static HtmlString MakeHrefString(string url, string linkText, string titleText, Dictionary<string, string> attributeDict)
        {
            return MakeHrefStringImpl(url, HttpUtility.HtmlEncode(linkText), titleText, attributeDict);
        }

        /// <summary>
        /// Same as <see cref="MakeHrefString(string,string,string,System.Collections.Generic.Dictionary{string,string})"/>, except that
        /// <paramref name="linkTextHtml"/> is emitted as-is instead of being HTML encoded. ONLY for callers that build the link
        /// contents out of trusted markup (glyph icons, etc.). If any part of the link text comes from user-entered data, encode
        /// that part yourself before calling this, or use MakeHrefString instead.
        /// </summary>
        public static HtmlString MakeHrefStringWithHtmlLinkText(string url, string linkTextHtml, string titleText, Dictionary<string, string> attributeDict)
        {
            return MakeHrefStringImpl(url, linkTextHtml, titleText, attributeDict);
        }

        public static HtmlString MakeHrefStringWithHtmlLinkText(string url, string linkTextHtml)
        {
            return MakeHrefStringImpl(url, linkTextHtml, null, null);
        }

        public static HtmlString MakeHrefStringWithHtmlLinkText(string url, string linkTextHtml, Dictionary<string, string> attributeDict)
        {
            return MakeHrefStringImpl(url, linkTextHtml, null, attributeDict);
        }

        private static HtmlString MakeHrefStringImpl(string url, string linkTextHtml, string titleText, Dictionary<string, string> attributeDict)
        {
            return new HtmlString(string.Format("<a title=\"{2}\" href=\"{0}\"{3}>{1}</a>", HttpUtility.HtmlAttributeEncode(url), linkTextHtml, HttpUtility.HtmlAttributeEncode(titleText), BuildAttributeString(attributeDict)));
        }

        public static HtmlString MakeHrefStringWithCurrentPage(string url, string linkText, string titleText, Dictionary<string, string> attributeDict)
        {
            return new HtmlString(string.Format("<a aria-current=\"page\" title=\"{2}\" href=\"{0}\"{3}>{1}</a>", HttpUtility.HtmlAttributeEncode(url), HttpUtility.HtmlEncode(linkText), HttpUtility.HtmlAttributeEncode(titleText), BuildAttributeString(attributeDict)));
        }

        private static string BuildAttributeString(Dictionary<string, string> attributeDict)
        {
            if (attributeDict == null || !attributeDict.Any())
            {
                return string.Empty;
            }

            return string.Format(" {0}", attributeDict.Aggregate(" ", (current, attribute) => current + String.Format(" {0}=\"{1}\"", attribute.Key, HttpUtility.HtmlAttributeEncode(attribute.Value))));
        }
    }
    public class LtInfoAnchorHtmlTag : IHtmlString
    {
        public string HRef { get; set; }
        public string Title { get; set; }
        public string InnerText { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> CustomAttributes { get; set; }
        public string ToHtmlString()
        {
            var htmlAnchorTag = new TagBuilder("a");
            if (!String.IsNullOrWhiteSpace(HRef))
            {
                htmlAnchorTag.Attributes.Add("href", Url);
            }
            if (!String.IsNullOrWhiteSpace(Title))
            {
                htmlAnchorTag.Attributes.Add("title", Title);
            }
            if (!String.IsNullOrWhiteSpace(InnerText))
            {
                htmlAnchorTag.SetInnerText(InnerText);
            }
            if (CustomAttributes != null)
            {
                htmlAnchorTag.MergeAttributes(CustomAttributes);
            }
            return htmlAnchorTag.ToString();
        }
    }
}
