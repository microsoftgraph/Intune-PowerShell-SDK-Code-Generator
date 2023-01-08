# Tabla de contenido
- [Tabla de contenido](#table-of-contents)
- [Intune-PowerShell-SDK-Code-Generator](#intune-powershell-sdk-code-generator)
- [Colaboradores](#contributing)
- [Diseño](#design)
    - [Arquitectura de alto nivel](#high-level-architecture)
    - [El paso de generación automática](#the-auto-generation-step)
- [Trabajos futuros](#future-work)
- [Preguntas frecuentes](#faq)
    - [¿Dónde está el código generado?](#where-is-the-generated-code)
    - [¿Por qué necesitamos este módulo de PowerShell?](#why-do-we-need-this-powershell-module)
    - [¿Por qué queremos generar automáticamente el módulo de PowerShell?](#why-do-we-want-to-auto-generate-the-powershell-module)
    - [¿Por qué la compilación está siendo realizada por Intune (y por qué no la está realizando el equipo SDK de Graph)?](#why-is-intune-building-it-and-why-isnt-the-graph-sdk-team-building-it)
    - [¿Qué es Vipr?](#what-is-vipr)
    - [¿Por qué un escritor personalizado?](#why-a-custom-writer)
    - [¿Por qué no usar OpenAPI y Swagger?](#why-not-use-openapi-and-swagger)

# Intune-PowerShell-SDK-Code-Generator
Este repositorio implementa el escritor de Vipr que genera cmdlets de PowerShell para todas las operaciones CRUD, las funciones y las acciones para compatibilidad con la API de Graph en Microsoft Intune.

# Colaboradores
Este proyecto recibe las contribuciones y las sugerencias. La mayoría de las contribuciones requiere
que acepte un Contrato de Licencia de Colaborador (CLA) donde declara que tiene el derecho, y realmente lo tiene, de otorgarnos los derechos para usar su contribución.
Para obtener más información, visite https://cla.microsoft.com.

Cuando envíe una solicitud de incorporación de cambios, un bot de CLA determinará automáticamente si necesita proporcionar un CLA y agregar el PR correctamente (por ejemplo, etiqueta, comentario).
Siga las instrucciones proporcionadas por el bot.
Solo debe hacerlo una vez en todos los repositorios que usen nuestro CLA.

Este proyecto ha adoptado el [Código de conducta de código abierto de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para obtener más información, vea [preguntas frecuentes sobre el código de conducta](https://opensource.microsoft.com/codeofconduct/faq/)
o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) si tiene otras preguntas o comentarios.

# Diseño
## Arquitectura de alto nivel
![Arquitectura de alto nivel](Design.jpg)
esta es la información general sobre cómo se junta el módulo de PowerShell.
1. El Vipr.exe es ejecutado con los siguientes argumentos para generar el código que definirá el comportamiento de los cmdlets.
- Los resultados de la compilación del proyecto GraphODataPowerShellWriter (es decir, GraphODataPowerShellWriter.dll).
- Un esquema de Graph (por ejemplo, el resultado de llamar a https://graph.microsoft.com/v1.0/$metadata) - este archivo tiene la extensión ".csdl" y se encuentra en "~/TestGraphSchemas".
2. La documentación generada en la salida de Vipr es extraída en un archivo XML. PowerShell usa el archivo XML para mostrar el resultado del cmdlet "Get-Help".
3. Estos cmdlets se escriben en sus propios módulos, debido a que algunas funcionalidades cobran más sentido al ser escritas a mano en PowerShell. Estos módulos se encuentran en "~/src/PowerShellGraphSDK/PowerShellModuleAdditions/CustomModules".
4. Los resultados de los tres pasos anteriores (es decir, los cmdlets generados, la documentación del cmdlet y los cmdlets escritos a mano) se combinan al usar un archivo de manifiesto de PowerShell. Este archivo tiene la extensión ".psd1".
5. El archivo de manifiesto de PowerShell creado en el paso anterior se utiliza para importar y utilizar el módulo. Por ejemplo, si el archivo de manifiesto se llamó "Intune.psd1", puede ejecutar "Import-Module ./Intune.psd1" en una ventana de PowerShell para importar el módulo.

## El paso de generación automática
![ genera el módulo binario de PowerShell](Generating_the_PowerShell_Binary_Module.jpg)
este es un examen detallado del paso 1 en la sección de[arquitectura de alto nivel](#high-level-architecture).
- El archivo de definición de esquema (CSDL) se carga en Vipr.
- La v4 del lector OData está incorporada en Vipr y se usa para leer el archivo CSDL. Este lector convierte la definición del esquema en una representación intermedia, denominada modelo ODCM.
- El escritor personalizado se pasó a Vipr para procesar el modelo ODCM. Para convertir el modelo ODCM en los archivos C# finales generados, el escritor consta de 4 pasos:
1. Recorra el modelo de ODCM (es decir, el esquema) para detectar todas las rutas.
2. Cree una representación abstracta de los cmdlets de cada una de las rutas que representan las operaciones en cada una.
3. Convertir todas las representaciones cmdlet abstractas en una representación C# abstracta.
4. Convierta cada representación C# abstracta en una cadena que se escribirá como el contenido de un archivo.

# Trabajos futuros
- [ ] Mejorar los nombres de los cmdlets generados.
    - [ ] Ver si podemos agregar anotaciones al esquema de gráficos que se puedan usar para crear nombres descriptivos para las rutas.
- [ ] Buscar un método para generar y crear cmdlets para todo el esquema de gráficos en una cantidad de tiempo razonable.
    - El número de rutas (debido a las propiedades de navegación) aumenta considerablemente una vez que las entidades de AAD son incluidas.
- [ ] Obtener la documentación de ayuda generada del cmdlet para incluir el vínculo a la página correspondiente en la documentación oficial de Graph.
- [x] Implementar la canalización de las salidas del cmdlet en otros cmdlets.
- [ ] Implementar la autenticación no interactiva.
    - [ ] Autenticación con certificados.
    - [x] Autenticación con un objeto PSCredential.
- [x] Asegurarse de obtener el tiempo de expiración correcto del token de autenticación, el cual se controla cuando es necesaria la actualización automática del token.
- [x] Crear un repositorio independiente para los cmdlets de escenario (que se serán usados como submódulo).
- [ ] Travis CI de integración de GitHub y los casos de prueba.
- [x] Crear un archivo de licencia para el software de terceros usado e incluirlo en el módulo.
- [x] Actualiza el archivo \*.psd1 para incluir los vínculos correctos al correo público de GitHub, el archivo de licencia y el icono.
- [x] Generar cmdlets de la aplicación de ayuda para crear objetos que se puedan serializar para los tipos definidos en el esquema.
- [ ] Obtener las restricciones de capacidad agregadas de OData al esquema de gráficos para que los cmdlets que realizan operaciones no permitidas no sean generados en primer lugar.

# Preguntas frecuentes
## ¿Dónde está el código generado?
GitHub: https://github.com/Microsoft/Intune-PowerShell-SDK

## ¿Por qué necesitamos este módulo de PowerShell?
Los clientes han expresado mucho interés en el uso de PowerShell para automatizar las tareas que se ejecutan actualmente con la extensión de Intune en el portal de Azure.

## ¿Por qué queremos generar automáticamente el módulo de PowerShell?
Si escribimos cada cmdlet manualmente, también tendríamos que actualizar el módulo manualmente cada vez que el esquema del gráfico sea actualizado.

## ¿Por qué la compilación está siendo realizada por Intune (y por qué no la está realizando el equipo SDK de Graph)?
La mayor parte de la base de usuarios de Intune (profesionales de la tecnología de la información) preferiría trabajar con PowerShell, mientras que el SDK de gráficos está dirigido a desarrolladores que preferirían trabajar con, por ejemplo, C#, Java y Python. Por esto lo necesitamos de forma urgente. La estructura de la sintaxis de PowerShell es incompatible con la estructura del actual generador de SDK de .Net/Java/Python (vea la explicación de por qué tenemos un [escritor personalizado de Vipr](#why-a-custom-writer)).

## ¿Qué es Vipr?
- La arquitectura es creada en torno a un proyecto de investigación de Microsoft denominado Vipr.
- Generalizando el concepto de transformar la información de una sintaxis a otra.
- Es esencialmente un analizador de texto modular que se adecua especialmente para las definiciones de servicio al estilo OData.
- El ejecutable acepta un montaje de lector y uno de escritor.
    - El lector está capacitado para analizar el archivo de entrada de texto en la representación (genérica) intermedia de Vipr.
    - El escritor admite la representación intermedia y devuelve una lista de los objetos de archivo, es decir, pares de cadena o ruta de acceso.
        - Luego, Vipr escribe estos archivos en la estructura de carpetas que le corresponde a la carpeta de salida.

## ¿Por qué un escritor personalizado?
No hay ningún escritor existente de PowerShell disponible para Vipr. Asimismo, el escritor del SDK de gráficos .Net/Java/Python no se prestará fácilmente a la sintaxis, el comportamiento o los procedimientos recomendados de PowerShell.

## ¿Por qué no usar OpenAPI y Swagger?
Esto podría haber sido posible, pero requeriría una transformación adicional del esquema OData de Graph al formato OpenAPI. Cada vez que se realiza una transformación, se corre el riesgo de perder información que podría ser valiosa en el módulo generado de PowerShell.