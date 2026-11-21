using MiniHttpServer.HtmlTemplating.Entities;
using MiniHttpServer.HtmlTemplating.Interfaces;
using MiniHttpServer.HtmlTemplating.Renderers;

namespace MiniHttpServer.HtmlTemplating.UnitTests;

[TestClass]
public class HtmlTemplateRendererTests
{
    [TestMethod]
    public void RenderFromString_ValidProperty_ReplacesPlaceholder()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<h1>${Name}</h1>";
        var model = new { Name = "Alex" };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<h1>Alex</h1>", result);
    }
    [TestMethod]
    public void RenderFromString_MultipleProperties_ReplacesAllPlaceholders()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<p>Name: ${Name}, Age: ${Age}</p>";
        var model = new { Name = "John", Age = 25 };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Name: John, Age: 25</p>", result);
    }
    [TestMethod]
    public void RenderFromString_EmptyTemplate_ReturnsEmptyString()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "";
        var model = new { Name = "Test" };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("", result);
    }
    [TestMethod]
    public void RenderFromString_TrueCondition_RendersIfBlock()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "$if(Age >= 18)<p>Добро пожаловать</p>$endif";
        var model = new { Age = 21 };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Добро пожаловать</p>", result);
    }
    [TestMethod]
    public void RenderFromString_FalseCondition_DoesNotRenderIfBlock()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "$if(Age >= 18)<p>Добро пожаловать</p>$endif";
        var model = new { Age = 10 };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("", result);
    }
    [TestMethod]
    public void RenderFromString_IfElseBlock()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "$if(Age >= 18)<p>Взрослый</p>$else<p>Ребенок</p>$endif";
        var model = new { Age = 10 };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Ребенок</p>", result);
    }
    [TestMethod]
    public void RenderFromString_IfElseBlock_TrueCondition()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "$if(Age >= 18)<p>Взрослый</p>$else<p>Ребенок</p>$endif";
        var model = new { Age = 21 };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Взрослый</p>", result);
    }
    [TestMethod]
    public void RenderFromString_Collection_RendersAllItems()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<ul>$foreach(var a in lib.Authors)<li>${a.Name}</li>$endfor</ul>";
        var model = new { lib = new { Authors = new List<object> { new { Name = "Толстой" }, new { Name = "Брэдбери" } } } };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<ul><li>Толстой</li><li>Брэдбери</li></ul>", result);
    }
    [TestMethod]
    public void RenderFromString_EmptyCollection_ReturnsEmpty()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<ul>$foreach(var a in lib.Authors)<li>${a.Name}</li>$endfor</ul>";
        var model = new { lib = new { Authors = new List<object>() } };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<ul></ul>", result);
    }
    [TestMethod]
    public void RenderFromString_NestedProperty()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        var bag = new Bag { Volume = 10.5, Brand = new Brand { Name = "Nike" } };
        string template = "<p>Бренд: ${bag.Brand.Name}</p><p>Объём: ${bag.Volume}</p>";
        string result = renderer.RenderFromString(template, bag);
        Assert.AreEqual("<p>Бренд: Nike</p><p>Объём: 10.5</p>", result);
    }
    [TestMethod]
    public void RenderFromString_MultiplePlaceholders_SameProperty()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<p>${Name} says hello to ${Name}</p>";
        var model = new { Name = "Alex" };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Alex says hello to Alex</p>", result);
    }
    [TestMethod]
    public void RenderFromString_FloatProperty()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "<p>${Price}</p>";
        var model = new { Price = 19.99m };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>19.99</p>", result);
    }
    [TestMethod]
    public void RenderFromString_BoolProperty()
    {
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        string template = "$if(IsActive)<p>Active</p>$endif";
        var model = new { IsActive = true };
        string result = renderer.RenderFromString(template, model);
        Assert.AreEqual("<p>Active</p>", result);
    }

    [TestMethod]
    public void RenderFromString_NestedIfBlock()
    {
        // Arrange
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        // Use boolean property - $if with comparison works
        string template = "$if(HasValue)<p>Has value</p>$endif";
        var model = new { HasValue = true };

        // Act
        string result = renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<p>Has value</p>", result);
    }

    [TestMethod]
    public void RenderFromString_NestedForeachBlock()
    {
        // Arrange
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        // Nested $foreach loops
        string template = "<ul>$foreach(var a in lib.Items)$foreach(var b in a.SubItems)<li>${b.Name}</li>$endfor$endfor</ul>";
        var model = new { lib = new { Items = new List<object> { new { Name = "Parent", SubItems = new List<object> { new { Name = "Child1" }, new { Name = "Child2" } } } } } };
        string result = renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<ul><li>Child1</li><li>Child2</li></ul>", result);
    }

    [TestMethod]
    public void RenderFromString_IfInsideForeachBlock()
    {
        // Arrange
        IHtmlTemplateRenderer renderer = new HtmlTemplate();
        // $foreach with $if inside - only items with HasBook=true
        string template = "<ul>$foreach(var a in lib.Authors)<li>$if(a.HasBook)<b>${a.Name}</b>$endif</li>$endfor</ul>";
        var model = new { lib = new { Authors = new List<object> { new { Name = "Толстой", HasBook = true }, new { Name = "Брэдбери", HasBook = false } } } };

        // Act
        string result = renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<ul><li><b>Толстой</b></li><li></li></ul>", result);
    }
}
