# SharpCut

**SharpCut** is a modern, precise C# library for generating and manipulating 2D vector shapes for laser cutting, CNC, and more. It supports compound shapes, edge merging, geometric utilities, and clean SVG export.

📦 [Available on NuGet](https://www.nuget.org/packages/SharpCutSvg/)

---

## ✨ Features

- Define axis-aligned rectangles and custom shapes  
- Compose compound shapes with automatic edge overlap removal  
- Build clean, deduplicated closed paths  
- Export compact SVG output with one path per shape  
- Control stroke width, color, units  
- Automatically size SVG documents to fit content  
- Fully typed with nullable reference types enabled  
- No dependencies – works cross-platform with .NET 8+

---

## 📦 Example Usage

### Basic Shape to SVG

```csharp
using SharpCut;
using SharpCut.Models;

// Define a simple rectangle
Rectangle rectangle = new Rectangle(0, 0, 100, 50);

// Create document, add the shape, resize and export
SvgDocument svg = new SvgDocument();
svg.Add(rectangle, copy: true);
svg.ResizeToFitContent(margin: 5, offsetContent: true);

string content = svg.Export();
```

---

### Compound Shape with Finger Joints

```csharp
using SharpCut;
using SharpCut.Models;
using SharpCut.Builders;

// Base panel
Rectangle panelBase = new Rectangle(x: 5, y: 5, width: 160, height: 50);

// Small vertical finger joint shape
Rectangle cut = new Rectangle(width: 3, height: 25);

// Distribute tabs evenly along the bottom of the base panel
List<Rectangle> placedCuts = cut.PlaceCopiesOnPoints(
    points: panelBase.GetEdge(Side.Bottom).GetDistributedPoints(2),
    origin: Origin.BottomCenter
);

// Build compound shape and clean overlapping edges
CompoundShape compoundShape = new CompoundShape(panelBase, placedCuts);

// Export to SVG
SvgDocument svg = new SvgDocument(strokeWidth: 0.1f, unit: "mm");
svg.Add(compoundShape, copy: true);
svg.ResizeToFitContent(margin: 5, offsetContent: true);

string exportedSvg = svg.Export();
```

---

## 🧱 Structure

- `SharpCut.Models` – geometry: `Point`, `Edge`, `Rectangle`, `Shape`, `CompoundShape`, `Origin`, `Side`  
- `SharpCut.Builders` – logic: `ShapeBuilder` for deduplicating edges  
- `SharpCut` – output: `SvgDocument` for exporting shapes as SVG

---

## 🧪 Testing

SharpCut uses **MSTest** with comprehensive coverage:

- `EdgeTests`  
- `PointTests`  
- `RectangleTests`  
- `ShapeTests`  
- `ShapeBuilderTests`  
- `SvgDocumentTests`

Run tests via:

```bash
dotnet test
```

---

## 🛠 Requirements

- .NET 8  
- No external dependencies  

---

## 📄 License

MIT License
(You can do whatever you want with the code, please feel free to contribute if you have any improvement suggestions)