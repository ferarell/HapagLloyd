Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security

' La información general sobre un ensamblado se controla mediante el siguiente 
' conjunto de atributos. Cambie estos atributos para modificar la información
' asociada con un ensamblado.

' Revisar los valores de los atributos del ensamblado

<Assembly: AssemblyTitle("HL PERU INFO")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyCompany("Hapag Lloyd Perú SAC")> 
<Assembly: AssemblyProduct("Paybot.HapagLloydPeru")> 
<Assembly: AssemblyCopyright("Copyright ©  2016")> 
<Assembly: AssemblyTrademark("")> 

' Si establece ComVisible en false, los tipos de este ensamblado no estarán visibles 
' para los componentes COM.  Si necesita obtener acceso a un tipo de este ensamblado desde 
' COM, establezca el atributo ComVisible en true en este tipo.
<Assembly: ComVisible(False)>

'El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
<Assembly: Guid("c90137b1-a358-483e-a73d-e08fbf6f58af")>

' La información de versión de un ensamblado consta de los cuatro valores siguientes:
'
'      Versión principal
'      Versión secundaria 
'      Número de compilación
'      Revisión
'
' Puede especificar todos los valores o usar los valores predeterminados de número de compilación y de revisión 
' mediante el carácter '*', como se muestra a continuación:
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion("1.0.9.8")>
<Assembly: AssemblyFileVersion("1.0.9.8")>

Friend Module DesignTimeConstants
    Public Const RibbonTypeSerializer As String = "Microsoft.VisualStudio.Tools.Office.Ribbon.Serialization.RibbonTypeCodeDomSerializer, Microsoft.VisualStudio.Tools.Office.Designer, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Public Const RibbonBaseTypeSerializer As String = "System.ComponentModel.Design.Serialization.TypeCodeDomSerializer, System.Design"
    Public Const RibbonDesigner As String = "Microsoft.VisualStudio.Tools.Office.Ribbon.Design.RibbonDesigner, Microsoft.VisualStudio.Tools.Office.Designer, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
End Module
