# CGRuleEngine

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🇬🇧 English

### Description
CGRuleEngine is an agnostic and modular rule engine designed to manage the logic of card video games. It acts as an independent core that handles game execution, players, zones, cards, effects, and turns, allowing developers to focus on the visual representation and specific mechanics of their game. You can find the complete report on the following link: https://drive.google.com/file/d/1ry9sQ37D3C4ZeJoiHufHMRKxWUogb1Eh/view?usp=sharing

### Features
*   **Data-Oriented Definition:** Design cards and their properties easily using JSON files.
*   **Automated Game Loop:** Manages turn flow, priorities, and win conditions automatically.
*   **Action & Response Management:** Includes a stack system (LIFO) for resolving effects and counter-actions efficiently.
*   **Highly Modular:** Built with standard design patterns (Builder, Factory, Strategy, Command, State) to allow easy extension of cards, zones, effects, and rules without modifying the core engine.
*   **Agnostic:** Compatible with any game engine or platform that supports C# (e.g., Godot Engine, Unity).

### JSON Card Example
CGRuleEngine allow the card design by using the following JSON format:
```JSON
{
    "Type":  "NormalCard",

    "Id": "TestCard",
    "Name": "The best card",
    "SideA": "",
    "SideB": "",

    "Tags": [
        "Scorcery"
    ],

    "Properties": {
        "Cost": 10
    },

    "Effects": [
        {
            "Type": "ModifyPropertyEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "AffectedProperty": "Health",
                "Value": 5,
                "Operation": "Subtract"
            }
        },
        {
            "Type": "ModifyTagEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "Tag": "Fly",
                "Operation": "Remove"
            }
        },
        {
            "Type": "DrawEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "Amount": 2
            }
        }
    ]
}
```

### Technologies
*   C#
*   .NET Standard 2.1
*   Newtonsoft.JSON
*   NuGet

### Installation
Generate the NuGet package (`.nupkg`) from the source code and add the reference to your project's configuration. The engine can be integrated into Godot, Unity, or any standard .NET environment.

### License
This project is licensed under the MIT License - see the LICENSE section below for details.

---

## 🇪🇸 Español

### Descripción
CGRuleEngine es un motor de reglas agnóstico y modular diseñado para gestionar la lógica de videojuegos de cartas. Actúa como un núcleo independiente que maneja la ejecución de la partida, jugadores, zonas, cartas, efectos y turnos, permitiendo a los desarrolladores centrarse en la representación visual y las mecánicas específicas de su juego. Puedes encontrar la memoria completa en el siguiente enlace: https://drive.google.com/file/d/1ry9sQ37D3C4ZeJoiHufHMRKxWUogb1Eh/view?usp=sharing

### Características
*   **Definición orientada a datos:** Diseña cartas y sus propiedades de forma sencilla utilizando archivos JSON.
*   **Ciclo de juego automatizado:** Gestiona el flujo de turnos, prioridades de los jugadores y condiciones de victoria de forma automática.
*   **Gestión de acciones y respuestas:** Incluye un sistema de pila (LIFO) para la resolución de efectos y respuestas a acciones del rival.
*   **Altamente modular:** Construido mediante patrones de diseño (Builder, Factory, Strategy, Command, State) para permitir la extensión de cartas, zonas, efectos y reglas sin necesidad de alterar el núcleo del motor.
*   **Agnóstico:** Compatible con cualquier motor gráfico o entorno que soporte el lenguaje C# (ej. Godot Engine, Unity).

### Ejemplo de Carta JSON
Puedes definir tus cartas de manera sencilla utilizando un formato estructurado en JSON:
```JSON
{
    "Type":  "NormalCard",

    "Id": "TestCard",
    "Name": "The best card",
    "SideA": "",
    "SideB": "",

    "Tags": [
        "Scorcery"
    ],

    "Properties": {
        "Cost": 10
    },

    "Effects": [
        {
            "Type": "ModifyPropertyEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "AffectedProperty": "Health",
                "Value": 5,
                "Operation": "Subtract"
            }
        },
        {
            "Type": "ModifyTagEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "Tag": "Fly",
                "Operation": "Remove"
            }
        },
        {
            "Type": "DrawEffect",
            "Parameters": {
                "Timing": "OnPlay",
                "Amount": 2
            }
        }
    ]
}
```

### Tecnologías
*   C#
*   .NET Standard 2.1
*   Newtonsoft.JSON
*   NuGet

### Instalación
Genera el paquete NuGet (`.nupkg`) a partir del código fuente y añade la referencia a la configuración de tu proyecto. El motor puede integrarse fácilmente en Godot, Unity o cualquier entorno .NET estándar.

### Licencia
Este proyecto se distribuye bajo la Licencia MIT. Consulta la sección inferior para más detalles.

---

## License / Licencia (MIT)

Copyright (c) 2026 Francisco Fernández Ugidos

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
