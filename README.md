# Sitecore SCS T4 Code Generator

![.NET](https://img.shields.io/badge/.NET-6.0+-512BD4?logo=.net)
![Sitecore](https://img.shields.io/badge/Sitecore-CLI_Ready-red?logo=sitecore)
![License](https://img.shields.io/badge/License-MIT-green.svg)

A standalone, enterprise-grade .NET console application that bridges the gap between modern **Sitecore Content Serialization (SCS)** and legacy **T4 Code Generation**.

As teams migrate away from legacy tools like TDS toward the modern Sitecore CLI, preserving existing T4 templates is often the biggest roadblock. This tool reads raw SCS YAML files, maps complex Helix inheritance graphs in memory, and feeds that data directly into the `Mono.TextTemplating` engine—allowing you to migrate to SCS without changing a single line of your generated C# models.

---

## ✨ Features

* **Zero-Configuration Discovery:** Automatically traverses the directory tree to locate your Helix root (`Foundation`, `Feature`, `Project`) and your T4 templates. No hardcoded absolute paths required.
* **Global Inheritance Resolution:** Reads the entire Sitecore YAML tree into memory first, guaranteeing that cross-layer base templates (e.g., a Feature inheriting from a Foundation template) resolve perfectly.
* **Targeted Generation:** Process the entire solution or execute against a single specific module to drastically speed up local developer workflows.
* **Custom Namespace Routing:** A `namespace-mappings.json` file allows you to safely map rogue third-party or legacy Sitecore paths into clean, Helix-compliant C# namespaces.
---

## 🏗️ Architecture

This application is built using **SOLID** principles and relies on a decoupled **Factory + Service** pattern.

* **`YamlDataService`:** Crawls the repository and deserializes `.yml` files using `YamlDotNet`.
* **`SitecoreMappingService`:** Parses the raw YAML, identifies Template GUIDs, and builds the inheritance graph.
* **`TemplateGenerationService`:** Executes the legacy `.tt` files programmatically using `Mono.TextTemplating`.
* **`CodeGenerationService`:** The core orchestrator that routes data between services.

---

## 🚀 Getting Started

### Prerequisites
* **.NET 6.0 SDK** (or higher)
* **Sitecore CLI** (installed locally via `dotnet tool restore`)
* Existing T4 templates (`header.tt`, `item.tt`, etc.)

### Installation
1. Clone the repository:
   ```bash
   git clone [https://github.com/prabhu-ranganathan/scs-code-generator.git](https://github.com/prabhu-ranganathan/scs-code-generator.git)
