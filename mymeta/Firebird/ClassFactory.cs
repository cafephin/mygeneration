using System;

using MyMeta;

namespace MyMeta.Firebird
{
#if ENTERPRISE
	using System.EnterpriseServices;
	using System.Runtime.InteropServices;
	[ComVisible(false)]
#endif
	public class ClassFactory : IClassFactory
	{
		public ClassFactory()
		{

		}

		public ITables CreateTables()
		{
			return new Firebird.FirebirdTables();
		}

		public ITable CreateTable()
		{
			return new Firebird.FirebirdTable();
		}

		public IColumn CreateColumn()
		{
			return new Firebird.FirebirdColumn();
		}

		public IColumns CreateColumns()
		{
			return new Firebird.FirebirdColumns();
		}

		public IDatabase CreateDatabase()
		{
			return new Firebird.FirebirdDatabase();
		}

		public IDatabases CreateDatabases()
		{
			return new Firebird.FirebirdDatabases();
		}

		public IProcedure CreateProcedure()
		{
			return new Firebird.FirebirdProcedure();
		}

		public IProcedures CreateProcedures()
		{
			return new Firebird.FirebirdProcedures();
		}

		public IView CreateView()
		{
			return new Firebird.FirebirdView();
		}

		public IViews CreateViews()
		{
			return new Firebird.FirebirdViews();
		}

		public IParameter CreateParameter()
		{
			return new Firebird.FirebirdParameter();
		}

		public IParameters CreateParameters()
		{
			return new Firebird.FirebirdParameters();
		}

		public IForeignKey  CreateForeignKey()
		{
			return new Firebird.FirebirdForeignKey();
		}

		public IForeignKeys CreateForeignKeys()
		{
			return new Firebird.FirebirdForeignKeys();
		}

		public IIndex CreateIndex()
		{
			return new Firebird.FirebirdIndex();
		}

		public IIndexes CreateIndexes()
		{
			return new Firebird.FirebirdIndexes();
		}

		public IResultColumn CreateResultColumn()
		{
			return new Firebird.FirebirdResultColumn();
		}

		public IResultColumns CreateResultColumns()
		{
			return new Firebird.FirebirdResultColumns();
		}

		public IDomain CreateDomain()
		{
			return new Firebird.FirebirdDomain();
		}

		public IDomains CreateDomains()
		{
			return new Firebird.FirebirdDomains();
		}

		public IProviderType CreateProviderType()
		{
			return new ProviderType();
		}

		public IProviderTypes CreateProviderTypes()
		{
			return new ProviderTypes();
		}
	}
}
