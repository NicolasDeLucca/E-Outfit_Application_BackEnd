namespace Blogs.Interfaces
{
    public interface IImporterService<Entity, ParameterType>
    {
        List<IImporter<Entity, ParameterType>> GetImporters();
        List<Entity> Import(string importerName, List<IParameter<ParameterType>> parameters);
    }

    public interface IImporter<Entity, ParameterType>
    {
        string GetName();
        List<IParameter<ParameterType>> GetParameters();
        List<Entity> Import(List<IParameter<ParameterType>> parameters);
    }

    public interface IParameter<Type>
    {
        string Name();
        object Value();
        bool IsRequired();
        Type ParameterType();
    }
}
