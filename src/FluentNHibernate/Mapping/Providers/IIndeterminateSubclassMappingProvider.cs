using System;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Mapping.Providers
{
    public interface IIndeterminateSubclassMappingProvider : IProvider
    {
        SubclassMapping GetSubclassMapping(SubclassMapping mapping);
        Type EntityType { get; }
        Type Extends { get; }
    }
}