namespace ExploripConfig.Integration;

public interface IConfig
{
    T ReadParam<T>(string paramName, string sectionName);

    T ReadParam<T>(string paramName, ESectionName sectionName);

    bool WriteParam(string paramName, object value);

    bool WriteParam(string paramName, string sectionName, object value);

    bool WriteParam(string paramName, ESectionName sectionName, object value);
}
