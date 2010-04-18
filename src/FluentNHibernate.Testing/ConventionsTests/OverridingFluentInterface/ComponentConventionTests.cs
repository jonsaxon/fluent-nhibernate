using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers.Builders;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Testing.DomainModel.Mapping;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionsTests.OverridingFluentInterface
{
    [TestFixture]
    public class ComponentConventionTests
    {
        private IProvider mapping;
        private Type mappingType;
        ConventionsCollection conventions;

        [SetUp]
        public void CreatePersistenceModel()
        {
            conventions = new ConventionsCollection();
        }

        [Test]
        public void AccessShouldntBeOverwritten()
        {
            Mapping(x => x.Access.Field());

            Convention(x => x.Access.Property());

            VerifyModel(x => x.Access.ShouldEqual("field"));
        }

        [Test]
        public void InsertShouldntBeOverwritten()
        {
            Mapping(x => x.Insert());

            Convention(x => x.Not.Insert());

            VerifyModel(x => x.Insert.ShouldBeTrue());
        }

        [Test]
        public void UpdateShouldntBeOverwritten()
        {
            Mapping(x => x.Update());

            Convention(x => x.Not.Update());

            VerifyModel(x => x.Update.ShouldBeTrue());
        }

        [Test]
        public void UniqueShouldntBeOverwritten()
        {
            Mapping(x => x.Unique());

            Convention(x => x.Not.Unique());

            VerifyModel(x => x.Unique.ShouldBeTrue());
        }

        [Test]
        public void OptimisticLockShouldntBeOverwritten()
        {
            Mapping(x => x.OptimisticLock());

            Convention(x => x.Not.OptimisticLock());

            VerifyModel(x => x.OptimisticLock.ShouldBeTrue());
        }

        #region Helpers

        private void Convention(Action<IComponentInstance> convention)
        {
            conventions.Add(new ComponentConventionBuilder().Always(convention));
        }

        private void Mapping(Action<ComponentPart<ComponentTarget>> mappingDefinition)
        {
            var classMap = new ClassMap<PropertyTarget>();
            classMap.Id(x => x.Id);
            var map = classMap.Component(x => x.Component, mappingDefinition);

            mappingDefinition(map);

            mapping = classMap;
            mappingType = typeof(PropertyTarget);
        }

        private void VerifyModel(Action<ComponentMapping> modelVerification)
        {
            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(mapping));
            instructions.UseConventions(conventions);

            var generatedModels = instructions.BuildMappings();
            var modelInstance = (ComponentMapping)generatedModels
                .First(x => x.Classes.FirstOrDefault(c => c.Type == mappingType) != null)
                .Classes.First()
                .Components.Where(x => x is ComponentMapping).First();

            modelVerification(modelInstance);
        }

        #endregion
    }
}