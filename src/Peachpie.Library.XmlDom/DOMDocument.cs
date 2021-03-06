﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Pchp.Core;
using Pchp.Library.Streams;

namespace Peachpie.Library.XmlDom
{
    [PhpType(PhpTypeAttribute.InheritName)]
    public class DOMDocument : DOMNode
    {
        #region Fields and Properties

        internal XmlDocument XmlDocument
        {
            get { return (XmlDocument)XmlNode; }
            set { XmlNode = value; }
        }

        private bool _formatOutput;
        private bool _validateOnParse;
        internal bool _isHtmlDocument;

        /// <summary>
        /// Returns &quot;#document&quot;.
        /// </summary>
        public override string nodeName => "#document";

        /// <summary>
        /// Returns <B>null</B>.
        /// </summary>
        public override string nodeValue
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Returns the type of the node (<see cref="NodeType.Document"/>).
        /// </summary>
        public override int nodeType => (int)NodeType.Document;

        /// <summary>
        /// Returns the node containing the DOCTYPE declaration.
        /// </summary>
        //public DOMDocumentType doctype
        //{
        //    get
        //    {
        //        XmlDocumentType doc_type = XmlDocument.DocumentType;
        //        return (doc_type == null ? null : DOMNode.Create(doc_type));
        //    }
        //}

        /// <summary>
        /// Returns the DOM implementation.
        /// </summary>
        public DOMImplementation implementation => new DOMImplementation();

        /// <summary>
        /// Returns the root element of this document.
        /// </summary>
        public DOMElement documentElement
        {
            get
            {
                XmlElement root = XmlDocument.DocumentElement;
                return (root != null ? (DOMElement)Create(root) : null);
            }
        }

        /// <summary>
        /// Returns the encoding of this document.
        /// </summary>
        public string actualEncoding => this.encoding;

        /// <summary>
        /// Returns the encoding of this document.
        /// </summary>
        public string xmlEncoding => this.encoding;

        /// <summary>
        /// Returns or set the encoding of this document.
        /// </summary>
        public string encoding
        {
            get
            {
                XmlDeclaration decl = GetXmlDeclaration();
                if (decl != null) return decl.Encoding;
                return null;
            }
            set
            {
                XmlDeclaration decl = GetXmlDeclaration();
                if (decl != null) decl.Encoding = value;
                else
                {
                    decl = XmlDocument.CreateXmlDeclaration("1.0", value, null);
                    XmlDocument.InsertBefore(decl, XmlDocument.FirstChild);
                }
            }
        }

        /// <summary>
        /// Returns or sets the standalone flag of this document.
        /// </summary>
        public bool xmlStandalone
        {
            get { return this.standalone; }
            set { this.standalone = value; }
        }

        /// <summary>
        /// Returns or sets the standalone flag of this document.
        /// </summary>
        public bool standalone
        {
            get
            {
                XmlDeclaration decl = GetXmlDeclaration();
                return (decl == null || (decl.Standalone != "no"));
            }
            set
            {
                string stand = (value ? "yes" : "no");

                XmlDeclaration decl = GetXmlDeclaration();
                if (decl != null) decl.Standalone = stand;
                else
                {
                    decl = XmlDocument.CreateXmlDeclaration("1.0", null, stand);
                    XmlDocument.InsertBefore(decl, XmlDocument.FirstChild);
                }
            }
        }

        /// <summary>
        /// Returns or sets the XML version of this document.
        /// </summary>
        public string xmlVersion
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>
        /// Returns or sets the XML version of this document.
        /// </summary>
        public string version
        {
            get
            {
                XmlDeclaration decl = GetXmlDeclaration();
                return (decl == null ? "1.0" : decl.Version);
            }
            set
            {
                XmlDeclaration decl = GetXmlDeclaration();
                if (decl != null)
                {
                    XmlDeclaration new_decl = XmlDocument.CreateXmlDeclaration(value, decl.Encoding, decl.Standalone);
                    XmlDocument.ReplaceChild(new_decl, decl);
                }
                else
                {
                    decl = XmlDocument.CreateXmlDeclaration(value, null, null);
                    XmlDocument.InsertBefore(decl, XmlDocument.FirstChild);
                }
            }
        }

        /// <summary>
        /// Returns <B>true</B>.
        /// </summary>
        public bool strictErrorChecking
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// Returns the base URI of this document.
        /// </summary>
        public string documentURI
        {
            get { return XmlDocument.BaseURI; }
            set { }
        }

        /// <summary>
        /// Returns <B>null</B>.
        /// </summary>
        public DOMConfiguration config => null;

        /// <summary>
        /// Returns or sets whether XML is formatted by <see cref="save(string,int)"/> and <see cref="saveXML(DOMNode)"/>.
        /// </summary>
        public bool formatOutput
        {
            get { return _formatOutput; }
            set { _formatOutput = value; }
        }

        /// <summary>
        /// Returns of sets whether XML is validated against schema by <see cref="load(DOMDocument,string,int)"/> and
        /// <see cref="loadXML(DOMDocument,string,int)"/>.
        /// </summary>
        public bool validateOnParse
        {
            get { return _validateOnParse; }
            set { _validateOnParse = value; }
        }

        /// <summary>
        /// Returns <B>false</B>.
        /// </summary>
        public bool resolveExternals
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Returns or sets whether whitespace should be preserved by this XML document.
        /// </summary>
        public bool preserveWhiteSpace
        {
            get { return XmlDocument.PreserveWhitespace; }
            set { XmlDocument.PreserveWhitespace = value; }
        }

        /// <summary>
        /// Returns <B>false</B>.
        /// </summary>
        public bool recover
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Returns <B>false</B>.
        /// </summary>
        public bool substituteEntities
        {
            get { return false; }
            set { }
        }

        #endregion

        #region Construction

        public DOMDocument()
        {
            this.XmlDocument = new XmlDocument();
            this.XmlDocument.PreserveWhitespace = true;
        }

        internal DOMDocument(XmlDocument xmlDocument)
        {
            this.XmlDocument = xmlDocument;
        }

        protected override DOMNode CloneObjectInternal(bool deepCopyFields) => new DOMDocument(XmlDocument);

        public virtual void __construct(string version = null, string encoding = null)
        {
            // append the corresponding XML declaration to the document
            if (version == null) version = "1.0";
            XmlDocument.AppendChild(XmlDocument.CreateXmlDeclaration("1.0", encoding, String.Empty));
        }

        #endregion

        #region Node factory

        /// <summary>
        /// Creates an element with the specified name and inner text.
        /// </summary>
        /// <param name="tagName">The qualified name of the element.</param>
        /// <param name="value">The inner text (value) of the element.</param>
        /// <returns>A new <see cref="DOMElement"/>.</returns>
        public DOMElement createElement(string tagName, string value = null)
        {
            XmlElement element = XmlDocument.CreateElement(tagName);
            if (value != null) element.InnerText = value;
            return new DOMElement(element);
        }

        /// <summary>
        /// Creates a new document fragment.
        /// </summary>
        /// <returns>A new <see cref="DOMDocumentFragment"/>.</returns>
        //public DOMDocumentFragment createDocumentFragment()
        //{
        //    XmlDocumentFragment fragment = XmlDocument.CreateDocumentFragment();
        //    return new DOMDocumentFragment(fragment);
        //}

        /// <summary>
        /// Creates a new text node with the specified text.
        /// </summary>
        /// <param name="data">The text for the text node.</param>
        /// <returns>A new <see cref="DOMText"/>.</returns>
        //public DOMText createTextNode(string data)
        //{
        //    XmlText text = XmlDocument.CreateTextNode(data);
        //    return new DOMText(text);
        //}

        /// <summary>
        /// Creates a comment node containing the specified data.
        /// </summary>
        /// <param name="data">The comment data.</param>
        /// <returns>A new <see cref="DOMComment"/>.</returns>
        //public DOMComment createComment(string data)
        //{
        //    XmlComment comment = XmlDocument.CreateComment(data);
        //    return new DOMComment(comment);
        //}

        /// <summary>
        /// Creates a CDATA section containing the specified data.
        /// </summary>
        /// <param name="data">The content of the new CDATA section.</param>
        /// <returns>A new <see cref="DOMCdataSection"/>.</returns>
        //public DOMCdataSection createCDATASection(string data)
        //{
        //    XmlCDataSection cdata = XmlDocument.CreateCDataSection(data);
        //    return new DOMCdataSection(cdata);
        //}

        /// <summary>
        /// Creates a processing instruction with the specified name and data.
        /// </summary>
        /// <param name="target">The name of the processing instruction.</param>
        /// <param name="data">The data for the processing instruction.</param>
        /// <returns>A new <see cref="DOMProcessingInstruction"/>.</returns>
        //public DOMProcessingInstruction createProcessingInstruction(string target, string data)
        //{
        //    XmlProcessingInstruction pi = XmlDocument.CreateProcessingInstruction(target, data);
        //    return new DOMProcessingInstruction(pi);
        //}

        /// <summary>
        /// Creates an attribute with the specified name.
        /// </summary>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <returns>A new <see cref="DOMAttr"/>.</returns>
        public DOMAttr createAttribute(string name)
        {
            XmlAttribute attribute = XmlDocument.CreateAttribute(name);
            return new DOMAttr(attribute);
        }

        /// <summary>
        /// Creates an entity reference with the specified name.
        /// </summary>
        /// <param name="name">The name of the entity reference.</param>
        /// <returns>A new <see cref="DOMEntityReference"/>.</returns>
        //public DOMEntityReference createEntityReference(string name)
        //{
        //    XmlEntityReference entref = XmlDocument.CreateEntityReference(name);
        //    return new DOMEntityReference(entref);
        //}

        /// <summary>
        /// Creates an element with the specified namespace URI and qualified name.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI of the element.</param>
        /// <param name="qualifiedName">The qualified name of the element.</param>
        /// <param name="value">The inner text (value) of the element.</param>
        /// <returns>A new <see cref="DOMElement"/>.</returns>
        //public DOMElement createElementNS(string namespaceUri, string qualifiedName, [Optional] string value)
        //{
        //    XmlElement element = XmlDocument.CreateElement(qualifiedName, namespaceUri);
        //    if (value != null) element.InnerText = value;
        //    return new DOMElement(element);
        //}

        /// <summary>
        /// Creates an attribute with the specified namespace URI and qualified name.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI of the attribute.</param>
        /// <param name="qualifiedName">The qualified name of the attribute.</param>
        /// <returns>A new <see cref="DOMAttr"/>.</returns>
        //public DOMAttr createAttributeNS(string namespaceUri, string qualifiedName)
        //{
        //    XmlAttribute attribute = XmlDocument.CreateAttribute(qualifiedName, namespaceUri);
        //    return new DOMAttr(attribute);
        //}

        #endregion

        #region Child elements

        /// <summary>
        /// Gets all descendant elements with the matching tag name.
        /// </summary>
        /// <param name="name">The tag name. Use <B>*</B> to return all elements within the element tree.</param>
        /// <returns>A <see cref="DOMNodeList"/>.</returns>
        public DOMNodeList getElementsByTagName(string name)
        {
            DOMNodeList list = new DOMNodeList();

            // enumerate elements in the default namespace
            foreach (XmlNode node in XmlDocument.GetElementsByTagName(name))
            {
                var dom_node = DOMNode.Create(node);
                if (dom_node != null) list.AppendNode(dom_node);
            }

            // enumerate all namespaces
            XPathNavigator navigator = XmlDocument.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//namespace::*[not(. = ../../namespace::*)]");

            while (iterator.MoveNext())
            {
                string prefix = iterator.Current.Name;
                if (!String.IsNullOrEmpty(prefix) && prefix != "xml")
                {
                    // enumerate elements in this namespace
                    foreach (XmlNode node in XmlDocument.GetElementsByTagName(name, iterator.Current.Value))
                    {
                        var dom_node = DOMNode.Create(node);
                        if (dom_node != null) list.AppendNode(dom_node);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Gets all descendant elements with the matching namespace URI and local name.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI.</param>
        /// <param name="localName">The local name. Use <B>*</B> to return all elements within the element tree.</param>
        /// <returns>A <see cref="DOMNodeList"/>.</returns>
        public DOMNodeList getElementsByTagNameNS(string namespaceUri, string localName)
        {
            DOMNodeList list = new DOMNodeList();

            foreach (XmlNode node in XmlDocument.GetElementsByTagName(localName, namespaceUri))
            {
                var dom_node = DOMNode.Create(node);
                if (dom_node != null) list.AppendNode(dom_node);
            }

            return list;
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public DOMElement getElementById(string elementId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Hierarchy

        /// <summary>
        /// Imports a node from another document to the current document.
        /// </summary>
        /// <param name="importedNode">The node being imported.</param>
        /// <param name="deep"><B>True</B> to perform deep clone; otheriwse <B>false</B>.</param>
        /// <returns>The imported <see cref="DOMNode"/>.</returns>
        public DOMNode importNode(DOMNode importedNode, bool deep)
        {
            if (importedNode.IsAssociated)
            {
                return DOMNode.Create(XmlDocument.ImportNode(importedNode.XmlNode, deep));
            }
            else
            {
                importedNode.Associate(XmlDocument);
                return importedNode;
            }
        }

        /// <summary>
        /// Not implemented in PHP 5.1.6.
        /// </summary>
        public DOMNode adoptNode(DOMNode source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Puts the entire XML document into a &quot;normal&quot; form.
        /// </summary>
        public void normalizeDocument() => XmlDocument.Normalize();

        /// <summary>
        /// Not implemented in PHP 5.1.6.
        /// </summary>
        public void renameNode(DOMNode node, string namespaceUri, string qualifiedName)
        {
            throw new NotImplementedException();
        }

        private XmlDeclaration GetXmlDeclaration() => (XmlNode.FirstChild as XmlDeclaration);

        #endregion

        #region Load and Save

        /// <summary>
        /// Loads the XML document from the specified URL.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="fileName">URL for the file containing the XML document to load.</param>
        /// <param name="options">Undocumented.</param>
        /// <returns><b>True</b> on success or <b>false</b> on failure.</returns>
        public bool load(Context ctx, string fileName, int options = 0)
        {
            // TODO: this method can be called both statically and via an instance

            _isHtmlDocument = false;

            using (PhpStream stream = PhpStream.Open(ctx, fileName, "rt"))
            {
                if (stream == null) return false;

                try
                {
                    if (_validateOnParse)
                    {
                        // create a validating XML reader
                        XmlReaderSettings settings = new XmlReaderSettings();
//#pragma warning disable 618
//                        settings.ValidationType = ValidationType.Auto;
//#pragma warning restore 618

                        XmlDocument.Load(XmlReader.Create(stream.RawStream, settings));
                    }
                    else XmlDocument.Load(stream.RawStream);
                }
                catch (XmlException e)
                {
                    PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, fileName);
                    return false;
                }
                catch (IOException e)
                {
                    PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, fileName);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Loads the XML document from the specified string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <param name="options">Undocumented.</param>
        /// <returns><b>True</b> on success or <b>false</b> on failure.</returns>
        public bool loadXML(string xmlString, int options = 0)
        {
            // TODO: this method can be called both statically and via an instance

            return loadXMLInternal(xmlString, options, false);
        }

        /// <summary>
        /// Loads provided XML string into this <see cref="DOMDocument"/>.
        /// </summary>
        /// <param name="xmlString">String representing XML document.</param>
        /// <param name="options">PHP options.</param>
        /// <param name="isHtml">Whether the <paramref name="xmlString"/> represents XML generated from HTML document (then it may contain some invalid XML characters).</param>
        /// <returns></returns>
        private bool loadXMLInternal(string xmlString, int options, bool isHtml)
        {
            this._isHtmlDocument = isHtml;

            var stream = new StringReader(xmlString);

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                // TODO: Enable when DtdProcessing.Parse is enabled in System.Xml.ReaderWriter package
                // validating XML reader
                if (this._validateOnParse)
//#pragma warning disable 618
//                    settings.DtdProcessing = DtdProcessing.Parse;
//#pragma warning restore 618

                // do not check invalid characters in HTML (XML)
                if (isHtml)
                    settings.CheckCharacters = false;

                // load the document
                this.XmlDocument.Load(XmlReader.Create(stream, settings));

                // done
                return true;
            }
            catch (XmlException e)
            {
                PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, null);
                return false;
            }
            catch (IOException e)
            {
                PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, null);
                return false;
            }
        }

        /// <summary>
        /// Saves the XML document to the specified stream.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="fileName">The location of the file where the document should be saved.</param>
        /// <param name="options">Unsupported.</param>
        /// <returns>The number of bytes written or <B>false</B> on error.</returns>
        public object save(Context ctx, string fileName, int options = 0)
        {
            using (PhpStream stream = PhpStream.Open(ctx, fileName, "wt"))
            {
                if (stream == null) return false;

                try
                {
                    // direct stream write indents
                    if (_formatOutput) XmlDocument.Save(stream.RawStream);
                    else
                    {
                        var settings = new XmlWriterSettings()
                        {
                            Encoding = Utils.GetNodeEncoding(ctx, XmlNode)
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream.RawStream, settings))
                        {
                            XmlDocument.Save(writer);
                        }
                    }
                }
                catch (XmlException e)
                {
                    PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, fileName);
                    return null;
                }
                catch (IOException e)
                {
                    PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, fileName);
                    return false;
                }

                // TODO:
                return (stream.RawStream.CanSeek ? stream.RawStream.Position : 1);
            }
        }

        /// <summary>
        /// Returns the string representation of this document.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="node">The node to dump (the entire document if <B>null</B>).</param>
        /// <returns>The string representation of the document / the specified node or <B>false</B>.</returns>
        [return: CastToFalse]
        public PhpString saveXML(Context ctx, DOMNode node = null)
        {
            XmlNode xml_node;

            if (node == null) xml_node = XmlNode;
            else
            {
                xml_node = node.XmlNode;
                if (xml_node.OwnerDocument != XmlDocument && xml_node != XmlNode)
                {
                    DOMException.Throw(ExceptionCode.WrongDocument);
                    return null;
                }
            }

            var settings = new XmlWriterSettings()
            {
                Encoding = Utils.GetNodeEncoding(ctx, xml_node),
                Indent = _formatOutput
            };

            using (MemoryStream stream = new MemoryStream())
            {
                // use a XML writer and set its Formatting proprty to Formatting.Indented
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    xml_node.WriteTo(writer);
                }

                return new PhpString(stream.ToArray());
            }
        }

        /// <summary>
        /// Processes HTML errors, if any.
        /// </summary>
        /// <param name="htmlDoc"><see cref="HtmlAgilityPack.HtmlDocument"/> instance to process errors from.</param>
        /// <param name="filename">HTML file name or <c>null</c> if HTML has been loaded from a string.</param>
        private void CheckHtmlErrors(HtmlAgilityPack.HtmlDocument htmlDoc, string filename)
        {
            Debug.Assert(htmlDoc != null);

            foreach (var error in htmlDoc.ParseErrors)
            {
                switch (error.Code)
                {
                    case HtmlAgilityPack.HtmlParseErrorCode.EndTagNotRequired:
                    case HtmlAgilityPack.HtmlParseErrorCode.TagNotOpened:
                        break;
                    default:
                        PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, error.Line, error.LinePosition, "(" + error.Code.ToString() + ")" + error.Reason, filename);
                        break;
                }
            }
        }

        /// <summary>
        /// Loads HTML from a string.
        /// </summary>
        /// <param name="source">String containing HTML document.</param>
        /// <returns>TRUE on success or FALSE on failure.</returns>
        public bool loadHTML(string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return loadHTML(new StringReader(source), null);
        }

        /// <summary>
        /// Loads HTML from a file.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="sourceFile">Path to a file containing HTML document.</param>
        public bool loadHTMLFile(Context ctx, string sourceFile)
        {
            using (PhpStream stream = PhpStream.Open(ctx, sourceFile, "rt"))
            {
                if (stream == null) return false;
    
                return loadHTML(new StreamReader(stream.RawStream), sourceFile);
            }
        }

        /// <summary>
        /// Load HTML DOM from given <paramref name="stream"/>.
        /// </summary>
        private bool loadHTML(TextReader stream, string filename)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // setup HTML parser
            htmlDoc.OptionOutputAsXml = true;
            //htmlDoc.OptionOutputOriginalCase = true;  // NOTE: we need lower-cased names because of XPath queries
            //htmlDoc.OptionFixNestedTags = true;
            htmlDoc.OptionCheckSyntax = false;
            htmlDoc.OptionUseIdAttribute = false;   // only needed when XPath navigator is used on htmlDoc
            htmlDoc.OptionWriteEmptyNodes = true;

            // load HTML (from string or a stream)
            htmlDoc.Load(stream);

            CheckHtmlErrors(htmlDoc, filename);

            // save to string as XML
            using (StringWriter sw = new StringWriter())
            {
                htmlDoc.Save(sw);

                // load as XML
                return this.loadXMLInternal(sw.ToString(), 0, true);
            }
        }

        /// <summary>
        /// Not implemented (TODO: need an HTML parser for this).
        /// </summary>
        public object saveHTMLFile(Context ctx, string file = null)
        {
            //TODO: use the HTML parse to same HTML
            return save(ctx, file, 0);
        }

        #endregion

        #region XInclude

        /// <summary>
        /// Not implemented (TODO: need a XInclude implementation for this).
        /// </summary>
        public void xinclude(int options = 0)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Not implemented (System.Xml does not support post-load DTD validation).
        /// </summary>
        public bool validate()
        {
            //PhpException.Throw(PhpError.Warning, Resources.PostLoadDtdUnsupported);
            throw new DOMException(Resources.PostLoadDtdUnsupported);
        }

        /// <summary>
        /// Validates the document against the specified XML schema.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="schemaFile">URL for the file containing the XML schema to load.</param>
        /// <returns><B>True</B> or <B>false</B>.</returns>
        //public bool schemaValidate(Context ctx, string schemaFile)
        //{
        //    XmlSchema schema;

        //    using (PhpStream stream = PhpStream.Open(ctx, schemaFile, "rt"))
        //    {
        //        if (stream == null) return false;

        //        try
        //        {
        //            schema = XmlSchema.Read(stream.RawStream, null);
        //        }
        //        catch (XmlException e)
        //        {
        //            PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_WARNING, 0, 0, 0, e.Message, schemaFile);
        //            return false;
        //        }
        //        catch (IOException e)
        //        {
        //            PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_ERROR, 0, 0, 0, e.Message, schemaFile);
        //            return false;
        //        }
        //    }

        //    XmlDocument.Schemas.Add(schema);
        //    try
        //    {
        //        XmlDocument.Validate(null);
        //    }
        //    catch (XmlException)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        XmlDocument.Schemas.Remove(schema);
        //    }
        //    return true;
        //}

        /// <summary>
        /// Validates the document against the specified XML schema.
        /// </summary>
        /// <param name="schemaString">The XML schema string.</param>
        /// <returns><B>True</B> or <B>false</B>.</returns>
        //public bool schemaValidateSource(string schemaString)
        //{
        //    XmlSchema schema;

        //    try
        //    {
        //        schema = XmlSchema.Read(new System.IO.StringReader(schemaString), null);
        //    }
        //    catch (XmlException e)
        //    {
        //        PhpLibXml.IssueXmlError(PhpLibXml.LIBXML_ERR_WARNING, 0, 0, 0, e.Message, null);
        //        return false;
        //    }

        //    XmlDocument.Schemas.Add(schema);
        //    try
        //    {
        //        XmlDocument.Validate(null);
        //    }
        //    catch (XmlException)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        XmlDocument.Schemas.Remove(schema);
        //    }
        //    return true;
        //}

        /// <summary>
        /// Not implemented (TODO: will need a Relax NG validator for this).
        /// </summary>
        public bool relaxNGValidate(string schemaFile)
        {
            PhpException.Throw(PhpError.Warning, Resources.RelaxNGUnsupported);
            return true;
        }

        /// <summary>
        /// Not implemented (TODO: will need a Relax NG validator for this).
        /// </summary>
        public bool relaxNGValidateSource(string schema)
        {
            PhpException.Throw(PhpError.Warning, Resources.RelaxNGUnsupported);
            return true;
        }

        #endregion
    }
}
