<!--
Sync Impact Report
- Version change: N/A → 1.0.0 (initial ratification)
- Added sections: Preamble, Principles (7), Governance, Compliance
- Removed sections: None
- Follow-up TODOs: None
-->

# Constitution: FinanzApp Estudiantil

**Version**: 1.0.0
**Ratification Date**: 2026-08-20
**Last Amended Date**: 2026-08-20

---

## Preamble

FinanzApp Estudiantil es una aplicación web de finanzas personales diseñada
exclusivamente para estudiantes universarios. Esta constitución establece
los principios fundamentales que guían el diseño, desarrollo y evolución
del producto.

---

## Principles

### 1. Enfoque en el Usuario Estudiante

La interfaz debe ser simple, intuitiva y accesible. Se prohíbe el uso de
jerga bancaria o financiera compleja. El tono de comunicación MUST ser
cercano, motivador y libre de lenguaje corporativo. Cada pantalla,
componente y mensaje MUST evaluarse desde la perspectiva de un estudiante
sin experiencia financiera formal.

**Rationale**: La mayoría de estudiantes universarios no tienen formación
financiera y abandonan apps que perciben como complicadas o impersonales.

### 2. Consistencia Visual

La paleta de colores principal MUST ser verde (asociado a dinero y
crecimiento), con acentos en morado y amarillo. La tipografía MUST ser
redondeada y amigable. Las ilustraciones MUST seguir el estilo flat design.
Las gráficas y visualizaciones MUST ser claras, legibles y constituir el
elemento central de la experiencia.

**Rationale**: La consistencia visual genera confianza y facilita la
identificación de la marca. El verde refuerza la asociación con finanzas
saludables.

### 3. Claridad de Datos

Toda visualización de gastos, ingresos o estadísticas MUST entenderse de
un vistazo. Se prohíbe asumir conocimientos financieros previos en el
usuario. Cuando se presente información compleja, MUST incluirse una
explicación breve y accesible. Las gráficas MUST usar etiquetas claras,
colores diferenciados y tooltips descriptivos.

**Rationale**: La app fracasa si el usuario no puede interpretar sus datos
financieros sin buscar definiciones externas.

### 4. Consejos de IA Honestos y Accionables

Los consejos generados por inteligencia artificial MUST basarse
exclusivamente en los datos reales del usuario. MUST ser específicos,
accional y verificables. Está estrictamente prohibido ofrecer consejos
genéricos, vagos o basados en suposiciones. Cuando la información del
usuario sea insuficiente para un consejo confiable, la IA MUST indicarlo
claramente.

**Rationale**: Consejos genéricos erosionan la confianza del usuario y
pueden llevar a decisiones financieras perjudiciales.

### 5. Privacidad

Los datos financieros del usuario se tratan como información sensible.
Todos los datos MUST cifrarse en reposo y en tránsito. Se prohíbe
compartir, vender o exponer datos financieros a terceros sin
consentimiento explícito. La app MUST cumplir con las regulaciones de
protección de datos aplicables. El usuario MUST poder eliminar sus datos
completamente en cualquier momento.

**Rationale**: La confianza del usuario depende de saber que su información
financiera está protegida con los más altos estándares.

### 6. Mobile-First

Todos los componentes y pantallas MUST diseñarse primero para dispositivos
móviles. El responsive design MUST garantizar una experiencia óptima en
cualquier tamaño de pantalla. Las interacciones táctiles MUST ser
naturales y accesibles. El contenido MUST priorizarse jerárquicamente
para pantallas pequeñas.

**Rationale**: Los estudiantes universarios utilizan mayormente sus
teléfonos celulares para gestionar sus finanzas.

### 7. Rendimiento

El dashboard y las gráficas MUST cargar rápidamente incluso con grandes
volúmenes de registros de gastos. Se MUST implementar paginación,
virtualización o lazy loading cuando sea necesario. Las métricas de
rendimiento MUST monitorearse continuamente. La experiencia MUST ser fluida
en conexiones de red variables.

**Rationale**: Un rendimiento lento causa frustración y abandono,
especialmente en usuarios con dispositivos de gama baja o conexiones
limitadas.

---

## Governance

### Amendment Procedure

1. Cualquier cambio a esta constitución MUST ser propuesto y documentado.
2. Los cambios MUST revisarse en relación con los 7 principios establecidos.
3. Las enmiendas MUST incrementar la versión según semver:
   - MAJOR: Eliminación o redefinición de principios existentes.
   - MINOR: Adición de nuevos principios o expansión material de guías.
   - PATCH: Aclaraciones, correcciones de redacción, refinamientos no
     semánticos.

### Versioning Policy

La versión del documento sigue semantic versioning (MAJOR.MINOR.PATCH).
Cada enmienda MUST actualizar la fecha de última modificación y el número
de versión.

### Compliance Review

Antes de cada release significativo, el equipo MUST revisar que el
producto cumpla con todos los principios de esta constitución. Las
violaciones detectadas MUST documentarse y priorizarse para corrección.

---

## Appendix: Deferred Intents

No hay intenciones diferidas. Todos los principios proporcionados por el
usuario han sido incorporados en esta constitución.
