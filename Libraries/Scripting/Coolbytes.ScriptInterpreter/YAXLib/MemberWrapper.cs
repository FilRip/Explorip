// Copyright 2009 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace CoolBytes.ScriptInterpreter.YAXLib;

/// <summary>
/// A wrapper class for members which only can be properties or member variables
/// </summary>
internal class MemberWrapper
{
    /// <summary>
    /// reference to the underlying <c>MemberInfo</c> from which this instance is built
    /// </summary>
    private readonly MemberInfo m_memberInfo = null;

    /// <summary>
    /// the member type of the underlying member
    /// </summary>
    private readonly Type m_memberType;

    /// <summary>
    /// a type wrapper around the underlying member type
    /// </summary>
    private readonly UdtWrapper m_memberTypeWrapper = null;

    /// <summary>
    /// the <c>PropertyInfo</c> instance, if this member corrsponds to a property, <c>null</c> otherwise
    /// </summary>
    private readonly PropertyInfo m_propertyInfoInstance = null;

    /// <summary>
    /// the <c>FieldInfo</c> instance, if this member corrsponds to a field, <c>null</c> otherwise
    /// </summary>
    private readonly FieldInfo m_fieldInfoInstance = null;

    /// <summary>
    /// The collection attribute instance
    /// </summary>
    private YaxCollectionAttribute m_collectionAttributeInstance = null;

    /// <summary>
    /// the dictionary attribute instance
    /// </summary>
    private YaxDictionaryAttribute m_dictionaryAttributeInstance = null;

    /// <summary>
    /// <c>true</c> if this instance corresponds to a property, <c>false</c> 
    /// if it corrsponds to a field (i.e., a member variable)
    /// </summary>
    private readonly bool m_isProperty = false;
    /// <summary>
    /// The location of the serialization
    /// </summary>
    private string m_serializationLocation = "";

    /// <summary>
    /// The alias specified by the user
    /// </summary>
    private XName m_alias = null;

    /// <summary>
    /// The xml-namespace this member is going to be serialized under.
    /// </summary>
    private XNamespace m_namespace = XNamespace.None;

    /// <summary>
    /// specifies whether this member is going to be serialized as an attribute
    /// </summary>
    private bool m_isSerializedAsAttribute = false;

    /// <summary>
    /// specifies whether this member is going to be serialized as a value for another element
    /// </summary>
    private bool m_isSerializedAsValue = false;

    private readonly List<YaxTypeAttribute> m_possibleRealTypes = [];

    private readonly List<YaxCollectionItemTypeAttribute> m_possibleCollectionItemRealTypes = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberWrapper"/> class.
    /// </summary>
    /// <param name="memberInfo">The member-info to build this instance from.</param>
    /// <param name="callerSerializer">The caller serializer.</param>
    public MemberWrapper(MemberInfo memberInfo, YaxSerializer callerSerializer)
    {
        Order = int.MaxValue;

        if (!(memberInfo.MemberType == MemberTypes.Property || memberInfo.MemberType == MemberTypes.Field))
            throw new YaxException("Member must be either property or field");

        m_memberInfo = memberInfo;
        m_isProperty = memberInfo.MemberType == MemberTypes.Property;

        Alias = StringUtils.RefineSingleElement(m_memberInfo.Name);

        if (m_isProperty)
            m_propertyInfoInstance = (PropertyInfo)memberInfo;
        else
            m_fieldInfoInstance = (FieldInfo)memberInfo;

        m_memberType = m_isProperty ? m_propertyInfoInstance.PropertyType : m_fieldInfoInstance.FieldType;

        m_memberTypeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(MemberType, callerSerializer);
        if (m_memberTypeWrapper.HasNamespace)
        {
            Namespace = m_memberTypeWrapper.Namespace;
            NamespacePrefix = m_memberTypeWrapper.NamespacePrefix;
        }

        InitInstance();

        TreatErrorsAs = callerSerializer != null ? callerSerializer.DefaultExceptionType : YAXExceptionTypes.Error;

        // discovver YAXCustomSerializerAttributes earlier, because some other attributes depend on it
        HashSet<Type> attrsToProcessEarlier = [typeof(YaxCustomSerializerAttribute), typeof(YaxCollectionAttribute)];
        foreach (Type attrType in attrsToProcessEarlier)
        {
            Attribute[] customSerAttrs = Attribute.GetCustomAttributes(m_memberInfo, attrType, true);
            foreach (Attribute attr in customSerAttrs)
            {
                ProcessYaxAttribute(attr);
            }
        }
        foreach (Attribute attr in Attribute.GetCustomAttributes(m_memberInfo, true))
        {
            // no need to preces, it has been proccessed earlier
            if (attrsToProcessEarlier.Contains(attr.GetType()))
                continue;

            if (attr is YaxBaseAttribute)
                ProcessYaxAttribute(attr);
        }

        // now override some values from memeber-type-wrapper into member-wrapper
        // if member-type has collection attributes while the member itself does not have them, 
        // then use those of the member-type
        if (m_collectionAttributeInstance == null && m_memberTypeWrapper.CollectionAttributeInstance != null)
            m_collectionAttributeInstance = m_memberTypeWrapper.CollectionAttributeInstance;
        m_memberInfo.GetCustomAttributes(true);

        if (m_dictionaryAttributeInstance == null && m_memberTypeWrapper.DictionaryAttributeInstance != null)
            m_dictionaryAttributeInstance = m_memberTypeWrapper.DictionaryAttributeInstance;
    }

    /// <summary>
    /// Gets the alias specified for this member.
    /// </summary>
    /// <value>The alias specified for this member.</value>
    public XName Alias
    {
        get
        {
            return m_alias;
        }

        private set
        {
            if (Namespace.IsEmpty())
                m_alias = Namespace + value.LocalName;
            else
            {
                m_alias = value;
                if (m_alias.Namespace.IsEmpty())
                    m_namespace = m_alias.Namespace;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the member corrsponding to this instance can be read from.
    /// </summary>
    /// <value><c>true</c> if the member corrsponding to this instance can be read from; otherwise, <c>false</c>.</value>
    public bool CanRead
    {
        get
        {
            if (m_isProperty)
            {
                return m_propertyInfoInstance.CanRead;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the member corrsponding to this instance can be written to.
    /// </summary>
    /// <value><c>true</c> if the member corrsponding to this instance can be written to; otherwise, <c>false</c>.</value>
    public bool CanWrite
    {
        get
        {
            if (m_isProperty)
            {
                return m_propertyInfoInstance.CanWrite;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Gets an array of comment lines.
    /// </summary>
    /// <value>The comment lines.</value>
    public string[] Comment { get; private set; }

    /// <summary>
    /// Gets the default value for this instance.
    /// </summary>
    /// <value>The default value for this instance.</value>
    public object DefaultValue { get; private set; }

    /// <summary>
    /// Gets the format specified for this value; <c>null</c> if no format is specified.
    /// </summary>
    /// <value>the format specified for this value; <c>null</c> if no format is specified.</value>
    public string Format { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance has comments.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has comments; otherwise, <c>false</c>.
    /// </value>
    public bool HasComment
    {
        get
        {
            return Comment != null && Comment.Length > 0;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has format values specified
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has format values specified; otherwise, <c>false</c>.
    /// </value>
    public bool HasFormat
    {
        get
        {
            // empty string may be considered as a valid format
            return Format != null;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is attributed as dont serialize.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is attributed as dont serialize; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttributedAsDontSerialize { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is attributed as not-collection.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is attributed as not-collection; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttributedAsNotCollection { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is attributed as serializable.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is attributed as serializable; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttributedAsSerializable { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is attributed as dont serialize when null.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is attributed as dont serialize when null; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttributedAsDontSerializeIfNull { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is serialized as an XML attribute.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is serialized as an XML attribute; otherwise, <c>false</c>.
    /// </value>
    public bool IsSerializedAsAttribute
    {
        get
        {
            return m_isSerializedAsAttribute;
        }

        private set
        {
            m_isSerializedAsAttribute = value;
            if (value)
            {
                // a field cannot be both serialized as an attribute and a value
                m_isSerializedAsValue = false;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is serialized as a value for an element.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is serialized as a value for an element; otherwise, <c>false</c>.
    /// </value>
    public bool IsSerializedAsValue
    {
        get
        {
            return m_isSerializedAsValue;
        }

        private set
        {
            m_isSerializedAsValue = value;
            if (value)
            {
                // a field cannot be both serialized as an attribute and a value
                m_isSerializedAsAttribute = false;
            }
        }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is serialized as an XML element.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is serialized as an XML element; otherwise, <c>false</c>.
    /// </value>
    public bool IsSerializedAsElement
    {
        get
        {
            return !IsSerializedAsAttribute && !IsSerializedAsValue;
        }

        private set
        {
            if (value)
            {
                m_isSerializedAsAttribute = false;
                m_isSerializedAsValue = false;
            }
        }
    }

    /// <summary>
    /// Gets the type of the member.
    /// </summary>
    /// <value>The type of the member.</value>
    public Type MemberType
    {
        get
        {
            return m_memberType;
        }
    }

    /// <summary>
    /// Gets the type wrapper instance corrsponding to the member-type of this instance.
    /// </summary>
    /// <value>The type wrapper instance corrsponding to the member-type of this instance.</value>
    public UdtWrapper MemberTypeWrapper
    {
        get
        {
            return m_memberTypeWrapper;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the underlying type is a known-type
    /// </summary>
    public bool IsKnownType
    {
        get { return m_memberTypeWrapper.IsKnownType; }
    }

    /// <summary>
    /// Gets the original of this member (as opposed to its alias).
    /// </summary>
    /// <value>The original of this member .</value>
    public string OriginalName
    {
        get
        {
            return m_memberInfo.Name;
        }
    }

    /// <summary>
    /// Gets the serialization location.
    /// </summary>
    /// <value>The serialization location.</value>
    public string SerializationLocation
    {
        get
        {
            return m_serializationLocation;
        }

        private set
        {
            m_serializationLocation = StringUtils.RefineLocationString(value);
        }
    }

    /// <summary>
    /// Gets the exception type for this instance in case of encountering missing values
    /// </summary>
    /// <value>The exception type for this instance in case of encountering missing values</value>
    public YAXExceptionTypes TreatErrorsAs { get; private set; }

    /// <summary>
    /// Gets the collection attribute instance.
    /// </summary>
    /// <value>The collection attribute instance.</value>
    public YaxCollectionAttribute CollectionAttributeInstance
    {
        get
        {
            return m_collectionAttributeInstance;
        }
    }

    /// <summary>
    /// Gets the dictionary attribute instance.
    /// </summary>
    /// <value>The dictionary attribute instance.</value>
    public YaxDictionaryAttribute DictionaryAttributeInstance
    {
        get
        {
            return m_dictionaryAttributeInstance;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is treated as a collection.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is treated as a collection; otherwise, <c>false</c>.
    /// </value>
    public bool IsTreatedAsCollection
    {
        get
        {
            return !IsAttributedAsNotCollection && m_memberTypeWrapper.IsTreatedAsCollection;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is treated as a dictionary.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is treated as a dictionary; otherwise, <c>false</c>.
    /// </value>
    public bool IsTreatedAsDictionary
    {
        get
        {
            return !IsAttributedAsNotCollection && m_memberTypeWrapper.IsTreatedAsDictionary;
        }
    }

    /// <summary>
    /// Gets or sets the type of the custom serializer.
    /// </summary>
    /// <value>The type of the custom serializer.</value>
    public Type CustomSerializerType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance has custom serializer.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has custom serializer; otherwise, <c>false</c>.
    /// </value>
    public bool HasCustomSerializer
    {
        get
        {
            return CustomSerializerType != null;
        }
    }

    public bool PreservesWhitespace { get; private set; }

    public int Order { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance has a custom namespace
    /// defined for it through the <see cref="YaxNamespaceAttribute"/> attribute.
    /// </summary>
    public bool HasNamespace
    {
        get
        {
            return Namespace.IsEmpty();
        }
    }

    /// <summary>
    /// Gets the namespace associated with this element.
    /// </summary>
    /// <remarks>
    /// If <see cref="HasNamespace"/> is <c>false</c> then this should
    /// be inherited from any parent elements.
    /// </remarks>
    public XNamespace Namespace
    {
        get
        {
            return m_namespace;
        }

        private set
        {
            m_namespace = value;
            // explicit namespace definition overrides namespace definitions in SerializeAs attributes.
            m_alias = m_namespace + m_alias.LocalName;
        }
    }

    /// <summary>
    /// Gets the namespace prefix associated with this element
    /// </summary>
    /// <remarks>
    /// If <see cref="HasNamespace"/> is <c>false</c> then this should
    /// be inherited from any parent elements.
    /// If this is <c>null</c>, then it should be assumed that the specified
    /// <see cref="Namespace"/> (if it is present) is the default namespace.
    /// 
    /// It should also be noted that if a namespace is not provided for the
    /// entire document (default namespace) and yet a default namespace is
    /// provided for one element that an exception should be thrown (since
    /// setting a default namespace for that element would make it apply to
    /// the whole document).
    /// </remarks>
    public string NamespacePrefix { get; private set; }

    public bool IsRealTypeDefined(Type type)
    {
        return GetRealTypeDefinition(type) != null;
    }

    public YaxTypeAttribute GetRealTypeDefinition(Type type)
    {
        return m_possibleRealTypes.Find(x => ReferenceEquals(x.Type, type));
    }

    /// <summary>
    /// Gets the original value of this member in the specified object
    /// </summary>
    /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
    /// <param name="index">The array of indeces (usually <c>null</c>).</param>
    /// <returns>the original value of this member in the specified object</returns>
    public object GetOriginalValue(object obj, object[] index)
    {
        if (m_isProperty)
        {
            try
            {
                if (m_propertyInfoInstance.GetIndexParameters()?.Length > 0 && index != null)
                    return m_propertyInfoInstance.GetValue(obj, index);
                else
                    return m_propertyInfoInstance.GetValue(obj);
            }
            catch { return null; }
        }
        else
        {
            return m_fieldInfoInstance.GetValue(obj);
        }
    }

    /// <summary>
    /// Gets the processed value of this member in the specified object
    /// </summary>
    /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
    /// <returns>the processed value of this member in the specified object</returns>
    public object GetValue(object obj)
    {
        object elementValue = GetOriginalValue(obj, null);

        if (elementValue == null)
            return null;

        if (m_memberTypeWrapper.IsEnum)
        {
            return m_memberTypeWrapper.EnumWrapper.GetAlias(elementValue);
        }

        // trying to build the element value
        if (HasFormat && !IsTreatedAsCollection)
        {
            // do the formatting. If formatting succeeds the type of 
            // the elementValue will become 'System.String'
            elementValue = ReflectionUtils.TryFormatObject(elementValue, Format);
        }

        return elementValue;
    }

    /// <summary>
    /// Sets the value of this member in the specified object
    /// </summary>
    /// <param name="obj">The object whose member corresponding to this instance, must be given value.</param>
    /// <param name="value">The value.</param>
    public void SetValue(object obj, object value)
    {
        if (m_isProperty)
        {
            m_propertyInfoInstance.SetValue(obj, value, null);
        }
        else
        {
            m_fieldInfoInstance.SetValue(obj, value);
        }
    }

    /// <summary>
    /// Determines whether this instance of <see cref="MemberWrapper"/> can be serialized.
    /// </summary>
    /// <param name="serializationFields">The serialization fields.</param>
    /// <param name="dontSerializePropertiesWithNoSetter">Skip serialization of fields which doesn't have a setter.</param>
    /// <returns>
    /// <c>true</c> if this instance of <see cref="MemberWrapper"/> can be serialized; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAllowedToBeSerialized(YAXSerializationFields serializationFields, bool dontSerializePropertiesWithNoSetter)
    {
        if (dontSerializePropertiesWithNoSetter && m_isProperty && !m_propertyInfoInstance.CanWrite)
            return false;

        bool retour = false;
        if (serializationFields.HasFlag(YAXSerializationFields.AllFields) && !m_isProperty)
            retour = !IsAttributedAsDontSerialize;
        if (serializationFields.HasFlag(YAXSerializationFields.AttributedFieldsOnly) && !m_isProperty)
            retour = retour || IsAttributedAsSerializable;
        if (serializationFields.HasFlag(YAXSerializationFields.PublicProperties) && m_isProperty)
            retour = ReflectionUtils.IsPublicProperty(m_propertyInfoInstance);
        return retour;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
        return m_memberInfo.ToString();
    }

    // Private Methods 

    /// <summary>
    /// Initializes this instance of <c>MemberWrapper</c>.
    /// </summary>
    private void InitInstance()
    {
        Comment = null;
        IsAttributedAsSerializable = false;
        IsAttributedAsDontSerialize = false;
        IsAttributedAsNotCollection = false;
        IsSerializedAsAttribute = false;
        IsSerializedAsValue = false;
        SerializationLocation = ".";
        Format = null;
        InitDefaultValue();
    }

    /// <summary>
    /// Initializes the default value for this instance of <c>MemberWrapper</c>.
    /// </summary>
    private void InitDefaultValue()
    {
        if (MemberType.IsValueType())
        {
            object[] args = [];
            DefaultValue = Activator.CreateInstance(MemberType, args);
        }
        else
            DefaultValue = null;
    }

    /// <summary>
    /// Processes the specified attribute which is an instance of <c>YAXAttribute</c>.
    /// </summary>
    /// <param name="attr">The attribute to process.</param>
    private void ProcessYaxAttribute(object attr)
    {
        if (attr is YaxCommentAttribute commentAttrib)
        {
            string comment = commentAttrib.Comment;
            if (!string.IsNullOrEmpty(comment))
            {
                string[] comments = comment.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < comments.Length; i++)
                {
                    comments[i] = string.Format(" {0} ", comments[i].Trim());
                }

                Comment = comments;
            }
        }
        else if (attr is YaxSerializableFieldAttribute)
        {
            IsAttributedAsSerializable = true;
        }
        else if (attr is YaxAttributeForClassAttribute)
        {
            // it is required that YAXCustomSerializerAttribute is processed earlier
            if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                m_collectionAttributeInstance != null && m_collectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                IsSerializedAsAttribute = true;
                SerializationLocation = ".";
            }
        }
        else if (attr is YaxContentForClassAttribute)
        {
            // it is required that YAXCustomSerializerAttribute is processed earlier
            if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                m_collectionAttributeInstance != null && m_collectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                IsSerializedAsValue = true;
                SerializationLocation = ".";
            }
        }
        else if (attr is YaxAttributeForAttribute attrib)
        {
            // it is required that YAXCustomSerializerAttribute is processed earlier
            if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                m_collectionAttributeInstance != null && m_collectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                IsSerializedAsAttribute = true;
                StringUtils.ExttractPathAndAliasFromLocationString(attrib.Parent, out string path, out string alias);

                SerializationLocation = path;
                if (!string.IsNullOrEmpty(alias))
                    Alias = StringUtils.RefineSingleElement(alias);
            }
        }
        else if (attr is YaxElementForAttribute elementAttrib)
        {
            IsSerializedAsElement = true;

            StringUtils.ExttractPathAndAliasFromLocationString(elementAttrib.Parent, out string path, out string alias);

            SerializationLocation = path;
            if (!string.IsNullOrEmpty(alias))
                Alias = StringUtils.RefineSingleElement(alias);
        }
        else if (attr is YaxContentForAttribute contentAttrib)
        {
            // it is required that YAXCustomSerializerAttribute is processed earlier
            if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                m_collectionAttributeInstance != null && m_collectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                IsSerializedAsValue = true;

                StringUtils.ExttractPathAndAliasFromLocationString(contentAttrib.Parent, out string path, out string alias);

                SerializationLocation = path;
                if (!string.IsNullOrEmpty(alias))
                    Alias = StringUtils.RefineSingleElement(alias);
            }
        }
        else if (attr is YaxDontSerializeAttribute)
        {
            IsAttributedAsDontSerialize = true;
        }
        else if (attr is YaxSerializeAsAttribute serializeAttrib)
        {
            Alias = StringUtils.RefineSingleElement(serializeAttrib.SerializeAs);
        }
        else if (attr is YaxCollectionAttribute collectionAttrib)
        {
            m_collectionAttributeInstance = collectionAttrib;
        }
        else if (attr is YaxDictionaryAttribute dictionaryAttrib)
        {
            m_dictionaryAttributeInstance = dictionaryAttrib;
        }
        else if (attr is YaxErrorIfMissedAttribute errorAttrib)
        {
            DefaultValue = errorAttrib.DefaultValue;
            TreatErrorsAs = errorAttrib.TreatAs;
        }
        else if (attr is YaxFormatAttribute formatAttrib)
        {
            Format = formatAttrib.Format;
        }
        else if (attr is YaxNotCollectionAttribute)
        {
            // arrays are always treated as collections
            if (!ReflectionUtils.IsArray(MemberType))
                IsAttributedAsNotCollection = true;
        }
        else if (attr is YaxCustomSerializerAttribute customSerializeAttrib)
        {
            Type serType = customSerializeAttrib.CustomSerializerType;

            bool isDesiredInterface = ReflectionUtils.IsDerivedFromGenericInterfaceType(serType, typeof(ICustomSerializer<>), out Type genTypeArg);

            if (!isDesiredInterface)
            {
                throw new YaxException("The provided custom serialization type is not derived from the proper interface");
            }
            else if (genTypeArg != MemberType)
            {
                throw new YaxException("The generic argument of the class and the member type do not match");
            }
            else
            {
                CustomSerializerType = serType;
            }
        }
        else if (attr is YaxPreserveWhitespaceAttribute)
        {
            PreservesWhitespace = true;
        }
        else if (attr is YaxSerializableTypeAttribute)
        {
            // this should not happen
            throw new YaxException("This attribute is not applicable to fields and properties!");
        }
        else if (attr is YaxNamespaceAttribute nsAttrib)
        {
            Namespace = nsAttrib.Namespace;
            NamespacePrefix = nsAttrib.Prefix;
        }
        else if (attr is YaxTypeAttribute yaxTypeAttr)
        {
            string alias = yaxTypeAttr.Alias;
            if (alias != null)
            {
                alias = alias.Trim();
                if (alias.Length == 0)
                    alias = null;
            }

            if (m_possibleRealTypes.Exists(x => x.Type == yaxTypeAttr.Type))
                throw new YaxPolymorphicException(string.Format("The type \"{0}\" for field/property \"{1}\" has already been defined through another attribute.", yaxTypeAttr.Type.Name, m_memberInfo));

            if (alias != null && m_possibleRealTypes.Exists(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                throw new YaxPolymorphicException(string.Format("The alias \"{0}\" given to type \"{1}\" for field/property \"{2}\" has already been given to another type through another attribute.",
                    alias, yaxTypeAttr.Type.Name, m_memberInfo));

            m_possibleRealTypes.Add(yaxTypeAttr);
        }
        else if (attr is YaxCollectionItemTypeAttribute yaxColletionItemTypeAttr)
        {
            string alias = yaxColletionItemTypeAttr.Alias;
            if (alias != null)
            {
                alias = alias.Trim();
                if (alias.Length == 0)
                    alias = null;
            }

            if (m_possibleCollectionItemRealTypes.Exists(x => x.Type == yaxColletionItemTypeAttr.Type))
                throw new YaxPolymorphicException(string.Format("The collection-item type \"{0}\" for collection \"{1}\" has already been defined through another attribute.", yaxColletionItemTypeAttr.Type.Name, m_memberInfo));

            if (alias != null && m_possibleCollectionItemRealTypes.Exists(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                throw new YaxPolymorphicException(string.Format("The alias \"{0}\" given to collection-item type \"{1}\" for field/property \"{2}\" has already been given to another type through another attribute.",
                    alias, yaxColletionItemTypeAttr.Type.Name, m_memberInfo));

            m_possibleCollectionItemRealTypes.Add(yaxColletionItemTypeAttr);
        }
        else if (attr is YaxDontSerializeIfNullAttribute)
        {
            IsAttributedAsDontSerializeIfNull = true;
        }
        else if (attr is YaxElementOrderAttribute orderAttrib)
        {
            Order = orderAttrib.Order;
        }
        else
        {
            throw new YaxException("Added new attribute type to the library but not yet processed!");
        }
    }
}
