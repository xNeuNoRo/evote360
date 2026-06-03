# Evote360
 eVote360 Pro — Sistema de Votación Electrónica

Una plataforma web avanzada diseñada para gestionar de manera integral y segura el ciclo completo de un proceso electoral desde el registro y validación por reconocimiento óptico de caracteres  de los ciudadanos, hasta la configuración dinámica de elecciones, partidos, alianzas, asignación de candidaturas y la confidencialidad absoluta en la emisión del voto[



---

## 🏗️ Arquitectura del Proyecto
El sistema se implementa siguiendo la **Arquitectura Onion** asegurando un acoplamiento débil, alta testabilidad y separación clara de responsabilidades a través de las siguientes capas:

1. **Domain (Núcleo):** Contiene las entidades del negocio, enumeraciones y lógica fundamental exenta de dependencias externas.
2. **Application (Servicios):** Implementa los casos de uso del sistema, interfaces y la transferencia de datos mediante **DTOs (Data Transfer Objects)**
3. **Infrastructure (Persistencia/Externo):** Configuración de la base de datos, repositorios, Entity Framework Core y servicios de terceros (como el motor OCR y envío de correos).
4. **Presentation (Web MVC):** Capa visual e interactiva que utiliza controladores y **ViewModels** con anotaciones de datos (`DataAnnotations`) para la validación estricta de formularios desde la vista[cite: 140].

---




