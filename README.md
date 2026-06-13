# 🗳️ eVote360 Pro — Sistema de Votación Electrónica

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512bd4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-v4-38bdf8?logo=tailwind-css)](https://tailwindcss.com/)
[![Bun](https://img.shields.io/badge/Bun-Runtime-f9f1e1?logo=bun)](https://bun.sh/)
[![Architecture](https://img.shields.io/badge/Architecture-Onion-green)](https://en.wikipedia.org/wiki/Multitier_architecture#Common_layers)

**eVote360 Pro** es una plataforma web avanzada diseñada para gestionar de manera integral y segura el ciclo completo de un proceso electoral. Desde el registro y validación por **reconocimiento óptico de caracteres (OCR)** de los ciudadanos, hasta la configuración dinámica de elecciones, partidos, alianzas, asignación de candidaturas y la **confidencialidad absoluta** en la emisión del voto.

---

## 🌟 Funcionalidades Destacadas

*   **🛡️ Seguridad Nivel Bancario:** Implementación estricta de **RBAC (Role Based Access Control)** y hashing de alta seguridad para la protección de datos sensibles.
*   **👁️ Validación Biométrica/OCR:** Registro ciudadano inteligente que utiliza **Tesseract OCR** para extraer datos automáticamente de documentos de identidad, reduciendo errores humanos.
*   **🤝 Gestión de Alianzas Políticas:** Módulo robusto para la creación y gestión de alianzas entre partidos, con estados dinámicos.
*   **🗳️ Proceso de Votación Blindado:** Sistema que asegura que cada ciudadano vote una sola vez, manteniendo el anonimato total del voto mediante desacoplamiento en la base de datos.
*   **📊 Analítica en Tiempo Real:** Dashboard administrativo con métricas de participación, tendencias y resultados preliminares inmediatos.
*   **📧 Sistema de Notificaciones:** Integración con **MailKit** y plantillas **Razor** para el envío de códigos de verificación y confirmaciones.

---

## 🧩 Módulos del Sistema

El sistema está compuesto por servicios especializados que cubren todas las necesidades electorales:

| Módulo | Descripción |
| :--- | :--- |
| **Auth & User** | Gestión de sesiones seguras, perfiles y recuperación de cuentas. |
| **Citizen & OCR** | Registro de votantes con escaneo automático de documentos. |
| **Election Engine** | Configuración de periodos electorales, fechas y estados. |
| **Political Entities** | Manejo de Partidos, Alianzas y Líderes Políticos. |
| **Candidacy** | Asignación de candidatos a puestos electivos específicos. |
| **Voting System** | Motor de emisión de votos con validación de participación. |
| **Results & Stats** | Procesamiento de escrutinio y generación de resultados. |

---

## 🏗️ Arquitectura Onion (Clean Architecture)

El proyecto está estructurado para ser mantenible, escalable y fácil de testear:

1.  **`eVote360_Pro.Domain`**: El corazón del sistema. Contiene las entidades (`Candidate`, `Citizen`, `Vote`, etc.), Enums, excepciones de dominio y contratos básicos.
2.  **`eVote360_Pro.Application`**: Contiene la lógica de aplicación, servicios (`VotingService`, `ElectionService`), DTOs para transferencia de datos y validadores con **FluentValidation**.
3.  **`eVote360_Pro.Infrastructure`**: Implementaciones técnicas. Acceso a datos con **EF Core**, repositorios, seguridad, OCR y mensajería.
4.  **`eVote360_Pro.WebApp`**: La capa de presentación (MVC). Controladores optimizados, ViewModels y estilos modernos con **Tailwind CSS**.

---

## 🛠️ Stack Tecnológico

*   **Lenguaje:** C# 13 / .NET 9
*   **Base de Datos:** SQL Server + Entity Framework Core (Code First)
*   **Frontend:** ASP.NET Core MVC + Razor Views + **Tailwind CSS v4**
*   **Herramientas de Build:** **Bun** para la gestión y compilación de activos CSS.
*   **Librerías Clave:** AutoMapper, FluentValidation, MailKit, Tesseract.NET, RazorLight.

---

## ⚙️ Configuración Rápida

1.  **Base de Datos:** Configura tu `ConnectionStrings:eVote360Db` en `appsettings.json`.
2.  **Archivos:** Define `FileSettings:BasePath` para el almacenamiento de fotos/documentos.
3.  **Correo:** Configura `SmtpSettings` para habilitar el envío de notificaciones.
4.  **Frontend:**
    ```bash
    cd eVote360_Pro.WebApp
    bun install
    bun run css:build
    ```
5.  **Migraciones:**
    ```bash
    dotnet ef database update --project eVote360_Pro.Infrastructure --startup-project eVote360_Pro.WebApp
    ```

---
*Desarrollado como una solución integral para la transparencia democrática.*
