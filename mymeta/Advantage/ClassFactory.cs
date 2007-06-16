using System;

using MyMeta;

namespace MyMeta.Advantage
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
			return new Advantage.AdvantageTables();
		}

		public ITable CreateTable()
		{
			return new Advantage.AdvantageTable();
		}

		public IColumn CreateColumn()
		{
			return new Advantage.AdvantageColumn();
		}

		public IColumns CreateColumns()
		{
			return new Advantage.AdvantageColumns();
		}

		public IDatabase CreateDatabase()
		{
			return new Advantage.AdvantageDatabase();
		}

		public IDatabases CreateDatabases()
		{
			return new Advantage.AdvantageDatabases();
		}

		public IProcedure CreateProcedure()
		{
			return new Advantage.AdvantageProcedure();
		}

		public IProcedures CreateProcedures()
		{
			return new Advantage.AdvantageProcedures();
		}

		public IView CreateView()
		{
			return new Advantage.AdvantageView();
		}

		public IViews CreateViews()
		{
			return new Advantage.AdvantageViews();
		}

		public IParameter CreateParameter()
		{
			return new Advantage.AdvantageParameter();
		}

		public IParameters CreateParameters()
		{
			return new Advantage.AdvantageParameters();
		}

		public IForeignKey  CreateForeignKey()
		{
			return new Advantage.AdvantageForeignKey();
		}

		public IForeignKeys CreateForeignKeys()
		{
			return new Advantage.AdvantageForeignKeys();
		}

		public IIndex CreateIndex()
		{
			return new Advantage.AdvantageIndex();
		}

		public IIndexes CreateIndexes()
		{
			return new Advantage.AdvantageIndexes();
		}

		public IResultColumn CreateResultColumn()
		{
			return new Advantage.AdvantageResultColumn();
		}

		public IResultColumns CreateResultColumns()
		{
			return new Advantage.AdvantageResultColumns();
		}

		public IDomain CreateDomain()
		{
			return new Advantage.AdvantageDomain();
		}

		public IDomains CreateDomains()
		{
			return new Advantage.AdvantageDomains();
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
