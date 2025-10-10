// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;

namespace CoolBytes.ScriptInterpreter.YAXLib;

/// <summary>
/// The base class for all attributes defined in YAXLib.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public abstract class YaxBaseAttribute : Attribute
{
}

/// <summary>
/// Creates a comment node per each line of the comment string provided.
/// This attribute is applicable to classes, structures, fields, and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxCommentAttribute"/> class.
/// </remarks>
/// <param name="comment">The comment.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property)]
public class YaxCommentAttribute(string comment) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    /// <value>The comment.</value>
    public string Comment { get; set; } = comment;

    #endregion
}

/// <summary>
/// Add this attribute to types, structs or classes which you want to override
/// their default serialization behaviour. This attribute is optional.
/// This attribute is applicable to classes and structures.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxSerializableTypeAttribute : YaxBaseAttribute
{
    #region Private Fields

    /// <summary>
    /// determines whether the serialization options property has been explicitly
    /// set by the user.
    /// </summary>
    private bool m_isOptionSet = false;

    /// <summary>
    /// Private variable to hold the serialization options
    /// </summary>
    private YAXSerializationOptions m_serializationOptions = YAXSerializationOptions.SerializeNullObjects;

    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="YaxSerializableTypeAttribute"/> class.
    /// </summary>
    public YaxSerializableTypeAttribute()
    {
        FieldsToSerialize = YAXSerializationFields.PublicProperties;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the fields which YAXLib selects for serialization
    /// </summary>
    /// <value>The fields to serialize.</value>
    public YAXSerializationFields FieldsToSerialize { get; set; }

    /// <summary>
    /// Gets or sets the serialization options.
    /// </summary>
    /// <value>The options.</value>
    public YAXSerializationOptions Options
    {
        get
        {
            return m_serializationOptions;
        }

        set
        {
            m_serializationOptions = value;
            m_isOptionSet = true;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether the serialization options property has been explicitly
    /// set by the user.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if the serialization options property has been explicitly
    /// set by the user; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSerializationOptionSet()
    {
        return m_isOptionSet;
    }

    #endregion
}

/// <summary>
/// Makes an element make use of a specific XML namespace.
/// This attribute is applicable to classes, structs, fields, enums and properties
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct)]
public class YaxNamespaceAttribute : YaxBaseAttribute
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxNamespaceAttribute"/> class.
    /// </summary>
    /// <remarks>
    /// The element this applies to will take on the given XML namespace. In the case
    /// of this constructor, the default one defined by xmlns="namespace"
    /// </remarks>
    /// <param name="defaultNamespace">The default namespace to use for this item</param>
    public YaxNamespaceAttribute(string defaultNamespace)
    {
        Namespace = defaultNamespace;
        Prefix = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxNamespaceAttribute"/> class.
    /// </summary>
    /// <remarks>
    /// The element this applies to will take on the given XML namespace. The namespace
    /// will be added to the root XML element, with the given prefix in the form: 
    ///     xmlns:prefix="namespace"
    /// </remarks>
    /// <param name="namespacePrefix">The prefix to use for this element's namespace</param>
    /// <param name="xmlNamespace">The xml namespace to use for this item</param>
    public YaxNamespaceAttribute(string namespacePrefix, string xmlNamespace)
    {
        Namespace = xmlNamespace;
        Prefix = namespacePrefix;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The namespace path
    /// </summary>
    public string Namespace
    { get; private set; }

    /// <summary>
    /// The xml prefix used for the namespace
    /// </summary>
    public string Prefix
    { get; private set; }

    #endregion

}

/// <summary>
/// Add this attribute to properties or fields which you wish to be serialized, when 
/// the enclosing class uses the <c>YAXSerializableType</c> attribute in which <c>FieldsToSerialize</c>
/// has been set to <c>AttributedFieldsOnly</c>.
/// This attribute is applicable to fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxSerializableFieldAttribute : YaxBaseAttribute
{
}

/// <summary>
/// Makes a property to appear as an attribute for the enclosing class (i.e. the parent element) if possible.
/// This attribute is applicable to fields and properties only.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxAttributeForClassAttribute : YaxBaseAttribute
{
}

/// <summary>
/// Makes a field or property to appear as an attribute for another element, if possible.
/// This attribute is applicable to fields and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxAttributeForAttribute"/> class.
/// </remarks>
/// <param name="parent">The element of which the property becomes an attribute.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxAttributeForAttribute(string parent) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the element of which the property becomes an attribute.
    /// </summary>
    public string Parent { get; set; } = parent;

    #endregion
}

/// <summary>
/// Makes a field or property to appear as a value for another element, if possible.
/// This attribute is applicable to fields and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxAttributeForAttribute"/> class.
/// </remarks>
/// <param name="parent">The element of which the property becomes an attribute.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxContentForAttribute(string parent) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the element for which the property becomes a value.
    /// </summary>
    public string Parent { get; set; } = parent;

    #endregion
}


/// <summary>
/// Makes a field or property to appear as a value for its parent element, if possible.
/// This attribute is applicable to fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxContentForClassAttribute : YaxBaseAttribute
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxContentForClassAttribute"/> class.
    /// </summary>
    public YaxContentForClassAttribute()
    {
    }

    #endregion

}

/// <summary>
/// Specifies the order upon which a field or property is serialized / deserialized.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxElementOrderAttribute"/> class.
/// </remarks>
/// <remarks>
/// The element this applies to will be given priority in being serialized or deserialized
/// depending on the relative value compared to other child elements.
/// </remarks>
/// <param name="order">The priority of the element in serializing and deserializing.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxElementOrderAttribute(int order) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties
    /// <summary>
    /// The order used to prioritize serialization and deserialization.
    /// </summary>
    public int Order { get; private set; } = order;

    #endregion
}

/// <summary>
/// Prevents serialization of some field or property.
/// This attribute is applicable to fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxDontSerializeAttribute : YaxBaseAttribute
{
}

/// <summary>
/// Prevents serialization of fields or properties when their value is null.
/// This attribute is applicable to fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxDontSerializeIfNullAttribute : YaxBaseAttribute
{
}

/// <summary>
/// Defines an alias for the field, property, class, or struct under 
/// which it will be serialized. This attribute is applicable to fields, 
/// properties, classes, and structs.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxSerializeAsAttribute"/> class.
/// </remarks>
/// <param name="serializeAs">the alias for the property under which the property will be serialized.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxSerializeAsAttribute(string serializeAs) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the alias for the property under which the property will be serialized.
    /// </summary>
    public string SerializeAs { get; set; } = serializeAs;

    #endregion
}

/// <summary>
/// Makes a property or field to appear as a child element 
/// for another element. This attribute is applicable to fields and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxElementForAttribute"/> class.
/// </remarks>
/// <param name="parent">The element of which the property becomes a child element.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxElementForAttribute(string parent) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the element of which the property becomes a child element.
    /// </summary>
    /// <value>The element of which the property becomes a child element.</value>
    public string Parent { get; set; } = parent;

    #endregion
}

/// <summary>
/// Controls the serialization of collection instances.
/// This attribute is applicable to fields and properties, and collection classes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxCollectionAttribute"/> class.
/// </remarks>
/// <param name="serType">type of the serialization of the collection.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxCollectionAttribute(YAXCollectionSerializationTypes serType) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the type of the serialization of the collection.
    /// </summary>
    /// <value>The type of the serialization of the collection.</value>
    public YAXCollectionSerializationTypes SerializationType { get; set; } = serType;

    /// <summary>
    /// Gets or sets the string to separate collection items, if the Serialization type is set to <c>Serially</c>.
    /// </summary>
    /// <value>the string to separate collection items, if the Serialization Type is set to <c>Serially</c>.</value>
    public string SeparateBy { get; set; } = " ";

    /// <summary>
    /// Gets or sets the name of each child element corresponding to the collection members, if the Serialization type is set to <c>Recursive</c>.
    /// </summary>
    /// <value>The name of each child element corresponding to the collection members, if the Serialization type is set to <c>Recursive</c>.</value>
    public string EachElementName { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether white space characters are to be
    /// treated as sparators or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if white space separator characters are to be
    /// treated as sparators; otherwise, <c>false</c>.
    /// </value>
    public bool IsWhiteSpaceSeparator { get; set; } = true;

    #endregion
}

/// <summary>
/// Controls the serialization of generic Dictionary instances.
/// This attribute is applicable to fields and properties, and 
/// classes derived from the <c>Dictionary</c> base class.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxDictionaryAttribute : YaxBaseAttribute
{
    private YAXNodeTypes _serializeKeyAs = YAXNodeTypes.Element;
    private YAXNodeTypes _serializeValueAs = YAXNodeTypes.Element;

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxDictionaryAttribute"/> class.
    /// </summary>
    public YaxDictionaryAttribute()
    {
        KeyName = null;
        ValueName = null;
        EachPairName = null;
        KeyFormatString = null;
        ValueFormatString = null;
    }

    /// <summary>
    /// Gets or sets the alias for the key part of the dicitonary.
    /// </summary>
    /// <value></value>
    public string KeyName { get; set; }

    /// <summary>
    /// Gets or sets alias for the value part of the dicitonary.
    /// </summary>
    /// <value></value>
    public string ValueName { get; set; }

    /// <summary>
    /// Gets or sets alias for the element containing the Key-Value pair.
    /// </summary>
    /// <value></value>
    public string EachPairName { get; set; }

    /// <summary>
    /// Gets or sets the node type according to which the key part of the dictionary is serialized.
    /// </summary>
    /// <value></value>
    public YAXNodeTypes SerializeKeyAs
    {
        get
        {
            return _serializeKeyAs;
        }

        set
        {
            _serializeKeyAs = value;
            CheckIntegrity();
        }
    }

    /// <summary>
    /// Gets or sets the node type according to which the value part of the dictionary is serialized.
    /// </summary>
    /// <value></value>
    public YAXNodeTypes SerializeValueAs
    {
        get
        {
            return _serializeValueAs;
        }

        set
        {
            _serializeValueAs = value;
            CheckIntegrity();
        }
    }

    /// <summary>
    /// Gets or sets the key format string.
    /// </summary>
    /// <value></value>
    public string KeyFormatString { get; set; }

    /// <summary>
    /// Gets or sets the value format string.
    /// </summary>
    /// <value></value>
    public string ValueFormatString { get; set; }

    private void CheckIntegrity()
    {
        if (_serializeKeyAs == _serializeValueAs && _serializeValueAs == YAXNodeTypes.Content)
        {
            throw new YaxException("Key and Value cannot both be serialized as Content at the same time.");
        }
    }
}

/// <summary>
/// Specifies the behavior of the deserialization method, if the element/attribute corresponding to this property is missed in the XML input.
/// This attribute is applicable to fields and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxErrorIfMissedAttribute"/> class.
/// </remarks>
/// <param name="treatAs">The value indicating this situation is going to be treated as Error or Warning.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxErrorIfMissedAttribute(YAXExceptionTypes treatAs) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the value indicating this situation is going to be treated as Error or Warning.
    /// </summary>
    /// <value>The value indicating this situation is going to be treated as Error or Warning.</value>
    public YAXExceptionTypes TreatAs { get; set; } = treatAs;

    /// <summary>
    /// Gets or sets the default value for the property if the element/attribute corresponding to this property is missed in the XML input.
    /// Setting <c>null</c> means do nothing.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue { get; set; } = null;

    #endregion
}

/// <summary>
/// Specifies the format string provided for serializing data. The format string is the parameter 
/// passed to the <c>ToString</c> method.
/// If this attribute is applied to collection classes, the format, therefore, is applied to 
/// the collection members.
/// This attribute is applicable to fields and properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxFormatAttribute"/> class.
/// </remarks>
/// <param name="format">The format string.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxFormatAttribute(string format) : YaxBaseAttribute
{

    #region Constructors
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the format string needed to serialize data. The format string is the parameter 
    /// passed to the <c>ToString</c> method.
    /// </summary>
    /// <value></value>
    public string Format { get; set; } = format;

    #endregion
}

/// <summary>
/// Specifies that a particular class, or a particular property or variable type, that is 
/// driven from <c>IEnumerable</c> should not be treated as a collection class/object.
/// This attribute is applicable to fields, properties, classes, and structs.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxNotCollectionAttribute : YaxBaseAttribute
{
}

/// <summary>
/// Specifies an alias for an enum member.
/// This attribute is applicable to enum members.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxEnumAttribute"/> class.
/// </remarks>
/// <param name="alias">The alias.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YaxEnumAttribute(string alias) : YaxBaseAttribute
{

    #region Constructor
    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the alias for the enum member.
    /// </summary>
    /// <value>The alias for the enum member.</value>
    public string Alias { get; private set; } = alias.Trim();

    #endregion
}

/// <summary>
/// Specifies a custom serializer class for a field, property, class, or struct. YAXLib will instantiate an object
/// from the specified type in this attribute, and calls appropriate methods while serializing.
/// This attribute is applicable to fields, properties, classes, and structs.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="YaxCustomSerializerAttribute"/> class.
/// </remarks>
/// <param name="customSerializerType">Type of the custom serializer.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxCustomSerializerAttribute(Type customSerializerType) : YaxBaseAttribute
{
    /// <summary>
    /// Gets or sets the type of the custom serializer.
    /// </summary>
    /// <value>The type of the custom serializer.</value>
    public Type CustomSerializerType { get; private set; } = customSerializerType;
}

/// <summary>
/// Adds the attribute xml:space="preserve" to the serialized element, so that the deserializer would
/// perserve all whitespace characters for the string values.
/// Add this attribute to any string field that you want their whitespace be preserved during 
/// deserialization, or add it to the containing class to be applied to all its fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class YaxPreserveWhitespaceAttribute : YaxBaseAttribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal class YaxCollectionItemTypeAttribute(Type type) : YaxBaseAttribute
{
    public Type Type { get; private set; } = type;

    public string Alias { get; set; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal class YaxTypeAttribute(Type type) : YaxBaseAttribute
{
    public Type Type { get; private set; } = type;

    public string Alias { get; set; }
}
