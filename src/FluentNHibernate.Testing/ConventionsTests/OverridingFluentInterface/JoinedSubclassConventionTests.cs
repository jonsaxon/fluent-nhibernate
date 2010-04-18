using System;
using System.Linq;
using FluentNHibernate.Automapping.TestFixtures;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers.Builders;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Testing.FluentInterfaceTests;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionsTests.OverridingFluentInterface
{
    [TestFixture]
    public class JoinedSubclassConventionTests
    {
        private IProvider mapping;
        private Type mappingType;
        SubclassMap<ExampleInheritedClass> subclassMap;
        ConventionsCollection conventions;

        [SetUp]
        public void CreatePersistenceModel()
        {
            conventions = new ConventionsCollection();
        }

        [Test]
        public void AbstractShouldntBeOverwritten()
        {
            Mapping(x => x.Abstract());

            Convention(x => x.Not.Abstract());

            VerifyModel(x => x.Abstract.ShouldBeTrue());
        }

        [Test]
        public void CheckConstraintShouldntBeOverwritten()
        {
            Mapping(x => x.Check("const"));

            Convention(x => x.Check("xxx"));

            VerifyModel(x => x.Check.ShouldEqual("const"));
        }

        [Test]
        public void DynamicInsertShouldntBeOverwritten()
        {
            Mapping(x => x.DynamicInsert());

            Convention(x => x.Not.DynamicInsert());

            VerifyModel(x => x.DynamicInsert.ShouldBeTrue());
        }

        [Test]
        public void DynamicUpdateShouldntBeOverwritten()
        {
            Mapping(x => x.DynamicUpdate());

            Convention(x => x.Not.DynamicUpdate());

            VerifyModel(x => x.DynamicUpdate.ShouldBeTrue());
        }

        [Test]
        public void LazyLoadShouldntBeOverwritten()
        {
            Mapping(x => x.LazyLoad());

            Convention(x => x.Not.LazyLoad());

            VerifyModel(x => x.Lazy.ShouldEqual(true));
        }

        [Test]
        public void ProxyShouldntBeOverwritten()
        {
            Mapping(x => x.Proxy(typeof(int)));

            Convention(x => x.Proxy(typeof(string)));

            VerifyModel(x => x.Proxy.ShouldEqual(typeof(int).AssemblyQualifiedName));
        }

        [Test]
        public void SchemaShouldntBeOverwritten()
        {
            Mapping(x => x.Schema("dbo"));

            Convention(x => x.Schema("xxx"));

            VerifyModel(x => x.Schema.ShouldEqual("dbo"));
        }

        [Test]
        public void SelectBeforeUpdateShouldntBeOverwritten()
        {
            Mapping(x => x.SelectBeforeUpdate());

            Convention(x => x.Not.SelectBeforeUpdate());

            VerifyModel(x => x.SelectBeforeUpdate.ShouldBeTrue());
        }

        [Test]
        public void TableNameShouldntBeOverwritten()
        {
            Mapping(x => x.Table("table"));

            Convention(x => x.Table("value"));

            VerifyModel(x => x.TableName.ShouldEqual("table"));
        }

        [Test]
        public void SubselectShouldntBeOverwritten()
        {
            Mapping(x => x.Subselect("select"));

            Convention(x => x.Subselect("xxx"));

            VerifyModel(x => x.Subselect.ShouldEqual("select"));
        }

        [Test]
        public void PersisterShouldntBeOverwritten()
        {
            Mapping(x => x.Persister<CustomPersister>());

            Convention(x => x.Persister<SecondCustomPersister>());

            VerifyModel(x => x.Persister.GetUnderlyingSystemType().ShouldEqual(typeof(CustomPersister)));
        }

        [Test]
        public void BatchSizeShouldntBeOverwritten()
        {
            Mapping(x => x.BatchSize(10));

            Convention(x => x.BatchSize(100));

            VerifyModel(x => x.BatchSize.ShouldEqual(10));
        }

        #region Helpers

        private void Convention(Action<IJoinedSubclassInstance> convention)
        {
            conventions.Add(new JoinedSubclassConventionBuilder().Always(convention));
        }

        private void Mapping(Action<SubclassMap<ExampleInheritedClass>> mappingDefinition)
        {
            var classMap = new ClassMap<ExampleClass>();
            classMap.Id(x => x.Id);
            subclassMap = new SubclassMap<ExampleInheritedClass>();

            mappingDefinition(subclassMap);

            mapping = classMap;
            mappingType = typeof(ExampleClass);
        }

        private void VerifyModel(Action<SubclassMapping> modelVerification)
        {
            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(mapping, subclassMap));
            instructions.UseConventions(conventions);

            var generatedModels = instructions.BuildMappings();
            var modelInstance = generatedModels
                .First(x => x.Classes.FirstOrDefault(c => c.Type == mappingType) != null)
                .Classes.First()
                .Subclasses.First();

            modelVerification(modelInstance);
        }

        #endregion
    }
}