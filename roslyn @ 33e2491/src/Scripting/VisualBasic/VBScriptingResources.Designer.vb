﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Reflection

'This class was auto-generated by the StronglyTypedResourceBuilder
'class via a tool like ResGen or Visual Studio.
'To add or remove a member, edit your .ResX file then rerun ResGen
'with the /str option, or rebuild your VS project.
'''<summary>
'''  A strongly-typed resource class, for looking up localized strings, etc.
'''</summary>
<Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
 Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
 Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
Friend Class VBScriptingResources
    
    Private Shared resourceMan As Global.System.Resources.ResourceManager
    
    Private Shared resourceCulture As Global.System.Globalization.CultureInfo
    
    <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
    Friend Sub New()
        MyBase.New
    End Sub
    
    '''<summary>
    '''  Returns the cached ResourceManager instance used by this class.
    '''</summary>
    <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
        Get
            If Object.ReferenceEquals(resourceMan, Nothing) Then
                Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("VBScriptingResources", GetType(VBScriptingResources).GetTypeInfo().Assembly)
                resourceMan = temp
            End If
            Return resourceMan
        End Get
    End Property
    
    '''<summary>
    '''  Overrides the current thread's CurrentUICulture property for all
    '''  resource lookups using this strongly typed resource class.
    '''</summary>
    <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
        Get
            Return resourceCulture
        End Get
        Set
            resourceCulture = value
        End Set
    End Property

    '''<summary>
    '''  Looks up a localized string similar to Cannot escape non-printable characters in Visual Basic notation unless quotes are used..
    '''</summary>
    Friend Shared ReadOnly Property ExceptionEscapeWithoutQuote() As String
        Get
            Return ResourceManager.GetString("ExceptionEscapeWithoutQuote", resourceCulture)
        End Get
    End Property

    '''<summary>
    '''  Looks up a localized string similar to Usage: vbi [options] [script-file.vbx] [-- script-arguments]
    '''
    '''If script-file is specified executes the script, otherwise launches an interactive REPL (Read Eval Print Loop).
    '''
    '''Options:
    '''  /help                          Display this usage message (Short form: /?)
    '''  /reference:&lt;alias&gt;=&lt;file&gt;      Reference metadata from the specified assembly file using the given alias (Short form: /r)
    '''  /reference:&lt;file list&gt;         Reference metadata from the specified assembly files (Short form: /r)
    '''  /referencePath [rest of string was truncated]&quot;;.
    '''</summary>
    Friend Shared ReadOnly Property InteractiveHelp() As String
        Get
            Return ResourceManager.GetString("InteractiveHelp", resourceCulture)
        End Get
    End Property

    '''<summary>
    '''  Looks up a localized string similar to Microsoft (R) Visual Basic Interactive Compiler version {0}.
    '''</summary>
    Friend Shared ReadOnly Property LogoLine1() As String
        Get
            Return ResourceManager.GetString("LogoLine1", resourceCulture)
        End Get
    End Property
    
    '''<summary>
    '''  Looks up a localized string similar to Copyright (C) Microsoft Corporation. All rights reserved..
    '''</summary>
    Friend Shared ReadOnly Property LogoLine2() As String
        Get
            Return ResourceManager.GetString("LogoLine2", resourceCulture)
        End Get
    End Property
End Class