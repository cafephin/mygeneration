using System;
using System.Data;
using System.Data.OleDb;

namespace MyMeta.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IColumn))]
#endif 
	public class SqlColumn : Column
	{
		public SqlColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

		override public System.Boolean IsAutoKey
		{
			get
			{
				SqlColumns cols = Columns as SqlColumns;
				return this.GetBool(cols.f_AutoKey);
			}
		}

		override public System.Boolean IsComputed
		{
			get
			{
				if(this.DataTypeName == "timestamp") return true;

				return this.GetBool(Columns.f_IsComputed);
			}
		}


		override public string DataTypeName
		{
			get
			{
				if(this.dbRoot.DomainOverride)
				{
					if(this.HasDomain)
					{
						if(this.Domain != null)
						{
							return this.Domain.DataTypeName;
						}
					}
				}

				SqlColumns cols = Columns as SqlColumns;
				return this.GetString(cols.f_TypeName);
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				if(this.dbRoot.DomainOverride)
				{
					if(this.HasDomain)
					{
						if(this.Domain != null)
						{
							return this.Domain.DataTypeNameComplete;
						}
					}
				}

				switch(this.DataTypeName)
				{
					case "binary":
					case "char":
					case "nchar":
					case "nvarchar":
					case "varchar":
					case "varbinary":

						return this.DataTypeName + "(" + this.CharacterMaxLength + ")";

					case "decimal":
					case "numeric":

						return this.DataTypeName + "(" + this.NumericPrecision + "," + this.NumericScale + ")";

					default:

						return this.DataTypeName;
				}
			}
		}

		public override object DatabaseSpecificMetaData(string key)
		{
			return SqlDatabase.DBSpecific(key, this);
		}
	}
}
