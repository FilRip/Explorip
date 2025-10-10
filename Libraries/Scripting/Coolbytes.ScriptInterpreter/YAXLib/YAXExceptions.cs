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
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;

namespace CoolBytes.ScriptInterpreter.YAXLib;

/// <summary>
/// The base for all exception classes of YAXLib
/// </summary>
[Serializable()]
public class YaxException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YaxException"/> class.
    /// </summary>
    public YaxException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public YaxException(string message)
        : base(message)
    {
    }

    /// <inheritdoc/>
    protected YaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

/// <summary>
/// Base class for all deserialization exceptions, which contains line and position info
/// </summary>
[Serializable()]
public class YaxDeserializationException : YaxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YaxDeserializationException"/> class.
    /// </summary>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxDeserializationException(IXmlLineInfo lineInfo)
    {
        if (lineInfo != null &&
            lineInfo.HasLineInfo())
        {
            HasLineInfo = true;
            LineNumber = lineInfo.LineNumber;
            LinePosition = lineInfo.LinePosition;
        }

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxDeserializationException"/> class.
    /// </summary>
    /// <param name="lineNumber">The line number on which the error occurred</param>
    /// <param name="linePosition">The line position on which the error occurred</param>
    public YaxDeserializationException(int lineNumber, int linePosition)
    {
        HasLineInfo = true;
        LineNumber = lineNumber;
        LinePosition = linePosition;
    }

    /// <inheritdoc/>
    protected YaxDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Gets whether the exception has line information
    /// Note: if this is unexpectedly false, then most likely you need to specify LoadOptions.SetLineInfo on document load
    /// </summary>
    public bool HasLineInfo { get; private set; }

    /// <summary>
    /// Gets the line number on which the exception occurred
    /// </summary>
    public int LineNumber { get; private set; }

    /// <summary>
    /// Gets the position at which the exception occurred
    /// </summary>
    public int LinePosition { get; private set; }

    /// <summary>
    /// Position string for use in error message
    /// </summary>
    protected string LineInfoMessage => HasLineInfo
        ? string.Format(CultureInfo.CurrentCulture, " Line {0}, position {1}.", LineNumber, LinePosition)
        : string.Empty;

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(HasLineInfo), HasLineInfo);
        info.AddValue(nameof(LineNumber), LineNumber);
        info.AddValue(nameof(LinePosition), LinePosition);
    }
}

/// <summary>
/// Raised when the location of serialization specified cannot be
/// created or cannot be read from.
/// This exception is raised during serialization
/// </summary>
[Serializable()]
public class YaxBadLocationException : YaxException
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadLocationException"/> class.
    /// </summary>
    /// <param name="location">The location.</param>
    public YaxBadLocationException(string location)
    {
        Location = location;
    }

    /// <inheritdoc/>
    protected YaxBadLocationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the bad location which caused the exception
    /// </summary>
    /// <value>The location.</value>
    public string Location { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "The location specified cannot be read from or written to: {0}", Location);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Location), Location);
    }

    #endregion
}

/// <summary>
/// Raised when trying to serialize an attribute where 
/// another attribute with the same name already exists.
/// This exception is raised during serialization.
/// </summary>
[Serializable()]
public class YaxAttributeAlreadyExistsException : YaxException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    public YaxAttributeAlreadyExistsException(string attrName)
    {
        AttrName = attrName;
    }

    /// <inheritdoc/>
    protected YaxAttributeAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string AttrName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "An attribute with this name already exists: '{0}'.", AttrName);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(AttrName), AttrName);
    }

    #endregion
}

/// <summary>
/// Raised when the attribute corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxAttributeMissingException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    public YaxAttributeMissingException(string attrName) :
        this(attrName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="attrName">Name of the attribute.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxAttributeMissingException(string attrName, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        AttributeName = attrName;
    }

    /// <inheritdoc/>
    protected YaxAttributeMissingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string AttributeName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "No attributes with this name found: '{0}'.{1}", AttributeName, LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(AttributeName), AttributeName);
    }

    #endregion
}

/// <summary>
/// Raised when the element value corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxElementValueMissingException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="elementName">Name of the element.</param>
    public YaxElementValueMissingException(string elementName) :
        this(elementName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxElementValueMissingException(string elementName, IXmlLineInfo lineInfo)
        : base(lineInfo)
    {
        ElementName = elementName;
    }

    /// <inheritdoc/>
    protected YaxElementValueMissingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string ElementName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "Element with the given name does not contain text values: '{0}'.{1}", ElementName, LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ElementName), ElementName);
    }
    #endregion
}

/// <summary>
/// Raised when the element value corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
[Obsolete("unused - will be removed in Yax 3"), Serializable()]
public class YaxElementValueAlreadyExistsException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="elementName">Name of the element.</param>
    public YaxElementValueAlreadyExistsException(string elementName) :
        this(elementName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxAttributeMissingException"/> class.
    /// </summary>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxElementValueAlreadyExistsException(string elementName, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        ElementName = elementName;
    }

    /// <inheritdoc/>
    protected YaxElementValueAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public string ElementName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "Element with the given name already has value: '{0}'.{1}", ElementName, LineInfoMessage);
        }
    }

    #endregion
}


/// <summary>
/// Raised when the element corresponding to some property is not present in the given XML file, when deserializing.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxElementMissingException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxElementMissingException"/> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    public YaxElementMissingException(string elemName) :
        this(elemName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxElementMissingException"/> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxElementMissingException(string elemName, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        ElementName = elemName;
    }

    /// <inheritdoc/>
    protected YaxElementMissingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or the name of the element.
    /// </summary>
    /// <value>The name of the element.</value>
    public string ElementName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "No elements with this name found: '{0}'.{1}", ElementName, LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ElementName), ElementName);
    }

    #endregion
}

/// <summary>
/// Raised when the value provided for some property in the XML input, cannot be 
/// converted to the type of the property.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxBadlyFormedInputException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadlyFormedInputException"/> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="badInput">The value of the input which could not be converted to the type of the property.</param>
    public YaxBadlyFormedInputException(string elemName, string badInput)
        : this(elemName, badInput, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadlyFormedInputException"/> class.
    /// </summary>
    /// <param name="elemName">Name of the element.</param>
    /// <param name="badInput">The value of the input which could not be converted to the type of the property.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxBadlyFormedInputException(string elemName, string badInput, IXmlLineInfo lineInfo)
        : base(lineInfo)
    {
        ElementName = elemName;
        BadInput = badInput;
    }

    /// <inheritdoc/>
    protected YaxBadlyFormedInputException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the element.
    /// </summary>
    /// <value>The name of the element.</value>
    public string ElementName { get; private set; }

    /// <summary>
    /// Gets the value of the input which could not be converted to the type of the property.
    /// </summary>
    /// <value>The value of the input which could not be converted to the type of the property.</value>
    public string BadInput { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "The format of the value specified for the property '{0}' is not proper: '{1}'.{2}",
                ElementName,
                BadInput,
                LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ElementName), ElementName);
        info.AddValue(nameof(BadInput), BadInput);
    }

    #endregion
}

/// <summary>
/// Raised when the value provided for some property in the XML input, cannot be 
/// assigned to the property.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxPropertyCannotBeAssignedToException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxPropertyCannotBeAssignedToException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>      
    public YaxPropertyCannotBeAssignedToException(string propName) :
        this(propName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxPropertyCannotBeAssignedToException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>        
    public YaxPropertyCannotBeAssignedToException(string propName, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        PropertyName = propName;
    }

    /// <inheritdoc/>
    protected YaxPropertyCannotBeAssignedToException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <value>The name of the property.</value>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "Could not assign to the property '{0}'.{1}", PropertyName, LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(PropertyName), PropertyName);
    }

    #endregion
}

/// <summary>
/// Raised when some member of the collection in the input XML, cannot be added to the collection object.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxCannotAddObjectToCollectionException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxCannotAddObjectToCollectionException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="obj">The object that could not be added to the collection.</param>
    public YaxCannotAddObjectToCollectionException(string propName, object obj) :
        this(propName, obj, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxCannotAddObjectToCollectionException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="obj">The object that could not be added to the collection.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxCannotAddObjectToCollectionException(string propName, object obj, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        PropertyName = propName;
        ObjectToAdd = obj;
    }

    /// <inheritdoc/>
    protected YaxCannotAddObjectToCollectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <value>The name of the property.</value>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Gets the object that could not be added to the collection.
    /// </summary>
    /// <value>the object that could not be added to the collection.</value>
    public object ObjectToAdd { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Could not add object ('{0}') to the collection ('{1}').{2}",
                ObjectToAdd,
                PropertyName,
                LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ObjectToAdd), ObjectToAdd);
        info.AddValue(nameof(PropertyName), PropertyName);
    }

    #endregion
}

/// <summary>
/// Raised when the default value specified by the <c>YAXErrorIfMissedAttribute</c> could not be assigned to the property.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxDefaultValueCannotBeAssignedException : YaxDeserializationException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxDefaultValueCannotBeAssignedException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="defaultValue">The default value which caused the problem.</param>
    public YaxDefaultValueCannotBeAssignedException(string propName, object defaultValue) :
        this(propName, defaultValue, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxDefaultValueCannotBeAssignedException"/> class.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="defaultValue">The default value which caused the problem.</param>
    /// <param name="lineInfo">IXmlLineInfo derived object, e.g. XElement, XAttribute containing line info</param>
    public YaxDefaultValueCannotBeAssignedException(string propName, object defaultValue, IXmlLineInfo lineInfo) :
        base(lineInfo)
    {
        PropertyName = propName;
        TheDefaultValue = defaultValue;
    }

    /// <inheritdoc/>
    protected YaxDefaultValueCannotBeAssignedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <value>The name of the property.</value>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Gets the default value which caused the problem.
    /// </summary>
    /// <value>The default value which caused the problem.</value>
    public object TheDefaultValue { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Could not assign the default value specified ('{0}') for the property '{1}'.{2}",
                TheDefaultValue,
                PropertyName,
                LineInfoMessage);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(TheDefaultValue), TheDefaultValue);
        info.AddValue(nameof(PropertyName), PropertyName);
    }

    #endregion
}

/// <summary>
/// Raised when the XML input does not follow standard XML formatting rules.
/// This exception is raised during deserialization.
/// </summary>
[Serializable()]
public class YaxBadlyFormedXmlException : YaxDeserializationException
{
    #region Fields

    /// <summary>
    /// The inner exception.
    /// </summary>
    private readonly Exception innerException;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadlyFormedXmlException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="lineNumber">The line number on which the error occurred</param>
    /// <param name="linePosition">The line position on which the error occurred</param>
    public YaxBadlyFormedXmlException(Exception innerException, int lineNumber, int linePosition)
        : base(lineNumber, linePosition)
    {
        this.innerException = innerException;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadlyFormedXmlException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    public YaxBadlyFormedXmlException(Exception innerException)
        : base(null)
    {
        this.innerException = innerException;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxBadlyFormedXmlException"/> class.
    /// </summary>       
    public YaxBadlyFormedXmlException()
        : this(null)
    {
    }

    /// <inheritdoc/>
    protected YaxBadlyFormedXmlException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            string msg = string.Format(CultureInfo.CurrentCulture, "The input xml file is not properly formatted!{0}", LineInfoMessage);

            if (innerException != null)
            {
                msg += string.Format(CultureInfo.CurrentCulture, "\r\nMore Details:\r\n{0}", innerException.Message);
            }

            return msg;
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(innerException), innerException);
    }

    #endregion
}

/// <summary>
/// Raised when an object cannot be formatted with the format string provided.
/// This exception is raised during serialization.
/// </summary>
[Obsolete("unused - will be removed in Yax 3"), Serializable()]
public class YaxInvalidFormatProvidedException : YaxException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxInvalidFormatProvidedException"/> class.
    /// </summary>
    /// <param name="objectType">Type of the fiedl to serialize.</param>
    /// <param name="format">The format string.</param>
    public YaxInvalidFormatProvidedException(Type objectType, string format)
    {
        ObjectType = objectType;
        Format = format;
    }

    /// <inheritdoc/>
    protected YaxInvalidFormatProvidedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the type of the field to serialize
    /// </summary>
    /// <value>The type of the field to serialize.</value>
    public Type ObjectType { get; private set; }

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public string Format { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Could not format objects of type '{0}' with the format string '{1}'",
                ObjectType.Name,
                Format);
        }
    }

    #endregion
}

/// <summary>
/// Raised when trying to serialize self-referential types. This exception cannot be turned off.
/// This exception is raised during serialization.
/// </summary>
[Serializable()]
public class YaxCannotSerializeSelfReferentialTypesException : YaxException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxCannotSerializeSelfReferentialTypesException"/> class.
    /// </summary>
    /// <param name="type">The the self-referential type that caused the problem.</param>
    public YaxCannotSerializeSelfReferentialTypesException(Type type)
    {
        SelfReferentialType = type;
    }

    /// <inheritdoc/>
    protected YaxCannotSerializeSelfReferentialTypesException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the self-referential type that caused the problem.
    /// </summary>
    /// <value>The type of the self-referential type that caused the problem.</value>
    public Type SelfReferentialType { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(CultureInfo.CurrentCulture, "Self Referential types ('{0}') cannot be serialized.", SelfReferentialType.FullName);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(SelfReferentialType), SelfReferentialType);
    }

    #endregion
}

/// <summary>
/// Raised when the object provided for serialization is not of the type provided for the serializer. This exception cannot be turned off.
/// This exception is raised during serialization.
/// </summary>
[Serializable()]
public class YaxObjectTypeMismatchException : YaxException
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YaxObjectTypeMismatchException"/> class.
    /// </summary>
    /// <param name="expectedType">The expected type.</param>
    /// <param name="receivedType">The type of the object which did not match the expected type.</param>
    public YaxObjectTypeMismatchException(Type expectedType, Type receivedType)
    {
        ExpectedType = expectedType;
        ReceivedType = receivedType;
    }

    /// <inheritdoc/>
    protected YaxObjectTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the expected type.
    /// </summary>
    /// <value>The expected type.</value>
    public Type ExpectedType { get; private set; }

    /// <summary>
    /// Gets the type of the object which did not match the expected type.
    /// </summary>
    /// <value>The type of the object which did not match the expected type.</value>
    public Type ReceivedType { get; private set; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>
    /// The error message that explains the reason for the exception, or an empty string("").
    /// </returns>
    public override string Message
    {
        get
        {
            return string.Format(
               CultureInfo.CurrentCulture,
               "Expected an object of type '{0}' but received an object of type '{1}'.",
               ExpectedType.Name,
               ReceivedType.Name);
        }
    }

    /// <summary>
    /// Implementation de Serializable()
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ExpectedType), ExpectedType);
        info.AddValue(nameof(ReceivedType), ReceivedType);
    }
    #endregion
}

/// <summary>
/// Raised when the object has field already defined through another attribute
/// </summary>
[Serializable()]
public class YaxPolymorphicException : YaxException
{
    /// <summary>
    /// Constructeur de l'exception
    /// </summary>
    /// <param name="message"></param>
    public YaxPolymorphicException(string message)
        : base(message)
    {
    }

    /// <inheritdoc/>
    protected YaxPolymorphicException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
