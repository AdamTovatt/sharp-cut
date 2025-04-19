# SharpCut

**SharpCut** is a C# library for generating precise 2D shapes for laser cutting, including automatic edge overlap removal and clean SVG output.

## ✨ Features

- Define axis-aligned rectangles with precise dimensions  
- Automatically removes overlapping edges between shapes  
- Build clean, deduplicated path data  
- Export to SVG for laser cutting software  
- Symmetrical or ordered edge resolution  
- Fully typed, null-safe (nullable reference types enabled)

## 📦 Example Usage

```csharp
// Define outer and cutout rectangles
Rectangle outer = new Rectangle(0, 0, 40, 60);
Rectangle cutout = new Rectangle(10, 30, 20, 30);

// Build final shape (removes overlapping edges)
Shape shape = ShapeBuilder.Build(new List<IShape> { outer, cutout }, symmetrical: true);

// Export to SVG
SvgDocument doc = new SvgDocument(50, 70);
doc.AddShape(shape);
string svg = doc.Export();
```

## 🔧 Structure

- `SharpCut.Models` – core geometry: `Point`, `Edge`, `Rectangle`, `IShape`, `Shape`  
- `SharpCut.Builders` – logic: `ShapeBuilder` for merging shapes  
- `SharpCut` – output: `SvgDocument`

## ✅ Testing

The project uses **MSTest** with a full suite of tests:

- `EdgeTests`
- `RectangleTests`
- `ShapeBuilderTests`
- `SvgDocumentTests`

Run tests with:

```bash
dotnet test
```

## 📂 Project Setup

- .NET 8  
- No external dependencies  
- Cross-platform and culture-invariant (for valid SVG output)

## 📄 License

MIT License
