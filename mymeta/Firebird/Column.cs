using System;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace MyMeta.Firebird
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IColumn))]
#endif 
	public class FirebirdColumn : Column
	{
		public FirebirdColumn()
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
				if(null != this.Table)
				{
					if(this.Table.Properties.ContainsKey("GEN:I:" + this.Name) ||
					   this.Table.Properties.ContainsKey("GEN:I:T:" + this.Name))
					{
						return true;
					}
				}

				return false;
			}
		}

		override public System.Int32 CharacterMaxLength
		{
			get
			{
				switch(DataTypeName)
				{
					case "VARCHAR":
					case "CHAR":
//						return (int)this._row["COLUMN_SIZE"];
						return this.CharacterOctetLength;

					default:
						return this.GetInt32(Columns.f_MaxLength);
				}
			}
		}

		override public System.Int32 NumericPrecision
		{
			get
			{
				if(this.DataTypeName == "NUMERIC")
				{
					switch((int)this._row["COLUMN_SIZE"])
					{
						case 2:
							return 4;
						case 4:
							return 9;
						case 8:
							return 15;
						default:
							return 18;
					}
				}
				return this.GetInt32(Columns.f_NumericScale);
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

				FirebirdColumns cols = Columns as FirebirdColumns;
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

				FirebirdColumns cols = Columns as FirebirdColumns;
				return this.GetString(cols.f_TypeNameComplete);
			}
		}
	}
}
