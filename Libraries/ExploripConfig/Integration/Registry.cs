namespace ExploripConfig.Integration;

internal class Registry : IConfig
{
    public T ReadParam<T>(string paramName, string sectionName)
    {
        throw new System.NotImplementedException();
    }

    public T ReadParam<T>(string paramName, ESectionName sectionName)
    {
        throw new System.NotImplementedException();
    }

    public bool WriteParam(string paramName, object value)
    {
        throw new System.NotImplementedException();
    }

    public bool WriteParam(string paramName, string sectionName, object value)
    {
        throw new System.NotImplementedException();
    }

    public bool WriteParam(string paramName, ESectionName sectionName, object value)
    {
        throw new System.NotImplementedException();
    }
}
