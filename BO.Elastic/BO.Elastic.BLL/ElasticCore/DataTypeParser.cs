using System;
using System.Collections.Generic;
using System.Linq;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.BLL.ElasticCore
{
    public static class DataTypeParser
    {
        public static List<EElasticDataTypes> GetPossibleElasticDataTypes(EDBDataType edbDataType)
        {
            switch (edbDataType)
            {
                case EDBDataType.Bigint:
                case EDBDataType.Numeric:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Long};
                case EDBDataType.Bit:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Boolean};
                case EDBDataType.Decimal:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Double};
                case EDBDataType.Int:
                case EDBDataType.Tinyint:
                case EDBDataType.Smallint:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Integer};
                case EDBDataType.Money:
                case EDBDataType.Smallmoney:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Double};
                case EDBDataType.Float:
                case EDBDataType.Real:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Float};
                case EDBDataType.Date:
                case EDBDataType.Datetime:
                    return new List<EElasticDataTypes>() {EElasticDataTypes.Date, EElasticDataTypes.Long};
                case EDBDataType.NVarchar:
                case EDBDataType.Nchar:
                case EDBDataType.NText:
                case EDBDataType.Char:
                case EDBDataType.Varchar:
                case EDBDataType.Text:
                    return new List<EElasticDataTypes>()
                    {
                        EElasticDataTypes.Text, EElasticDataTypes.Keyword, EElasticDataTypes.IP,
                        EElasticDataTypes.Completion
                    };
                default: return new List<EElasticDataTypes>();
            }
        }

        public static EDBDataType GetDBDateTypeFromString(string dateType)
        {
            dateType = dateType.ToLower();
            switch (dateType)
            {
                case "bigint":
                    return EDBDataType.Bigint;
                case "numeric":
                    return EDBDataType.Numeric;
                case "bit":
                    return EDBDataType.Bit;
                case "decimal":
                    return EDBDataType.Decimal;
                case "int":
                    return EDBDataType.Int;
                case "tinyint":
                    return EDBDataType.Tinyint;
                case "smallint":
                    return EDBDataType.Smallint;
                case "money":
                    return EDBDataType.Money;
                case "smallmoney":
                    return EDBDataType.Smallmoney;
                case "float":
                    return EDBDataType.Float;
                case "real":
                    return EDBDataType.Real;
                case "date":
                    return EDBDataType.Date;
                case "datetime":
                    return EDBDataType.Datetime;
                case "nchar":
                    return EDBDataType.Nchar;
                case "nvarchar":
                    return EDBDataType.NVarchar;
                case "ntext":
                    return EDBDataType.NText;
                case "char":
                    return EDBDataType.Char;
                case "varchar":
                    return EDBDataType.Varchar;
                case "text":
                    return EDBDataType.Text;
                default: return EDBDataType.Undefined;
            }
        }
    }
}