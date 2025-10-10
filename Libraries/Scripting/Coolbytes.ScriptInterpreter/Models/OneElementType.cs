namespace CoolBytes.ScriptInterpreter.Models;

public class OneElementType
{
    private readonly string _name;
    private readonly ElementTypes _elementType;
    private readonly string _parameterType;
    private readonly bool _optional;
    private readonly string _defaultValue;

    internal OneElementType(string elementName)
    {
        _name = elementName;
    }

    public OneElementType(string elementName, ElementTypes elementType) : this(elementName)
    {
        _elementType = elementType;
    }

    public OneElementType(string elementName, string parameterType) : this(elementName)
    {
        _parameterType = parameterType;
        if (_parameterType.EndsWith("&"))
            _parameterType = "ref/out " + _parameterType.Substring(0, _parameterType.Length - 1);
        _elementType = ElementTypes.PARAMETER;
    }

    public OneElementType(string elementName, string parameterType, bool optional, string defaultValue) : this(elementName, parameterType)
    {
        _optional = optional;
        _defaultValue = defaultValue;
    }

    public override string ToString()
    {
        if (_elementType == ElementTypes.PARAMETER)
            return (_optional ? "(" : "") + _parameterType + " " + _name + (_optional ? ")" : "");
        else
            return _name;
    }

    public string Name
    {
        get { return _name; }
    }

    public ElementTypes ElementType
    {
        get { return _elementType; }
    }

    public string ParameterType
    {
        get { return _parameterType; }
    }

    public string DefaultValue
    {
        get { return _defaultValue; }
    }

    public bool OptionalParameter
    {
        get { return _optional; }
    }
}
